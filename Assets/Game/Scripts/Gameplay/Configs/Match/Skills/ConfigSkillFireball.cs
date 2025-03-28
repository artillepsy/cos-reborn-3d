using System;
using Gameplay.Shared.Skills.Fireball;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay.Configs.Match.Skills
{
[CreateAssetMenu(menuName = "Configs/Skills/Fireball")]
public class ConfigSkillFireball : ConfigSkill
{
	public NetworkObject ProjectilePrefab;
	
	public float Speed;

	//--------------------------------------------------------
	//--------------------------------------------------------

	public override Type Type => typeof(SkillFireball);
}
}