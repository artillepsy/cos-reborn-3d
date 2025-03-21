using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Scripts.Foyer.Network.Dto.Lobby;
using Game.Scripts.Foyer.Network.Dto.Lobby.Launch;
using Game.Scripts.Foyer.Network.Service.Auth;
using Game.Scripts.Foyer.Network.Service.Lobby;
using Game.Scripts.Gameplay.Shared.Network;
using Game.Scripts.Gameplay.Shared.Util;
using Game.Scripts.Shared.Logging;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Scripts.Foyer.Ui.Lobby
{
    public class LobbyForm : MonoBehaviour
    {
        public event Action LeaveEvent;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Button _btnLaunch;
        [SerializeField] private Button _btnLeave;
        [SerializeField] private PlayerItem _playerItemPrefab;
        private LobbyLaunchService.LobbyLaunchResultListener _launchLobbyStream;
        public LobbyDto Lobby { get; set; }
        private LobbyService.ALobbyStreamBase _lobbyStream;

        private UnityAction _actionLeave;
        private UnityAction _actionLaunch;

        private bool _canKickOthers;

        private void Awake()
        {
            _btnLaunch.gameObject.SetActive(false);
            InitActions();
        }

        private async void OnEnable()
        {
            SetInteractable(true);
            var adminId = await LobbyService.GetAdminId(Lobby);
            if (adminId == AuthService.UserId)
            {
                _btnLaunch.gameObject.SetActive(true);
                _canKickOthers = true;
            }
            else
            {
                _btnLaunch.gameObject.SetActive(false);
                _canKickOthers = false;
            }
            _btnLaunch.onClick.AddListener(_actionLaunch);
            _btnLeave.onClick.AddListener(_actionLeave);
            _lobbyStream = LobbyService.GetWebSocketLobbyStream(Lobby);
            _lobbyStream.PlayersRefreshedEvent += PlayersRefresh;
            _lobbyStream.ErrorEvent += OnError;
            _lobbyStream.Open();

            _launchLobbyStream = LobbyLaunchService.GetLobbyLaunchStream(Lobby);
            _launchLobbyStream.SuccessEvent += OnLobbyLaunchSuccess;
            _launchLobbyStream.ErrorEvent += OnLobbyLaunchError;
            _launchLobbyStream.LaunchStartedEvent += OnLobbyLaunchStarted;
            
            _launchLobbyStream.Listen();
        }

        private void OnLobbyLaunchSuccess(LaunchDto launchDto)
        {
            SetInteractable(true);
            var connectionError = Topology.StartAsClient("127.0.0.1", launchDto.port, launchDto.secret);
            if (connectionError == null)
            {
                Log.Inf("Sync server", "Connected");
                return;
            }
            Log.Err("Sync server", $"Cannot connect to the server: {connectionError}");
        }

        private void OnLobbyLaunchError(string error)
        {
            SetInteractable(true);
        }

        private void OnLobbyLaunchStarted()
        {
            SetInteractable(false);
        }

        private void SetInteractable(bool interactable)
        {
            _btnLaunch.interactable = interactable;
            _btnLeave.interactable = interactable;
        }

        private async void OnError(string error)
        {
            Debug.LogError(error);
            await LeaveLobby();
        }

        private void InitActions()
        {
            _actionLeave = async () => { await LeaveLobby(); };
            _actionLaunch = LobbyLaunchService.LaunchLobby;
        }

        private async Task LeaveLobby()
        {
            _lobbyStream.Close();
            await LobbyService.LeaveLobby();
            KickSelf();
        }

        private void OnDisable()
        {
            _btnLaunch.onClick.RemoveListener(_actionLaunch);
            _btnLeave.onClick.RemoveListener(_actionLeave);
            _lobbyStream.PlayersRefreshedEvent -= PlayersRefresh;
            _lobbyStream.ErrorEvent -= OnError;
            _lobbyStream.Close();
            _lobbyStream = null;
            Lobby = null;
            _launchLobbyStream.SuccessEvent -= OnLobbyLaunchSuccess;
            _launchLobbyStream.ErrorEvent -= OnLobbyLaunchError;
            _launchLobbyStream.LaunchStartedEvent -= OnLobbyLaunchStarted;
            _launchLobbyStream.Dispose();
            _launchLobbyStream = null;
        }

        private void OnDestroy()
        {
            _lobbyStream.Close();
        }

        private void PlayersRefresh(PlayerDto[] playersDto)
        {
            foreach (Transform child in _scrollRect.content.transform)
            {
                Destroy(child.gameObject);
            }

            if (playersDto.Length == 0)
            {
                OnAdminCancelLobby();
                KickSelf();
            }
            else if (!IsThereOwnerUserId(playersDto))
            {
                OnKickedByAdmin();
                KickSelf();
            }

            foreach (var playerDto in playersDto)
            {
                var canBeKicked = _canKickOthers && playerDto.userId != AuthService.UserId;
                var playerItemGo = Instantiate(_playerItemPrefab, _scrollRect.content);
                InitPlayerItemGo(playerDto, playerItemGo.GetComponent<PlayerItem>(), canBeKicked);
            }
        }

        private void KickSelf()
        {
            gameObject.SetActive(false);
            LeaveEvent?.Invoke();
        }

        private static void OnAdminCancelLobby()
        {
            Debug.Log("Admin cancelled the lobby");
        }

        private static void OnKickedByAdmin()
        {
            Debug.Log("You were kicked");
        }

        private static bool IsThereOwnerUserId(IEnumerable<PlayerDto> playersDto)
        {
            return playersDto.Any(playerDto => playerDto.userId == AuthService.UserId);
        }

        private static void InitPlayerItemGo(PlayerDto playerDto, PlayerItem playerItem, bool canBeKicked)
        {
            playerItem.CanBeKicked = canBeKicked;
            playerItem.Init(playerDto);
        }
    }
}