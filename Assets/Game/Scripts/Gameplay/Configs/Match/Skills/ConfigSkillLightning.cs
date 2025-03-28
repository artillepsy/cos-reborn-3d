using System;
using Gameplay.Shared.Skills.Lightning;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Configs.Match.Skills
{
[CreateAssetMenu(menuName = "Configs/Skills/Lightning")]
public class ConfigSkillLightning : ConfigSkill
{
	public NetworkObject ProjectilePrefab;
	
	[Header("Speed")]
	public float SpeedForward;
	public float SpeedSide;

	[Header("Trajectory")]
	public float DirChangeTimeStartMin;
	public float DirChangeTimeStartMax;
	[Space]
	public float DirChangeTimeAddMin;
	public float DirChangeTimeAddMax;
	
	//--------------------------------------------------------
	//--------------------------------------------------------
	public override Type Type => typeof(SkillLightning);

	public float GetRandDirChangeTimeStart()
	{
		return Random.Range(DirChangeTimeStartMin, DirChangeTimeStartMax);
	}
	
	public float GetRandDirChangeTimeAdd()
	{
		return Random.Range(DirChangeTimeAddMin, DirChangeTimeAddMax);
	}
}
}