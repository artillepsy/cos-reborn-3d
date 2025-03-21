using System;
using System.Threading.Tasks;
using Game.Scripts.Foyer.Network.Dto.Lobby;
using Game.Scripts.Foyer.Network.Dto.Lobby.Launch;
using Game.Scripts.Foyer.Network.Http;
using Game.Scripts.Foyer.Network.WebSocket;
using Game.Scripts.Foyer.Network.WebSocket.Stomp;
using Game.Scripts.Gameplay.Shared.Util;
using Newtonsoft.Json;

namespace Game.Scripts.Foyer.Network.Service.Lobby
{
    public static class LobbyLaunchService
    {
        private const string BaseUrl = "/lobby/launch";
        private const string LobbyWebSocketBroadcastUrl = "/broadcast/lobby";

        public static async void LobbyLaunchSuccess(ServerLaunchSuccessDto successDto)
        {
            var url = $"{BaseUrl}/success";
            var jsonDto = JsonConvert.SerializeObject(successDto);
            using var httpRequest = new HttpRequest(url, HttpMethod.Post, jsonDto);
            await httpRequest.SendWebRequestAsync();
        }

        public static async Task LobbyLaunchError(ServerLaunchErrorDto errorDto)
        {
            var url = $"{BaseUrl}/error";
            var jsonDto = JsonConvert.SerializeObject(errorDto);
            using var httpRequest = new HttpRequest(url, HttpMethod.Post, jsonDto);
            await httpRequest.SendWebRequestAsync();
        }

        public static LobbyLaunchResultListener GetLobbyLaunchStream(LobbyDto lobbyDto)
        {
            return new LobbyLaunchResultListener(lobbyDto);
        }

        public static async void LaunchLobby()
        {
            const string url = BaseUrl;
            using var request = new HttpRequest(url, HttpMethod.Post);
            await request.SendWebRequestAsync();
        }

        public static async Task<ushort> GetPort(string secret)
        {
            var url = $"{BaseUrl}/port";
            using var request = new HttpRequest(url, HttpMethod.Post, secret);
            var portStr = await request.SendWebRequestAsync();
            return ushort.Parse(portStr);
        }

        public class LobbyLaunchResultListener : IDisposable
        {
            public delegate void LaunchDelegate(LaunchDto launchDto);

            public event Action LaunchStartedEvent;
            public event LaunchDelegate SuccessEvent;
            public event Action<string> ErrorEvent;
            private readonly StompSubscriptionMessageListener _launchResultStompListener;
            private readonly StompSubscriptionMessageListener _launchStartedResultStompListener;

            public LobbyLaunchResultListener(LobbyDto lobbyDto)
            {
                var subscriptionUrl = $"{LobbyWebSocketBroadcastUrl}/{lobbyDto.lobbyId}/launch";
                var subscriptionStartedUrl = $"{LobbyWebSocketBroadcastUrl}/{lobbyDto.lobbyId}/launch-started";

                _launchResultStompListener = new StompSubscriptionMessageListener(
                    StompWebSocket.Instance, subscriptionUrl);
                _launchResultStompListener.OnMessage += OnMessageSuccessError;
                _launchStartedResultStompListener = new StompSubscriptionMessageListener(
                    StompWebSocket.Instance, subscriptionStartedUrl);
                _launchStartedResultStompListener.OnMessage += OnMessageLaunchStarted;
            }

            private void OnMessageLaunchStarted(string message)
            {
                LaunchStartedEvent?.Invoke();
            }

            private void OnMessageSuccessError(string message)
            {
                var launchDto = JsonConvert.DeserializeObject<LaunchDto>(message);
                if (string.IsNullOrEmpty(launchDto?.error))
                {
                    SuccessEvent?.Invoke(launchDto);
                }
                else
                {
                    ErrorEvent?.Invoke(launchDto?.error);
                }
            }

            public void Listen()
            {
                _launchResultStompListener.Subscribe();
                _launchStartedResultStompListener.Subscribe();
            }

            public void Dispose()
            {
                _launchResultStompListener.OnMessage -= OnMessageSuccessError;
                _launchStartedResultStompListener.OnMessage -= OnMessageLaunchStarted;
                _launchResultStompListener.Unsubscribe();
                _launchStartedResultStompListener.Unsubscribe();
            }
        }
    }
}