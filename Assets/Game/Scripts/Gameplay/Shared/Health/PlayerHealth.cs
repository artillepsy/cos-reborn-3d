using Gameplay.Matches.LostSoul;
using Gameplay.Shared.Components;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay.Shared.Health
{
// TODO add health calculation
public class PlayerHealth : NetworkBehaviour
{
	private NetworkObject            _netObj;
	private LocalComponentsActivator _componentsActivator;
	
	//todo: add init
	/*public void Init(MatchContext context)
	{
		//todo: add health values
	}*/

	public override void OnNetworkSpawn()
	{
		_netObj              = GetComponent<NetworkObject>();
		_componentsActivator = GetComponent<LocalComponentsActivator>();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.K))
		{
			KillSelfServerRpc(OwnerClientId);
		}
	}

	public void KillServer(ulong killerId)
	{
		_componentsActivator.SetActive(false);
		MatchEventsS.SendEvPlayerDied(OwnerClientId, killerId);
		
		KillPlayerClientRpc(OwnerClientId);
	}

	[Rpc(SendTo.NotServer)]
	private void KillPlayerClientRpc(ulong ownerClientId)
	{
		if (_netObj.OwnerClientId == ownerClientId)
		{
			_componentsActivator.SetActive(false);
		}
	}

	[Rpc(SendTo.Server)]
	private void KillSelfServerRpc(ulong playerId)
	{
		if (OwnerClientId == playerId)
		{
			KillServer(playerId);
		}
	}
}
}