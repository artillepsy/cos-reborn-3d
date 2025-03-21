using Game.Scripts.Foyer.Network.Dto.Lobby;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Foyer.Ui.Lobby
{
    public class LobbyItem : MonoBehaviour
    {
        public delegate void LobbyDelegate(LobbyDto lobbyDto);
        public static event LobbyDelegate OnJoinClickedEvent;
        
        [SerializeField] private TextMeshProUGUI _lobbyNameText;
        [SerializeField] private Button _btnJoin;
        private LobbiesForm _lobbiesForm;

        public LobbiesForm FormLobbies
        {
            get => _lobbiesForm;
            set => _lobbiesForm = value;
        }


        private LobbyDto _lobbyDto;
        public LobbyDto LobbyDto
        {
            set
            {
                _lobbyDto = value;
                _lobbyNameText.text = _lobbyDto.name;
                _btnJoin.onClick.AddListener(OnJoinClicked);
            }
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