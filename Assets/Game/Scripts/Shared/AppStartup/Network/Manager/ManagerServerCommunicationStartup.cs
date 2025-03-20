using System;
using Game.Scripts.Foyer.Network.Dto.Lobby.Launch;
using Game.Scripts.Foyer.Network.Service.Lobby;
using Game.Scripts.Gameplay.Shared.Network;
using Game.Scripts.Shared.AppStartup.CommandLine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts.Shared.AppStartup.Network.Manager
{
    public class ManagerServerCommunicationStartup : MonoBehaviour
    {
        [SerializeField] private ushort defaultServerPort = 7777;
        public const string DefaultMatchSceneName = "Game/Scenes/SampleScene";

        private async void Start()
        {
            DontDestroyOnLoad(gameObject);
            var asServer = CommandLineArgs.AsServer;
            if (!asServer)
            {
                return;
            }

            try
            {
                var started = false;
                ushort port = 0;
                while (!started)
                {
                    port = await LobbyLaunchService.GetPort(CommandLineArgs.Secret);
                    if (port == 0)
                    {
                        port = defaultServerPort;
                    }
                    started = Topology.StartAsServer(port);
                }

                NetworkManager.Singleton.SceneManager.LoadScene(DefaultMatchSceneName, LoadSceneMode.Single);
                LobbyLaunchService.LobbyLaunchSuccess(new ServerLaunchSuccessDto
                {
                    secret = CommandLineArgs.Secret,
                    port = port
                });
            }
            catch (Exception e)
            {
                try
                {
                    await LobbyLaunchService.LobbyLaunchError(new ServerLaunchErrorDto
                    {
                        secret = CommandLineArgs.Secret, error = e.Message
                    });
                }
                catch (Exception)
                {
                    // ignored
                }

                Application.Quit(1);
            }
        }
    }
}