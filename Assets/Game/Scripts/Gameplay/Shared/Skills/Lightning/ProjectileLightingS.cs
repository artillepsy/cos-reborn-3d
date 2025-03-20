using Game.Scripts.Gameplay.Configs.Match.Skills;
using UnityEngine;

namespace Game.Scripts.Gameplay.Shared.Skills.Lightning
{
public class ProjectileLightingS : ProjectileBaseS
{
	private ConfigSkillLightning _configSkill;

	private float   _nextTimeToChangeDir;
	private float   _currInterval;
	private bool    _isDirRight;
	private Vector3 _dir;

	public void Init(ulong ownerClientId, Vector3 dir, ConfigSkillLightning configSkill)
	{
		SetProjOwnerId(ownerClientId);
		_dir           = dir;
		_configSkill   = configSkill;

		_isDirRight          = Random.value > 0.5f;
		_currInterval        = configSkill.GetRandDirChangeTimeStart();
		_nextTimeToChangeDir = Time.time + _currInterval;
	}
	
	private void Update()
	{
		if (!IsServer)
		{
			return;
		}
		HandleSideDirChange();
		HandleMove();
	}

	private void HandleSideDirChange()
	{
		if (Time.time < _nextTimeToChangeDir)
		{
			return;
		}
		_isDirRight          =  !_isDirRight;
		_currInterval        += _configSkill.GetRandDirChangeTimeAdd();
		_nextTimeToChangeDir =  Time.time + _currInterval;
	}

	private void HandleMove()
	{
		var forwardVelocity    = _dir.normalized * _configSkill.SpeedForward;
		var rotationQuaternion = Quaternion.Euler(0f, _isDirRight ? 90f : -90f, 0f);
		var sideDir            = rotationQuaternion * _dir.normalized;
		var sideVelocity       = sideDir            * _configSkill.SpeedSide;

		_rb.linearVelocity = forwardVelocity + sideVelocity;
	}
}
}