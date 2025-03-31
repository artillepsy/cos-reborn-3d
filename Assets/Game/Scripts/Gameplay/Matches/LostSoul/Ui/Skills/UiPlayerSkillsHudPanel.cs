using System.Collections.Generic;
using System.Linq;
using Gameplay.Matches.LostSoul.Context;
using Gameplay.Matches.LostSoul.Ui.Shared;
using Gameplay.Shared.Skills;
using Shared.Extensions;
using Shared.Ui.Layouts;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay.Matches.LostSoul.Ui.Skills
{
/// <summary>
/// The panel at the corner of the screen which displays current skill information (cooldown yet)
/// </summary>
public class UiPlayerSkillsHudPanel : UiCanvasBaseC
{
	[SerializeField]
	private UiHorizontalAligner _aligner;
	[SerializeField]
	private List<UiPlayerSkillHudItem> _items;

	private PlayerSkillsManager _skillsManager;
	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------
	private bool _isInitialized;

	public override void InitClient(MatchContext context, ulong localPlayerId)
	{
		base.InitClient(context, localPlayerId);
		
		var managers = FindObjectsByType<PlayerSkillsManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
		_skillsManager = managers.First(a => a.OwnerClientId == localPlayerId);

		int maxSkills = context.Configs.MatchProfile.SkillsNumber;
		for (int i = 0; i < _items.Count; i++)
		{
			_items[i].Init(i);
			_items[i].gameObject.SetActiveSafe(i < maxSkills);
		}

		_isInitialized = true;
		_aligner.Align();
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