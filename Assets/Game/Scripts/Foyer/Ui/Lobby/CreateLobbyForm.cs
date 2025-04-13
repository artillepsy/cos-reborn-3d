using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foyer.Network.Dto.Lobby;
using Foyer.Network.Service.Lobby;
using Shared.Logging;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Foyer.Ui.Lobby
{
    public class CreateLobbyForm : MonoBehaviour
    {
        private const string Tag = "Create lobby";
        public event Action CancelEvent;
        public event Action CreatedEvent;
        [SerializeField] private Button _btnCreateLobbyCreate;
        [SerializeField] private Button _btnCreateLobbyCancel;
        [SerializeField] private TMP_InputField _createLobbyNameInputField;
        [SerializeField] private TMP_InputField _createLobbyMinPlayersInputField;
        [SerializeField] private TMP_InputField _createLobbyMaxPlayersInputField;
        [SerializeField] private TMP_Dropdown _matchTypeDropdown;
        [SerializeField] private Toggle _isPrivateLobbyToggle;
        [SerializeField] private LobbyForm _lobbyForm;

        private static MatchTypeDto[] matchTypes;

        private async void Awake()
        {
            await InitMatchTypes();
            _btnCreateLobbyCreate.onClick.AddListener(OnCreate);
            _btnCreateLobbyCancel.onClick.AddListener(OnCancel);
            _matchTypeDropdown.onValueChanged.AddListener(OnMatchTypeChanged);
        }

        private async Task InitMatchTypes()
        {
            matchTypes = await LobbyService.GetMatchTypes();
            var options = matchTypes.Select(matchTypeDto => 
                new TMP_Dropdown.OptionData {text = matchTypeDto.name}).ToList();
            _matchTypeDropdown.AddOptions(options);
            OnMatchTypeChanged(0);
        }

        private void OnMatchTypeChanged(int index)
        {
            var currentMatchType = matchTypes[index];
            _createLobbyMinPlayersInputField.text = currentMatchType.minPlayersAllowed.ToString();
            _createLobbyMaxPlayersInputField.text = currentMatchType.maxPlayersAllowed.ToString();
        }

        private async void OnCreate()
        {
            _btnCreateLobbyCreate.interactable = false;
            try
            {
                var currentMatchType = matchTypes[_matchTypeDropdown.value];
                var minPlayers = int.Parse(_createLobbyMinPlayersInputField.text);
                var maxPlayers = int.Parse(_createLobbyMaxPlayersInputField.text);
                await CreateLobby(
                    _createLobbyNameInputField.text, 
                    _isPrivateLobbyToggle.isOn, 
                    minPlayers, 
                    maxPlayers, 
                    currentMatchType.id);
                gameObject.SetActive(false);
                CreatedEvent?.Invoke();
            }
            catch (Exception e)
            {
                Log.Err(Tag, e.Message);
            }

            _btnCreateLobbyCreate.interactable = true;
        }

        private void OnCancel()
        {
            CancelEvent?.Invoke();
        }

        private async Task CreateLobby(
            string lobbyName, 
            bool isPrivate, 
            int minPlayers, 
            int maxPlayers, 
            int matchTypeIdx)
        {
            var lobby = new LobbyDto
            {
                name = lobbyName, 
                isPrivate = isPrivate,
                minPlayers = minPlayers,
                maxPlayers = maxPlayers,
                matchTypeIdx = matchTypeIdx
            };
            lobby = await LobbyService.CreateLobby(lobby);
            _lobbyForm.Lobby = lobby;
        }

        private void OnDestroy()
        {
            _btnCreateLobbyCreate.onClick.RemoveListener(OnCreate);
            _btnCreateLobbyCancel.onClick.RemoveListener(OnCancel);
            _matchTypeDropdown.onValueChanged.RemoveListener(OnMatchTypeChanged);
        }
    }
}