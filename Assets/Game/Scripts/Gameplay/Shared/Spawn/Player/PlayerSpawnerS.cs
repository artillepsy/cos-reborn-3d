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
            MatchEventsS.EvPlayerDied += RespawnPlayer;
        }

        public override void OnNetworkDespawn()
        {
            MatchEventsS.EvPlayerDied -= RespawnPlayer;
        }

        public override void Spawn()
        {
            foreach (var clientId in NetworkManager.ConnectedClients.Keys)
            {
                _spawnObject.ClientIdS                  = clientId;
                _context.PlayersDataS[clientId].IsAlive = true;
                _spawnPointResolver.ResolveSpawnPoint().SpawnObject(_spawnObject);
            }
        }

        public void InitServer(MatchContext context)
        {
            _context = context;
            _matchProfile = context.Configs.MatchProfile;
        }

        private void RespawnPlayer(ulong playerId, ulong? _)
        {
            StartCoroutine(RespawnPlayerCoServer(playerId));
        }

        private IEnumerator RespawnPlayerCoServer(ulong playerId)
        {
            yield return new WaitForSeconds(_matchProfile.PlayerRespawnCooldown / 2);
            var spawnPoint = _spawnPointResolver.ResolveSpawnPoint();
            var netPlayerObj = _netManager.ConnectedClients[playerId].PlayerObject;
            netPlayerObj.transform.position = spawnPoint.transform.position;
		
            yield return new WaitForSeconds(_matchProfile.PlayerRespawnCooldown / 2);
		
            _context.PlayersDataS[playerId].IsAlive = true;

            netPlayerObj.GetComponent<LocalComponentsActivator>().SetActive(true);
		
            MatchEventsS.SendEvPlayerSpawned(playerId);
            ActivatePlayerClientRpc(playerId);
        }

        [Rpc(SendTo.NotServer)]
        private void ActivatePlayerClientRpc(ulong ownerClientId)
        {
            var netPlayerObj = _netManager.ConnectedClients[ownerClientId].PlayerObject;
            netPlayerObj.GetComponent<LocalComponentsActivator>().SetActive(true);
        }
    }
}