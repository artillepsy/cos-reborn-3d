using System.Linq;
using Gameplay.MatchLostSoul.Ui.Shared;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay.MatchLostSoul.Startup
{
public class UiStartup : NetworkBehaviour
{
	public override void OnNetworkSpawn()
	{
		MatchEventsS.EvMatchStarted += InitS;
	}

	public override void OnNetworkDespawn()
	{
		MatchEventsS.EvMatchStarted -= InitS;
	}
	
	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------

	private void InitS()
	{
		InitClientRpc();
	}

	[Rpc(SendTo.NotServer)]
	private void InitClientRpc()
	{
		var context = FindFirstObjectByType<MatchStartup>().Context;
		
		FindObjectsByType<UiInitCompBaseC>(FindObjectsInactive.Include, FindObjectsSortMode.None)
		   .ToList().ForEach(comp => comp.InitClient(context));
	}
}
}