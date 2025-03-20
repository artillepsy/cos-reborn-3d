using System;
using Game.Scripts.Gameplay.Configs.Match.Skills;
using Game.Scripts.Gameplay.Shared.Util;
using Game.Scripts.Gameplay.Shared.Util.SerializableDataStructure;
using UnityEngine;

namespace Game.Scripts.Gameplay.Shared.Skills.Lightning
{
/// <summary>
/// Shoots one projectile, which flies in main direction and in side direction additionally,
/// changing from right to left and vise-versa.
/// </summary>
public class SkillLightning : SkillBase
{
	public override void OnKeyPressC()
	{
		if (!IsReady)
		{
			return;
		}
		var mousePos = GetMouseCursorPosC();
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

		var config     = ((ConfigSkillLightning)Config);
		var netObj     = GameObject.Instantiate(config.ProjectilePrefab);
		var projectile = netObj.GetComponent<ProjectileLightingS>();

		projectile.transform.position = pos;
		projectile.Init(dto.OwnerClientId, dir, config);
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