using System;
using Gameplay.Shared.Skills.Teleport;
using UnityEngine;

namespace Gameplay.Configs.Match.Skills
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