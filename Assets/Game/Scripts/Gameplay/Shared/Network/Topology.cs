using Shared.AppStartup.CommandLine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Gameplay.Shared.Network
{
    public static class Topology
    {
        public static void StartAsServer()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
            NetworkManager.Singleton.StartServer();
        }
        public static bool StartAsServer(ushort port)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
            var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
            System.Diagnostics.Debug.Assert(transport != null, nameof(transport) + " != null");
            transport.ConnectionData.Port = port;
            return NetworkManager.Singleton.StartServer();
        }

        public static void StartAsClient(string ipAddress, string port)
        {
            var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
            System.Diagnostics.Debug.Assert(transport != null, nameof(transport) + " != null");
            transport.SetConnectionData(ipAddress, ushort.Parse(port));
            NetworkManager.Singleton.NetworkConfig.ConnectionData =
                System.Text.Encoding.ASCII.GetBytes("Room password");
            NetworkManager.Singleton.OnClientDisconnectCallback += OnDeclineCallback;
            NetworkManager.Singleton.StartClient();
        }

        public static string StartAsClient(string ipAddress, ushort port, string secret)
        {
            var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
            System.Diagnostics.Debug.Assert(transport != null, nameof(transport) + " != null");
            transport.SetConnectionData(ipAddress, port);
            NetworkManager.Singleton.NetworkConfig.ConnectionData =
                System.Text.Encoding.ASCII.GetBytes(secret);
            NetworkManager.Singleton.OnClientDisconnectCallback += OnDeclineCallback;
            return NetworkManager.Singleton.StartClient() ? null : NetworkManager.Singleton.DisconnectReason;
        }
        private static void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response)
        {
            // TODO
            // The client identifier to be authenticated
            var clientId = request.ClientNetworkId;

            // Additional connection data defined by user code
            var connectionData = request.Payload;
            var secret = System.Text.Encoding.ASCII.GetString(connectionData);

            if (secret != CommandLineArgs.ArgsS?.Secret && !string.IsNullOrEmpty(CommandLineArgs.ArgsS?.Secret))
            {
                response.Approved = false;

                // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.Reason
                // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
                response.Reason = "Invalid secret key";
                return;
            }
            response.Approved = true;
            response.CreatePlayerObject = false;

            // The Prefab hash value of the NetworkPrefab, if null the default NetworkManager player Prefab is used
            response.PlayerPrefabHash = null;

            // Position to spawn the player object (if null it uses default of Vector3.zero)
            response.Position = Vector3.zero;

            // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
            response.Rotation = Quaternion.identity;
            response.Reason = "Some reason for not approving the client";

            // If additional approval steps are needed, set this to true until the additional steps are complete
            // once it transitions from true to false the connection approval response will be processed.
            response.Pending = false;
        }

        private static void OnDeclineCallback(ulong obj)
        {
            if (NetworkManager.Singleton.DisconnectReason != string.Empty)
            {
                Debug.Log($"Approval Declined Reason: {NetworkManager.Singleton.DisconnectReason}");
            }
        }
        
    }
}