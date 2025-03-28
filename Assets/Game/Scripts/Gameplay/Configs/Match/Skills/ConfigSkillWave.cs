using System;
using Gameplay.Shared.Skills.Wave;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Configs.Match.Skills
{
[CreateAssetMenu(menuName = "Configs/Skills/Wave")]
public class ConfigSkillWave : ConfigSkill
{
	public NetworkObject ProjectilePrefab;
	[Space]
	public float SpeedForward;
	[Space]
	public float SpeedSideMin;
	public float SpeedSideMax;
	[Space]
	public float DirChangeDelayMin;
	public float DirChangeDelayMax;
	[Space]
	public float DirDelayGainMin;
	public float DirDelayGainMax;
	[Space]
	public AnimationCurve SideIntervalToSpeedCurve;
	//--------------------------------------------------------
	//--------------------------------------------------------

	public override Type Type => typeof(SkillWave);
	
	public float GetRandSpeedSide()
	{
		return Random.Range(SpeedSideMin, SpeedSideMax);
	}
		
	public float GetRandDirChangeDelay()
	{
		return Random.Range(DirChangeDelayMin, DirChangeDelayMax);
	}
	
	public float GetRandDirDelayGain()
	{
		return Random.Range(DirDelayGainMin, DirDelayGainMax);
	}
}
}