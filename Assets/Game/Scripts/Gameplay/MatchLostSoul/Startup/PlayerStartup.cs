using Game.Scripts.Gameplay.Shared.Init;
using Unity.Netcode;

namespace Game.Scripts.Gameplay.MatchLostSoul.Startup
{
public class PlayerStartup : NetworkBehaviour
{
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