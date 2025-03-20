using System;
using Game.Scripts.Gameplay.Configs.Match.Skills;
using Game.Scripts.Gameplay.Shared.Util;
using Game.Scripts.Gameplay.Shared.Util.SerializableDataStructure;
using UnityEngine;

namespace Game.Scripts.Gameplay.Shared.Skills.Wave
{
/// <summary> Shoots 2 projectile, which fly like a Sin wave in one direction. </summary>
public class SkillWave : SkillBase
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

		var config = ((ConfigSkillWave)Config);

		var dto = BinarySerializer.Deserialize<ShotDto>(bytesDto);
		var dir = dto.Dir.ToObject();
		var pos = _tf.position;
		
		float speedSide          = config.GetRandSpeedSide();
		float dirChangeDelay     = config.GetRandDirChangeDelay();
		float dirChangeDelayGain = config.GetRandDirDelayGain();

		SpawnProjectile(dto.OwnerClientId, dir, pos, speedSide, dirChangeDelay, dirChangeDelayGain, true);
		SpawnProjectile(dto.OwnerClientId, dir, pos, speedSide, dirChangeDelay, dirChangeDelayGain, false);
	}

	private void SpawnProjectile(ulong ownerClientId, Vector3 dir, Vector3 spawnPos,
		float speedSide, float dirChangeDelay, float dirChangeDelayGain, bool isRight)
	{
		var config = ((ConfigSkillWave)Config);
		var netObj     = GameObject.Instantiate(config.ProjectilePrefab);
		var proj = netObj.GetComponent<ProjectileWaveS>();
		
		proj.transform.position = spawnPos;
		proj.Init(ownerClientId, dir, config, speedSide, dirChangeDelay, dirChangeDelayGain, isRight);
		netObj.Spawn();
	}

	[Serializable]
	public class ShotDto
	{
		public Vector3Serializable Dir;
		public ulong               OwnerClientId;

		public override string ToString()
		{
			return $"{nameof(Dir)}: {Dir}, " +
			       $"{nameof(OwnerClientId)}: {OwnerClientId}";
		}
	}
}
}