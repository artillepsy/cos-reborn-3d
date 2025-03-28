using System;
using Gameplay.Configs.Match.Skills;
using Gameplay.Shared.Util;
using Gameplay.Shared.Util.Camera;
using Gameplay.Shared.Util.SerializableDataStructure;
using UnityEngine;

namespace Gameplay.Shared.Skills.Flower
{
/// <summary>
/// Shoots firstly one main projectile, which creates a few more child ones.
/// At the end of lifetime all the projectiles slightly change direction, creating a curve resembling a flower
/// </summary>
public class SkillFlower : SkillBase
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

		var config = ((ConfigSkillFlower)Config);
		var netObj = GameObject.Instantiate(config.Main.ProjectilePrefab);
		var proj   = netObj.GetComponent<ProjectileFlowerMainS>();

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