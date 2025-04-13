using Foyer.Network.Dto.Lobby;
using Foyer.Network.Service.Lobby;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Foyer.Ui.Lobby
{
    public class LobbyItem : MonoBehaviour
    {
        public delegate void LobbyDelegate(LobbyDto lobbyDto);
        public static event LobbyDelegate OnJoinClickedEvent;
        
        [SerializeField] private TextMeshProUGUI _lobbyNameText;
        [SerializeField] private Button _btnJoin;
        [SerializeField] private TextMeshProUGUI _matchType;
        [SerializeField] private TextMeshProUGUI _playersCount;
        private LobbiesForm _lobbiesForm;

        public LobbiesForm FormLobbies
        {
            get => _lobbiesForm;
            set => _lobbiesForm = value;
        }


        private LobbyDto _lobbyDto;

        public async void SetLobbyDto(LobbyDto lobbyDto)
        {
            _lobbyDto = lobbyDto;
            _lobbyNameText.text = _lobbyDto.name;
            _matchType.text = (await LobbyService.GetMatchTypes())[_lobbyDto.matchTypeIdx].name;
            _btnJoin.onClick.AddListener(OnJoinClicked);
        }

        private void OnJoinClicked()
        {
            OnJoinClickedEvent?.Invoke(_lobbyDto);
        }

        private void OnDestroy()
        {
            _btnJoin.onClick.RemoveListener(OnJoinClicked);
        }
    }
}