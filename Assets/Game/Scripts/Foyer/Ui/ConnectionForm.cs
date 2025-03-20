using Game.Scripts.Gameplay.Shared.Network;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

namespace Game.Scripts.Foyer.Ui
{
    public class ConnectionForm : MonoBehaviour
    {
        [Header("Connection data")]
        [SerializeField]
        private GameObject _connectionGroup;
        [SerializeField]
        private Button _clientBtn;
        [SerializeField]
        private Button _serverBtn;
        [SerializeField]
        private TMP_InputField _ipInputField;
        [SerializeField]
        private TMP_InputField _portInputField;

        [Header("Info")]
        [SerializeField]
        private GameObject _infoGroup;
        [SerializeField]
        private TextMeshProUGUI _transportText;
        [SerializeField]
        private TextMeshProUGUI _modeText;
        
        //--------------------------------------------------------
        //--------------------------------------------------------
        private NetworkManager _networkManager;

        private UnityAction _actionStartAsClient;
        private UnityAction _actionStartAsServer;

        private void Awake()
        {
            InitActions();
        }

        private void InitActions()
        {
            _actionStartAsClient = () =>
            {
                Topology.StartAsClient(_ipInputField.text, _portInputField.text);
            };
            _actionStartAsServer = () =>
            {
                Topology.StartAsServer(ushort.Parse(_portInputField.text));
            };
        }

        private void Start()
        {
            _networkManager = NetworkManager.Singleton;

            _networkManager.OnServerStarted += SetStatusActive;
            _networkManager.OnClientStarted += SetStatusActive;

            _serverBtn.onClick.AddListener(_actionStartAsServer);
            _clientBtn.onClick.AddListener(_actionStartAsClient);

            var transport = _networkManager.NetworkConfig.NetworkTransport as UnityTransport;

            Debug.Assert(transport != null, nameof(transport) + " != null");
            _ipInputField.text   = transport.ConnectionData.Address;
            _portInputField.text = transport.ConnectionData.Port.ToString();
            
            _connectionGroup.SetActive(true);
            _infoGroup.SetActive(false);
        }

        private void SetStatusActive()
        {
            _connectionGroup.SetActive(false);
            _infoGroup.SetActive(true);
            
            var mode = _networkManager.IsServer ? "Server" : "Client";

            _transportText.text = $"Transport: {_networkManager.NetworkConfig.NetworkTransport.GetType().Name}";
            _modeText.text      = $"Mode: {mode}";
        }

        private void OnDestroy()
        {
            _serverBtn.onClick.RemoveListener(_actionStartAsServer);
            _clientBtn.onClick.RemoveListener(_actionStartAsClient);

            _networkManager.OnServerStarted -= SetStatusActive;
            _networkManager.OnClientStarted -= SetStatusActive;
        }
    }
}