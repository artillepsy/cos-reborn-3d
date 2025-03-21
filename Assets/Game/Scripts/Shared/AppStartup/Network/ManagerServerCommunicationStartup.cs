﻿using System;
using Game.Scripts.Foyer.Network.Dto.Lobby.Launch;
using Game.Scripts.Foyer.Network.Service.Lobby;
using Game.Scripts.Gameplay.Shared.Network;
using Game.Scripts.Shared.AppStartup.CommandLine;
using Game.Scripts.Shared.Logging;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts.Shared.AppStartup.Network
{
    public class ManagerServerCommunicationStartup : MonoBehaviour
    {
        [SerializeField] private ushort defaultServerPort = 7777;
        public const string DefaultMatchSceneName = "Game/Scenes/Game";

        private int _connectedPlayersCount = 0;

        public bool AllClientsConnected { get; private set; } = false;

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
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                while (!started)
                {
                    port = await LobbyLaunchService.GetPort(CommandLineArgs.ArgsS.Secret);
                    if (port == 0)
                    {
                        port = defaultServerPort;
                    }
                    started = Topology.StartAsServer(port);
                }

                NetworkManager.Singleton.SceneManager.LoadScene(DefaultMatchSceneName, LoadSceneMode.Single);
                LobbyLaunchService.LobbyLaunchSuccess(new ServerLaunchSuccessDto
                {
                    secret = CommandLineArgs.ArgsS.Secret,
                    port = port
                });
            }
            catch (Exception e)
            {
                try
                {
                    await LobbyLaunchService.LobbyLaunchError(new ServerLaunchErrorDto
                    {
                        secret = CommandLineArgs.ArgsS.Secret, error = e.Message
                    });
                }
                catch (Exception)
                {
                    // ignored
                }

                Application.Quit(1);
            }
        }

        private void OnClientConnected(ulong clientId)
        {
            var neededPlayersCount = CommandLineArgs.ArgsS.PlayersCount;
            _connectedPlayersCount++;
            Log.Inf("Client connected", $"{_connectedPlayersCount}/{neededPlayersCount} connected");
            if (_connectedPlayersCount != neededPlayersCount)
            {
                return;
            }

            AllClientsConnected = true;
        }
    }
}