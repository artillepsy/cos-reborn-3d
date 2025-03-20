using System.Collections.Generic;
using Game.Scripts.Gameplay.Configs.Match.Skills;
using UnityEngine;

namespace Game.Scripts.Gameplay.Shared.Skills.Flower
{
public class ProjectileFlowerMainS : ProjectileBaseS
{
	private ConfigSkillFlower _config;

	private List<float> _childrenSpawnTime  = new List<float>();
	private List<float> _spawnTimesToDelete = new List<float>();

	private int _childSpawnCount;
	private int _childSpawned;
	
	private float _randRotCurveLerp;
	private float _lifeTimeCurr;
	private float _lifeTimeMax;
	private bool  _isRotDirRight;
	
	public void InitServer(ulong ownerClientId, Vector3 dir, ConfigSkillFlower configSkill)
	{
		SetProjOwnerId(ownerClientId);
		_config        = configSkill;

		_lifeTimeCurr     = 0f;
		_lifeTimeMax      = configSkill.Main.GetRandLifetime();
		_isRotDirRight    = Random.value > 0.5f;
		_randRotCurveLerp = Random.value;

		transform.up = dir.normalized;
		
		_childSpawnCount = configSkill.Child.GetRandSpawnCount();

		for (int i = 0; i < _childSpawnCount; i++)
		{
			_childrenSpawnTime.Add(configSkill.Child.GetRandSpawnDelay() + Time.time);
		}
	}

	private void Update()
	{
		if (!IsServer)
		{
			return;
		}
		HandleSpawnChild();
		HandleMoveForward();
		HandleRotation();
		HandleDestroyDelay();
	}

	private void HandleSpawnChild()
	{
		_spawnTimesToDelete.Clear();
		
		foreach (var spawnTime in _childrenSpawnTime)
		{
			if (Time.time < spawnTime)
			{
				continue;
			}
			bool isRotRight = (float)_childSpawned / _childSpawnCount >= 0.5f;
				
			var netObj     = GameObject.Instantiate(_config.Child.ProjectilePrefab);
			var projectile = netObj.GetComponent<ProjectileFlowerChildS>();

			projectile.transform.position = transform.position;
			projectile.InitServer(ProjOwnerId.Value, transform.forward, isRotRight, _config);
			netObj.Spawn();

			_spawnTimesToDelete.Add(spawnTime);
			_childSpawned++;
		}

		foreach (var spawnTime in _spawnTimesToDelete)
		{
			_childrenSpawnTime.Remove(spawnTime);
		}
	}

	private void HandleMoveForward()
	{
		_rb.linearVelocity = transform.up * _config.Child.Speed;
	}

	private void HandleRotation()
	{
		var lifeTimeLerpVal = _lifeTimeCurr / _lifeTimeMax;
		var rotAngleY = _config.Main.LifetimeRotationCurve.Evaluate(lifeTimeLerpVal, _randRotCurveLerp) * Time.deltaTime;
		rotAngleY = _isRotDirRight ? rotAngleY : -rotAngleY;
		transform.Rotate(0f, rotAngleY, 0f);
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