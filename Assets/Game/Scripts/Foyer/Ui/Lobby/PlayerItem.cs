using Game.Scripts.Foyer.Network.Dto.Lobby;
using Game.Scripts.Foyer.Network.Service.Lobby;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Button = UnityEngine.UI.Button;

namespace Game.Scripts.Foyer.Ui.Lobby
{
    public class PlayerItem : MonoBehaviour
    {
        [SerializeField] private Button _btnKick;
        [SerializeField] private TextMeshProUGUI _playerIdText;
        [SerializeField] private TextMeshProUGUI _playerNameText;

        private UnityAction _actionKick;

        private PlayerDto _playerDto;

        private bool _canBeKicked;
        
        public bool CanBeKicked
        {
            get => _canBeKicked;
            set
            {
                _canBeKicked = value;
                _btnKick.gameObject.SetActive(_canBeKicked);
            }
        }

        private void Awake()
        {
            InitActions();
            _btnKick.onClick.AddListener(_actionKick);
        }

        private void InitActions()
        {
            _actionKick = async () =>
            {
                await LobbyService.Kick(_playerDto);
            };
        }

        public void Init(PlayerDto playerDto)
        {
            _playerDto = playerDto;
            _playerIdText.text = _playerDto.userId.ToString();
            _playerNameText.text = _playerDto.playerName;
        }

        private void OnDestroy()
        {
            _btnKick.onClick.RemoveListener(_actionKick);
        }
    }
}