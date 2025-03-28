using System.Collections;
using Gameplay.Configs.Match;
using Gameplay.Matches.LostSoul;
using Gameplay.Matches.LostSoul.Context;
using Gameplay.Shared.Components;
using Gameplay.Shared.Init;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay.Shared.Spawn.Player
{
    public class PlayerSpawnerS : SpawnerS<PlayerSpawnableObject>, IMatchInitServer
    {
        private MatchContext _context;
        private NetworkManager _netManager;
        private ConfigMatchProfile _matchProfile;
        

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _netManager = NetworkManager.Singleton;
            if (!IsServer)
            {
                return;
            }
            MatchEventsS.EvPlayerDied += RespawnPlayerServer;
        }

        public override void Spawn()
        {
            foreach (var clientId in NetworkManager.ConnectedClients.Keys)
            {
                _spawnObject.ClientId = clientId;
                _spawnPointResolver.ResolveSpawnPoint().SpawnObject(_spawnObject);
            }
        }

        public void InitServer(MatchContext context)
        {
            _context = context;
            _matchProfile = context.Configs.MatchProfile;
        }

        private void RespawnPlayerServer(ulong ownerClientId)
        {
            StartCoroutine(RespawnPlayerCoServer(ownerClientId));
        }

        private IEnumerator RespawnPlayerCoServer(ulong ownerClientId)
        {
            yield return new WaitForSeconds(_matchProfile.PlayerRespawnCooldown / 2);
            var spawnPoint = _spawnPointResolver.ResolveSpawnPoint();
            var netPlayerObj = _netManager.ConnectedClients[ownerClientId].PlayerObject;
            netPlayerObj.transform.position = spawnPoint.transform.position;
		
            yield return new WaitForSeconds(_matchProfile.PlayerRespawnCooldown / 2);
		
            netPlayerObj.GetComponent<LocalComponentsActivator>().SetActive(true);
		
            MatchEventsS.SendEvPlayerSpawned(ownerClientId);
            ActivatePlayerClientRpc(ownerClientId);
        }

        public override void OnNetworkDespawn()
        {
            MatchEventsS.EvPlayerDied -= RespawnPlayerServer;
        }

        [Rpc(SendTo.NotServer)]
        private void ActivatePlayerClientRpc(ulong ownerClientId)
        {
            var netPlayerObj = _netManager.ConnectedClients[ownerClientId].PlayerObject;
            netPlayerObj.GetComponent<LocalComponentsActivator>().SetActive(true);
        }
    }
}