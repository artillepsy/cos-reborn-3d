using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Gameplay.MatchLostSoul.Context;
using Game.Scripts.Gameplay.MatchLostSoul.Ui.Shared;
using Game.Scripts.Gameplay.Shared.Skills;
using Game.Scripts.Shared.Extensions;
using Unity.Netcode;
using UnityEngine;

namespace Game.Scripts.Gameplay.MatchLostSoul.Ui.Skills
{
public class UiPlayerSkillsHudPanel : UiComponentBaseC
{
	[SerializeField]
	private List<UiPlayerSkillHudItem> _items;

	private PlayerSkillsManager _skillsManager;
	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------
	private bool _isInitialized;

	public override void InitClient(MatchContext context)
	{
		var managers = FindObjectsByType<PlayerSkillsManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
		_skillsManager = managers.First(a => a.OwnerClientId == NetworkManager.Singleton.LocalClientId);

		int maxSkills = context.Configs.MatchProfile.SkillsNumber;
		for (int i = 0; i < _items.Count; i++)
		{
			_items[i].Init(i);
			_items[i].gameObject.SetActiveSafe(i < maxSkills);
		}

		_isInitialized = true;
	}
	
	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------

	private void Update()
	{
		if (!_isInitialized)
		{
			return;
		}
		foreach (var skillState in _skillsManager.EnumAllSkillStates())	
		{
			_items[skillState.SkillIndex].SetCooldownActive(!skillState.IsReady);

			if (!skillState.IsReady)
			{
				_items[skillState.SkillIndex].SetCooldown(skillState.CooldownCurr);
			}
		}
	}
}
}