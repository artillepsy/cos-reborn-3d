using Game.Scripts.Gameplay.Configs.Match.Skills;
using UnityEngine;

namespace Game.Scripts.Gameplay.Shared.Skills.Wave
{
public class ProjectileWaveS : ProjectileBaseS
{
	private ConfigSkillWave _configSkill;

	private float _dirChangeDelayGain;
	private float _speedSide;

	private float _currDirChangeDelay;
	private float _currDirTime;
	private bool  _isDirRight;

	private Vector3 _dir;

	public void Init(ulong ownerClientId, Vector3 dir, ConfigSkillWave configSkill,
		float sideSpeed, float dirChangeDelay, float dirChangeDelayGain, bool isRight)
	{
		SetProjOwnerId(ownerClientId);
		_dir         = dir;
		_configSkill = configSkill;
		_isDirRight  = isRight;

		_speedSide          = sideSpeed;
		_currDirChangeDelay = dirChangeDelay;
		_dirChangeDelayGain = dirChangeDelayGain;

		_currDirTime = 0f;
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
		_currDirTime += Time.deltaTime;

		if (_currDirTime < _currDirChangeDelay)
		{
			return;
		}

		_isDirRight         =  !_isDirRight;
		_currDirChangeDelay += _dirChangeDelayGain;
		_currDirTime  =  0f;
	}

	private void HandleMove()
	{
		var forwardVelocity    = _dir.normalized * _configSkill.SpeedForward;
		var rotationQuaternion = Quaternion.Euler(0f, _isDirRight ? 90f : -90f, 0f);
		var sideDir            = rotationQuaternion * _dir.normalized;

		var speedLerpVal = _configSkill.SideIntervalToSpeedCurve.Evaluate(_currDirTime / _currDirChangeDelay);
		var sideVelocity = sideDir * Mathf.Lerp(0f, _speedSide, speedLerpVal);

		_rb.linearVelocity = forwardVelocity + sideVelocity;
	}
}
}