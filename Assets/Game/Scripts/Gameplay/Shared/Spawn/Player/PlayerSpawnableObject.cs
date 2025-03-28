using Gameplay.Matches.LostSoul.Startup;
using Gameplay.Shared.Init;

namespace Gameplay.Shared.Spawn.Player
{
    public class PlayerSpawnableObject : SpawnableObject
    {
        public ulong ClientIdS { get; set; }

        public override void OnNetworkSpawn()
        {
            var context = FindAnyObjectByType<MatchStartup>().Context;

            if (IsServer)
            {
                foreach (var comp in GetComponentsInChildren<IMatchInitServer>())
                {
                    comp.InitServer(context);
                }
            }
            if (IsOwner)
            {
                foreach (var comp in GetComponentsInChildren<IMatchInitClient>())
                {
                    comp.InitClient(context);
                }
            }
        }
    }
}