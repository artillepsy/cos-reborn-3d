using System;
using System.Threading.Tasks;
using Game.Scripts.Foyer.Network.Dto.Lobby;
using Game.Scripts.Foyer.Network.Http;
using Game.Scripts.Foyer.Network.Sse;
using Game.Scripts.Foyer.Network.WebSocket.Stomp;
using Game.Scripts.Gameplay.Shared.Util;
using Newtonsoft.Json;

namespace Game.Scripts.Foyer.Network.Service.Lobby
{
    public static class LobbyService
    {
        private const string LobbyBaseUrl = "/lobby";
        private const string LobbyWebSocketBroadcastUrl = "/broadcast/lobby";

        public static async Task<LobbyDto> CreateLobby(LobbyDto lobbyDto)
        {
            var jsonLobbyDto = JsonConvert.SerializeObject(lobbyDto);
            var httpRequest = new HttpRequest($"{LobbyBaseUrl}", HttpMethod.Post, jsonLobbyDto);
            jsonLobbyDto = await httpRequest.SendWebRequestAsync();
            return JsonConvert.DeserializeObject<LobbyDto>(jsonLobbyDto);
        }

        [Obsolete]
        public static SseLobbyStream GetSseLobbyStream()
        {
            var url = $"{HttpConfig.BaseUrl}{LobbyBaseUrl}/stream";
            return new SseLobbyStream(url);
        }

        public static WebSocketLobbyStream GetWebSocketLobbyStream(LobbyDto lobbyDto)
        {
            var url = $"{LobbyWebSocketBroadcastUrl}/{lobbyDto.lobbyId}";
            return new WebSocketLobbyStream(url);
        }

        public static async Task<LobbyDto[]> FetchLobbies()
        {
            var httpRequest = new HttpRequest(LobbyBaseUrl, HttpMethod.Get);
            var jsonArr = await httpRequest.SendWebRequestAsync();
            return JsonConvert.DeserializeObject<LobbyDto[]>(jsonArr);
        }

        public static async Task<LobbyDto> JoinLobby(LobbyDto lobbyDto)
        {
            var jsonLobbyDto = JsonConvert.SerializeObject(lobbyDto);
            var httpRequest = new HttpRequest($"{LobbyBaseUrl}/join", HttpMethod.Post, jsonLobbyDto);
            jsonLobbyDto = await httpRequest.SendWebRequestAsync();
            return JsonConvert.DeserializeObject<LobbyDto>(jsonLobbyDto);
        }

        public static async Task Kick(PlayerDto playerDto)
        {
            var jsonPlayerDto = JsonConvert.SerializeObject(playerDto);
            var httpRequest = new HttpRequest($"{LobbyBaseUrl}/kick", HttpMethod.Delete, jsonPlayerDto);
            await httpRequest.SendWebRequestAsync();
        }

        public static async Task<long> GetAdminId(LobbyDto lobbyDto)
        {
            var httpRequest = new HttpRequest($"{LobbyBaseUrl}/admin?lobby-id={lobbyDto.lobbyId}", HttpMethod.Get);
            var jsonAdminId = await httpRequest.SendWebRequestAsync();
            var adminId = JsonConvert.DeserializeObject<long>(jsonAdminId);
            return adminId;
        }

        public static async Task LeaveLobby()
        {
            var url = $"{LobbyBaseUrl}/leave";
            var httpRequest = new HttpRequest(url, HttpMethod.Post);
            await httpRequest.SendWebRequestAsync();
        }
        
        public interface ILobbyStream
        {
            public void Open();
            public void Close();
            
        }
        
        public abstract class ALobbyStreamBase : ILobbyStream
        {
            public delegate void PlayersDelegate(PlayerDto[] players);
            public event PlayersDelegate PlayersRefreshedEvent;
            public event Delegates.VoidDelegate CompleteEvent;
            public event Delegates.VoidStringDelegate ErrorEvent;
            public abstract void Open();
            public abstract void Close();

            protected virtual void OnError(string str)
            {
                ErrorEvent?.Invoke(str);
            }

            protected virtual void OnMessage(string message)
            {
                var players = JsonConvert.DeserializeObject<PlayerDto[]>(message);
                PlayersRefreshedEvent?.Invoke(players);
            }

            protected void OnComplete()
            {
                CompleteEvent?.Invoke();
            }
        }

        public class SseLobbyStream : ALobbyStreamBase
        {
            private readonly SseClient _sseClient;
            private string _url;

            public SseLobbyStream(string url)
            {
                _url = url;
                _sseClient = new SseClient(url);
            }

            public override async void Open()
            {
                _sseClient.OnMessageEvent += OnMessage;
                _sseClient.OnCompleteEvent += OnComplete;
                _sseClient.OnErrorEvent += OnError;
                await _sseClient.StartListen();
            }

            public override void Close()
            {
                _sseClient.EndListen();
                _sseClient.OnMessageEvent -= OnMessage;
                _sseClient.OnCompleteEvent -= OnComplete;
            }
            
        }

        public class WebSocketLobbyStream : ALobbyStreamBase
        {
            private readonly StompSubscriptionMessageListener _stompMessageListener;

            public WebSocketLobbyStream(string url)
            {
                var webSocket = WebSocket.StompWebSocket.Instance;
                _stompMessageListener = new StompSubscriptionMessageListener(webSocket, url);
            }

            public override void Open()
            {
                _stompMessageListener.OnMessage += OnMessage;
                _stompMessageListener.Subscribe();
            }

            public override void Close()
            {
                _stompMessageListener.Unsubscribe();
                _stompMessageListener.OnMessage -= OnMessage;
            }
        }
    }
}