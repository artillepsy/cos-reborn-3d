using System;
using Game.Scripts.Gameplay.Configs.Match;
using Game.Scripts.Gameplay.MatchLostSoul.Context;
using Game.Scripts.Gameplay.Shared.Init;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace Game.Scripts.Gameplay.Shared.Movement
{
public class PlayerMovement : NetworkBehaviour, IMatchInitServer, IMatchInitClient
{
	private Rigidbody _rb;

	private NetworkTransform _netTransformC;
	private ConfigInput      _configInputC;
	
	private float _speed;
	//--------------------------------------------------------
	//--------------------------------------------------------
	private Vector3 _moveDelta;

	public override void OnNetworkSpawn()
	{
		System.Diagnostics.Debug.Assert(UnityEngine.Camera.main != null, "Camera.main != null");
		_rb    = GetComponent<Rigidbody>();
	}

	public void InitServer(MatchContext context)
	{
		_speed = context.Configs.Player.Speed;
	}

	public void InitClient(MatchContext context)
	{
		_speed         = context.Configs.Player.Speed;
		_configInputC  = context.Configs.Input;
		_netTransformC = GetComponent<NetworkTransform>();
	}

	//--------------------------------------------------------
	//--------------------------------------------------------

	//todo: if movement is driven by local player, disable it completely when they're dead 
	//todo: change camera bahaviour, move it in a different class
	private void Update()
	{
		if (!IsLocalPlayer)
		{
			return;
		}

		_moveDelta = GetMoveInputDeltaС();
	}

	private void FixedUpdate()
	{
		if (!IsLocalPlayer)
		{
			return;
		}
		
		if (IsOwner && _netTransformC.AuthorityMode == NetworkTransform.AuthorityModes.Owner)
		{
			var newVelocity = _moveDelta;
			newVelocity        *= _speed;
			newVelocity        =  Vector3.ClampMagnitude(newVelocity, _speed);
			_rb.linearVelocity =  newVelocity;
		}
		else
		{
			UpdateServerRpc(_moveDelta);
		}
	}

	private Vector3 GetMoveInputDeltaС()
	{
		float x = 0f;
		float z = 0f;
		
		if (Input.GetKey(_configInputC.Keys.MoveLeft))
		{
			x -= 1f;
		}
		if (Input.GetKey(_configInputC.Keys.MoveRight))
		{
			x += 1f;
		}
		if (Input.GetKey(_configInputC.Keys.MoveUp))
		{
			z += 1f;
		}
		if (Input.GetKey(_configInputC.Keys.MoveDown))
		{
			z -= 1f;
		}

		/*float x = Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");*/
		return new Vector3(x, 0f, z);
	}

	[Rpc(SendTo.Server)]
	private void UpdateServerRpc(Vector3 newVelocity)
	{
		newVelocity        *= _speed;
		newVelocity        =  Vector3.ClampMagnitude(newVelocity, _speed);
		_rb.linearVelocity =  newVelocity;
	}
}
}