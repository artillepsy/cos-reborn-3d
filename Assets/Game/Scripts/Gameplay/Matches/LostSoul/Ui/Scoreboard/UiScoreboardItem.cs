using Gameplay.Matches.LostSoul.Context;
using TMPro;
using UnityEngine;

namespace Gameplay.Matches.LostSoul.Ui.Scoreboard
{
public class UiScoreboardItem : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _playerNameText;
	[SerializeField]
	private TextMeshProUGUI _pointsText;
	[SerializeField]
	private TextMeshProUGUI _killsText;
	[SerializeField]
	private TextMeshProUGUI _deathsText;
	
	
	public void InitClient(MatchContext context)
	{
		
	}
}
}