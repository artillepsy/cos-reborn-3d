using Gameplay.Shared.Health;
using Shared.Logging;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay.Shared.Skills
{
/// <summary> The base class is needed to check collisions between 2 projectiles. </summary>
public abstract class ProjectileBaseS : NetworkBehaviour
{
	protected NetworkObject _netObj;
	protected Rigidbody     _rb;
	
	protected ulong? ProjOwnerId { get; private set; }

	//--------------------------------------------------------
	//--------------------------------------------------------

	/// <summary> Override this method carefully, it contains basic components initialization </summary>
	public override void OnNetworkSpawn()
	{
		if (IsServer)
		{
			_netObj = GetComponent<NetworkObject>();
			_rb     = GetComponent<Rigidbody>();
		}
	}
	
	/// <summary> sets <see cref="ownerClientId"/>>. If it's not set, there will be an error in Start() method; </summary>
	protected void SetProjOwnerId(ulong ownerClientId)
	{
		ProjOwnerId = ownerClientId;
	}

	protected virtual void Awake()
	{
		if (IsServer)
		{
			System.Diagnostics.Debug.Assert(ProjOwnerId != null, nameof(ProjOwnerId) + " != null");
		}
	}

	/// <summary>
	/// Is called when projectile hits something. Usually, it flies through other projectiles and kills the enemy player,
	/// destroying itself. Override the method to implement additional behaviour 
	/// </summary>
	protected virtual void OnTriggerEnter(Collider other)
	{
		if (!IsServer)
		{
			return;
		}
		if (other.TryGetComponent(out PlayerHealth health))
		{
			if (health.OwnerClientId == ProjOwnerId)
			{
				return;
			}
			Log.Inf(nameof(ProjectileBaseS), $"Hit player with id: {health.OwnerClientId}");
			health.KillServer(ProjOwnerId.Value);
			_netObj.Despawn();
		}
		else if (!other.isTrigger)
		{
			var pos = transform.position;
			Debug.DrawRay(pos + Vector3.down * 0.5f, Vector3.up, Color.yellow, 30f);
			Debug.DrawRay(pos + Vector3.left * 0.5f, Vector3.right, Color.yellow, 30f);
		
			_netObj.Despawn();
		}
	}
}
}