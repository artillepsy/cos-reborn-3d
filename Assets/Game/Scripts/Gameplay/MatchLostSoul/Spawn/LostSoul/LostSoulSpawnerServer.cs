using System.Collections;
using Gameplay.Configs.Match;
using Gameplay.MatchLostSoul.Context;
using Gameplay.Shared.Init;
using Gameplay.Shared.Spawn;
using UnityEngine;

namespace Gameplay.MatchLostSoul.Spawn.LostSoul
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