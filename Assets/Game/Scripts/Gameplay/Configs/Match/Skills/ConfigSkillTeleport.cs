using System;
using Game.Scripts.Gameplay.Shared.Skills.Teleport;
using UnityEngine;

namespace Game.Scripts.Gameplay.Configs.Match.Skills
{
[CreateAssetMenu(menuName = "Configs/Skills/Teleport")]
public class ConfigSkillTeleport : ConfigSkill
{
	public float MaxDist;

	//--------------------------------------------------------
	//--------------------------------------------------------

	public override Type Type => typeof(SkillTeleport);
}
}