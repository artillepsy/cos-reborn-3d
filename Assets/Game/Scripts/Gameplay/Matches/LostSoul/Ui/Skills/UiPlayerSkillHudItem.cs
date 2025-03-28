using Shared.Extensions;
using TMPro;
using UnityEngine;

namespace Gameplay.Matches.LostSoul.Ui.Skills
{
public class UiPlayerSkillHudItem : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _skillIndexText;
	[SerializeField]
	private TextMeshProUGUI _currCooldownText;

	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------
	
	public void Init(int skillIndex)
	{
		_skillIndexText.text = $"{skillIndex + 1}";
	}

	public void SetCooldownActive(bool isActive)
	{
		_currCooldownText.gameObject.SetActiveSafe(isActive);
	}

	public void SetCooldown(float cooldown)
	{
		_currCooldownText.text = NumbStrHelper.ToStr(cooldown, false, 1, 1);
	}
}
}