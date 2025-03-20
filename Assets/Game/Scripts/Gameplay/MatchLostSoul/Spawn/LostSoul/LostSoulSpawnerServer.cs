using System.Collections;
using Game.Scripts.Gameplay.Configs.Match;
using Game.Scripts.Gameplay.MatchLostSoul.Context;
using Game.Scripts.Gameplay.Shared.Init;
using Game.Scripts.Gameplay.Shared.Spawn;
using UnityEngine;

namespace Game.Scripts.Gameplay.MatchLostSoul.Spawn.LostSoul
{
    public class LostSoulSpawnerServer : SpawnerServer<LostSoulSpawnableObject>, IMatchInitServer
    {
        private ConfigMatchProfile _matchProfile;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer)
            {
                return;
            }

            MatchEventsS.EvLostSoulAbsorbed += RespawnLostSoul;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer)
            {
                return;
            }
            MatchEventsS.EvLostSoulAbsorbed -= RespawnLostSoul;
        }

        private void RespawnLostSoul(ulong ownerId)
        {
            StartCoroutine(RespawnLostSoulCo());
        }

        private IEnumerator RespawnLostSoulCo()
        {
            yield return new WaitForSeconds(_matchProfile.LostSoulRespawnCooldown);
            Spawn();
        }

        public void InitServer(MatchContext context)
        {
            _matchProfile = context.Configs.MatchProfile;
        }
    }
}