using System;
using System.Linq;
using Gameplay.Configs.Match.Skills;
using Gameplay.Matches.LostSoul.Context;
using Gameplay.Shared.Util;
using Gameplay.Shared.Util.Camera;
using Gameplay.Shared.Util.SerializableDataStructure;
using UnityEngine;

namespace Gameplay.Shared.Skills.Teleport
{
/// <summary> Teleports player to a desired location, restricted by colliders and max distance </summary>
public class SkillTeleport : SkillBase
{
	private CapsuleCollider _playerColliderS;
	private Rigidbody       _rb;

	public override void InitClient(MatchContext context, ConfigSkill config, PlayerSkillsManager manager, int skillIndex)
	{
		base.InitClient(context, config, manager, skillIndex);
		_rb = _tf.GetComponent<Rigidbody>();
	}

	public override void InitServer(MatchContext context, ConfigSkill config, PlayerSkillsManager manager, int skillIndex)
	{
		base.InitServer(context, config, manager, skillIndex);
		_playerColliderS = _tf.GetComponentsInChildren<CapsuleCollider>()
		   .ToList().Find(c => !c.isTrigger);

		_rb = _tf.GetComponent<Rigidbody>();
	}

	public override void OnKeyPressC()
	{
		if (!IsReady)
		{
			return;
		}
		var mousePos = CameraHelperC.GetMouseCursorWorldPos();
		
		_rb.MovePosition(mousePos);
		
		var dto = new TeleportDto()
		{
			DesiredPos    = new Vector3Serializable(mousePos),
			OwnerClientId = _manager.OwnerClientId,
		};
		var bytes = BinarySerializer.Serialize(dto);
		
		_manager.ReceiveSkillDtoC(SkillIndex, bytes);
		StartCooldown();
	}
	
	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------
	
	public override void OnDtoReceivedServer(byte[] bytesDto)
	{
		StartCooldown();
		var dto        = BinarySerializer.Deserialize<TeleportDto>(bytesDto);
		var pos        = _tf.position;
		var desiredPos = dto.DesiredPos.ToObject();

		var config = ((ConfigSkillTeleport)Config);

		var dist        = Mathf.Min(Vector3.Distance(pos, desiredPos), config.MaxDist);
		var dir         = (desiredPos - pos).normalized;
		var teleportPos = pos + dir * dist;

		var results  = new RaycastHit[10];
		int hitCount = Physics.RaycastNonAlloc(pos, teleportPos, results, dist);
		
		if (hitCount > 0)
		{
			foreach (var hit in results)
			{
				if (!hit.collider.isTrigger && hit.collider != _playerColliderS)
				{
					dist = hit.distance - _playerColliderS.radius;
					break;
				}
			}
		}
		
		teleportPos   = pos + dir * dist;
		_rb.position = teleportPos; //rb's owner is player, so it won't work on the server side nor rpc methods would work (it's not networkBeh)
		Debug.DrawLine(pos + Vector3.up, teleportPos + Vector3.up, Color.green, 30f);
	}


	[Serializable]
	public class TeleportDto
	{
		public Vector3Serializable DesiredPos;
		public ulong               OwnerClientId;

		public override string ToString()
		{
			return $"{nameof(DesiredPos)}: {DesiredPos}, " +
			       $"{nameof(OwnerClientId)}: {OwnerClientId}";
		}
	}
}
}