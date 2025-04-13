using System;
using System.Threading.Tasks;
using Foyer.Network.Dto.Lobby;
using Foyer.Network.Service.Lobby;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Foyer.Ui.Lobby
{
    public class LobbiesForm : MonoBehaviour
    {
        [SerializeField] private LobbyItem _lobbyItemPrefab;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Button _btnRefresh;
        [SerializeField] private Button _btnCreate;

        [SerializeField] private LobbyForm _lobbyForm;
        [SerializeField] private CreateLobbyForm _createLobbyForm;
        
        private UnityAction _actionOpenCreateForm;
        private UnityAction _actionRefresh;
        private LobbyItem.LobbyDelegate _actionJoinLobby;
        private void Awake()
        {
            InitActions();
            _btnCreate.onClick.AddListener(_actionOpenCreateForm);
            _btnRefresh.onClick.AddListener(_actionRefresh);
            LobbyItem.OnJoinClickedEvent += _actionJoinLobby;
            _lobbyForm.LeaveEvent += OnLobbyLeave;
            _createLobbyForm.CancelEvent += CancelCreateLobbyForm;
            _createLobbyForm.CreatedEvent += OnCreateLobby;
            
        }

        private void CancelCreateLobbyForm()
        {
            SwitchCreateLobbyScreen(false);
        }

        private void OnCreateLobby()
        {
            SwitchLobbyScreen(true);
        }

        private async void OnEnable()
        {
            await RefreshLobbies();
        }

        private void OnLobbyLeave()
        {
            gameObject.SetActive(true);
        }

        private void InitActions()
        {
            _actionOpenCreateForm = () =>
            {
                SwitchCreateLobbyScreen(true);
            };
            _actionRefresh = async () =>
            {
                _btnRefresh.interactable = false;
                try
                {
                    await RefreshLobbies();

                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                _btnRefresh.interactable = true;
            };
            _actionJoinLobby = async lobbyDto =>
            {
                await JoinLobby(lobbyDto);
            };
        }

        private async Task RefreshLobbies()
        {
            foreach (Transform child in _scrollRect.content.transform)
            {
                Destroy(child.gameObject);
            }

            var lobbies = await LobbyService.FetchLobbies();
            foreach (var lobby in lobbies)
            {
                var lobbyItemGo = Instantiate(_lobbyItemPrefab, _scrollRect.content);
                InitLobbyItemGo(lobbyItemGo.GetComponent<LobbyItem>(), lobby, this);
            }
        }

        private async Task JoinLobby(LobbyDto lobbyDto)
        {
            lobbyDto = await LobbyService.JoinLobby(lobbyDto);
            _lobbyForm.Lobby = lobbyDto;
            SwitchLobbyScreen(true);
        }

        private void SwitchLobbyScreen(bool isLobbyScreen)
        {
            gameObject.SetActive(!isLobbyScreen);
            _lobbyForm.gameObject.SetActive(isLobbyScreen);
        }

        private void SwitchCreateLobbyScreen(bool isCreateLobbyScreen)
        {
            gameObject.SetActive(!isCreateLobbyScreen);
            _createLobbyForm.gameObject.SetActive(isCreateLobbyScreen);
        }

        private static void InitLobbyItemGo(LobbyItem lobbyItem, LobbyDto lobbyDto, LobbiesForm lobbiesForm)
        {
            lobbyItem.SetLobbyDto(lobbyDto);
            lobbyItem.FormLobbies = lobbiesForm;
        }

        private void OnDestroy()
        {
            _btnCreate.onClick.RemoveListener(_actionOpenCreateForm);
            _btnRefresh.onClick.RemoveListener(_actionRefresh);
            LobbyItem.OnJoinClickedEvent -= _actionJoinLobby;
            _lobbyForm.LeaveEvent -= OnLobbyLeave;
            _createLobbyForm.CancelEvent -= CancelCreateLobbyForm;
            _createLobbyForm.CreatedEvent -= OnCreateLobby;
        }
    }
}