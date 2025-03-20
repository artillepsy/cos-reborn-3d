using System;
using Game.Scripts.Gameplay.Shared.Skills.Flower;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Gameplay.Configs.Match.Skills
{
[CreateAssetMenu(menuName = "Configs/Skills/Flower")]
public class ConfigSkillFlower : ConfigSkill
{
	[Header("Main")]
	public ConfigSkillFlowerMain Main;

	[Header("Children")]
	public ConfigSkillFlowerChild Child;
	
	//--------------------------------------------------------
	//--------------------------------------------------------

	public override Type Type => typeof(SkillFlower);

	/// <summary> Behaviour of the first projectile, which spawns others (Children) </summary>
	[Serializable]
	public class ConfigSkillFlowerMain
	{
		public NetworkObject ProjectilePrefab;
		[Space]
		public float Speed;
		
		[Space]
		public float LifetimeMin;
		public float LifetimeMax;
		
		/// <summary> How strong the rotation of a single projectile will be.
		/// Gets degrees per lifetime point. (Rotation degrees per sec) </summary>
		[Space]
		public ParticleSystem.MinMaxCurve LifetimeRotationCurve =
			new ParticleSystem.MinMaxCurve(1, new AnimationCurve(), new AnimationCurve());

		public float GetRandLifetime()
		{
			return Random.Range(LifetimeMin, LifetimeMax);
		}
	}

	/// <summary> Behaviour of each individual child projectile </summary>
	[Serializable]
	public class ConfigSkillFlowerChild
	{
		public NetworkObject ProjectilePrefab;
		[Space]
		public float Speed;
		[Space]
		public int CountMin;	
		public int CountMax;
		[Space]
		public float SpawnDelayMin;
		public float SpawnDelayMax;
		[Space]
		public float LifetimeMin;
		public float LifetimeMax;
		/// <summary> How strong the rotation of a single projectile will be.
		/// Gets degrees per lifetime point. (Rotation degrees per sec) </summary>
		[Space]
		public ParticleSystem.MinMaxCurve LifetimeRotationCurve =
			new ParticleSystem.MinMaxCurve(1, new AnimationCurve(), new AnimationCurve());
		
		public int GetRandSpawnCount()
		{
			return Random.Range(CountMin, CountMax);
		}
		
		public float GetRandSpawnDelay()
		{
			return Random.Range(SpawnDelayMin, SpawnDelayMax);
		}
		
		public float GetRandLifetime()
		{
			return Random.Range(LifetimeMin, LifetimeMax);
		}
	}
}
}