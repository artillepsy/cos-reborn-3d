using Game.Scripts.Gameplay.Configs.Match.Skills;
using UnityEngine;

namespace Game.Scripts.Gameplay.Shared.Skills.Fireball
{
public class ProjectileFireballS : ProjectileBaseS
{
	private float   _speed;
	private Vector3 _dir;

	public void InitServer(ulong ownerClientId, Vector3 dir, ConfigSkillFireball configSkill)
	{
		SetProjOwnerId(ownerClientId);
		_speed = configSkill.Speed;
		_dir   = dir;
	}

	private void Update()
	{
		if (IsServer)
		{
			_rb.linearVelocity = _dir.normalized * _speed;
		}
	}
}
}