using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Gameplay.Configs.Match.Skills
{
[CreateAssetMenu(menuName = "Configs/Skills/Collection")]
public class ConfigSkills : ScriptableObject
{
	[SerializeField]
	private List<ConfigSkill> _allSkills;
	
	public List<ConfigSkillsGroup> Groups;
	
	[Serializable]
	public class ConfigSkillsGroup
	{
		[Tooltip("If true, skills from this group can be replaced others " +
		         "in the same group when the soul is delivered by a player")]
		public bool              IsReplaceable;
		public List<ConfigSkill> Skills;
	}
	
	//--------------------------------------------------------
	//--------------------------------------------------------

	public ConfigSkill Get(ushort id)
	{
		foreach (var config in _allSkills)
		{
			if (config.Id == id)
			{
				return config;
			}
		}

		throw new Exception($"there's no skill with id: {id}");
	}
}
}