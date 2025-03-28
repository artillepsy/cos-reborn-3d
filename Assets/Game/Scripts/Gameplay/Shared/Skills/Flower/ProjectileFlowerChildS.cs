using Gameplay.Configs.Match.Skills;
using UnityEngine;

namespace Gameplay.Shared.Skills.Flower
{
public class ProjectileFlowerChildS : ProjectileBaseS
{
	private ConfigSkillFlower _config;
	
	private float _randRotCurveLerp;
	private float _lifeTimeCurr;
	private float _lifeTimeMax;
	private bool  _isRotDirRight;

	public void InitServer(ulong ownerClientId, Vector3 dir, bool isRotRight, ConfigSkillFlower configSkill)
	{
		SetProjOwnerId(ownerClientId);
		_config = configSkill;
		
		_lifeTimeCurr     = 0f;
		_lifeTimeMax      = configSkill.Child.GetRandLifetime();
		_isRotDirRight    = isRotRight;
		_randRotCurveLerp = Random.value;
		
		transform.forward = dir.normalized;
	}
	
	private void Update()
	{
		if (!IsServer)
		{
			return;
		}
		HandleMoveForward();
		HandleRotation();
		HandleDestroyDelay();
	}
	
	private void HandleMoveForward()
	{
		_rb.linearVelocity = transform.forward * _config.Main.Speed;
	}

	private void HandleRotation()
	{
		var lifeTimeLerpVal = _lifeTimeCurr / _lifeTimeMax;
		var rotAngleY       = _config.Main.LifetimeRotationCurve.Evaluate(lifeTimeLerpVal, _randRotCurveLerp) * Time.deltaTime;
		rotAngleY = _isRotDirRight ? rotAngleY : -rotAngleY;
		transform.Rotate(0f, rotAngleY, 0f );
	}

	private void HandleDestroyDelay()
	{
		_lifeTimeCurr += Time.deltaTime;
		
		if (_lifeTimeCurr >= _lifeTimeMax)
		{
			_netObj.Despawn();
		}
	}
}
}