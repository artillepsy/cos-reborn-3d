using System;
using Gameplay.Configs.Match.Skills;
using Gameplay.Shared.Util;
using Gameplay.Shared.Util.Camera;
using Gameplay.Shared.Util.SerializableDataStructure;
using UnityEngine;

namespace Gameplay.Shared.Skills.Fireball
{
/// <summary> Shoots a single projectile which flies in a straight direction </summary>
public class SkillFireball : SkillBase
{
	public override void OnKeyPressC()
	{
		if (!IsReady)
		{
			return;
		}
		var mousePos = CameraHelperC.GetMouseCursorWorldPos();
		var pos      = _tf.position;
		var dir      = (mousePos - pos).normalized;

		var dto = new ShotDto()
		{
			Dir           = new Vector3Serializable(dir),
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
		var dto = BinarySerializer.Deserialize<ShotDto>(bytesDto);
		var dir = dto.Dir.ToObject();
		var pos = _tf.position;

		var config = ((ConfigSkillFireball)Config);
		var netObj = GameObject.Instantiate(config.ProjectilePrefab);
		var proj   = netObj.GetComponent<ProjectileFireballS>();

		proj.transform.position = pos;
		proj.InitServer(dto.OwnerClientId, dir, config);
		netObj.Spawn();
	}

	[Serializable]
	public class ShotDto
	{
		public Vector3Serializable Dir;
		public ulong               OwnerClientId;

		public override string ToString()
		{
			return $"{nameof(Dir)}: {Dir}, "                     +
			       $"{nameof(OwnerClientId)}: {OwnerClientId}";
		}
	}
}
}