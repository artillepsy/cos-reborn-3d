using Gameplay.Matches.LostSoul.Context;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Matches.LostSoul.Ui.Scoreboard
{
public class UiScoreboardItem : MonoBehaviour
{
	[Header("Background")]
	[SerializeField]
	private Image _bg;
	[SerializeField]
	private Color _bgColorDefault = Color.white;
	[SerializeField]
	private Color _bgColorHightlight = Color.white;
	
	[Header("Score")]
	[SerializeField]
	private TextMeshProUGUI _nicknameText;
	[SerializeField]
	private TextMeshProUGUI _pointsText;
	[SerializeField]
	private TextMeshProUGUI _killsText;
	[SerializeField]
	private TextMeshProUGUI _deathsText;
	
	
	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------
	
	public void InitClient(MatchContext context, string nickname, bool isLocalPlayer)
	{
		_nicknameText.text = nickname;
		_bg.color          = isLocalPlayer ? _bgColorHightlight : _bgColorDefault;
	}

	public void SetScore(int points, int kills, int deaths)
	{
		_pointsText.text = points.ToString();
		_killsText.text  = kills.ToString();
		_deathsText.text = deaths.ToString();
	}
}
}