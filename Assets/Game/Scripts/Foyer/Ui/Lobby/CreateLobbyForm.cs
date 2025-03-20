using System;
using System.Threading.Tasks;
using Game.Scripts.Foyer.Network.Dto.Lobby;
using Game.Scripts.Foyer.Network.Service.Lobby;
using Game.Scripts.Gameplay.Shared.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Scripts.Foyer.Ui.Lobby
{
    public class CreateLobbyForm : MonoBehaviour
    {
        public event Delegates.VoidDelegate CancelEvent;
        public event Delegates.VoidDelegate CreatedEvent;
        [SerializeField] private Button _btnCreateLobbyCreate;
        [SerializeField] private Button _btnCreateLobbyCancel;
        [SerializeField] private TMP_InputField _createLobbyNameInputField;
        [SerializeField] private TMP_InputField _createLobbyMinPlayersInputField;
        [SerializeField] private TMP_InputField _createLobbyMaxPlayersInputField;
        [SerializeField] private Toggle _isPrivateLobbyToggle;
        [SerializeField] private LobbyForm _lobbyForm;

        private UnityAction _actionCreate;
        private UnityAction _actionCancel;

        private void Awake()
        {
            InitActions();
            _btnCreateLobbyCreate.onClick.AddListener(_actionCreate);
            _btnCreateLobbyCancel.onClick.AddListener(_actionCancel);
        }

        private void InitActions()
        {
            _actionCreate = async () =>
            {
                _btnCreateLobbyCreate.interactable = false;
                try
                {
                    await CreateLobby();
                    gameObject.SetActive(false);
                    CreatedEvent?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                _btnCreateLobbyCreate.interactable = true;
            };
            _actionCancel = () => { CancelEvent?.Invoke(); };
        }

        private async Task CreateLobby()
        {
            var lobby = new LobbyDto
            {
                name = _createLobbyNameInputField.text, 
                isPrivate = _isPrivateLobbyToggle.isOn,
                minPlayers = int.Parse(_createLobbyMinPlayersInputField.text),
                maxPlayers = int.Parse(_createLobbyMaxPlayersInputField.text)
            };
            lobby = await LobbyService.CreateLobby(lobby);
            _lobbyForm.Lobby = lobby;
        }

        private void OnDestroy()
        {
            _btnCreateLobbyCreate.onClick.RemoveListener(_actionCreate);
            _btnCreateLobbyCancel.onClick.RemoveListener(_actionCancel);
        }
    }
}