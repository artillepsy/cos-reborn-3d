using DG.Tweening;
using Gameplay.Configs.Match;
using Gameplay.Matches.LostSoul.Context;
using Gameplay.Matches.LostSoul.Ui.Shared;
using Gameplay.Shared.Init;
using TMPro;
using UnityEngine;

namespace Gameplay.Matches.LostSoul.Ui.Hints
{
public class UiHintsPanel : UiCanvasBaseC, IMatchInitClient
{
	[SerializeField]
	private TextMeshProUGUI _hintText;
	[SerializeField]
	private CanvasGroup _canvasGroup;

	private ConfigUx.ConfigUxHints _configHints;
	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------
	
	private Tween _fadeTween;

	public bool IsGoing => _fadeTween.IsActive() && _fadeTween.IsPlaying();

	public void InitClient(MatchContext context)
	{
		_configHints = context.Configs.Ux.Hints;
	}

	public void LaunchDeadHint()
	{
		_hintText.text  = "You were slain";
		_hintText.color = _configHints.DeadColor;
		Launch();
	}

	public void LaunchKillHint(int kills)
	{
		_hintText.text  = $"{kills} kills";
		_hintText.color = _configHints.DefaultColor;
	}

	public void LaunchSoulAbsorbedHint()
	{
		_hintText.text  = $"Soul absorbed";
		_hintText.color = _configHints.SoulAbsorbedColor;
	}

	private void Launch()
	{
		_canvasGroup.alpha = 0f;
		_canvasGroup.gameObject.SetActive(true);

		_fadeTween ??= GetFadeTween(_configHints.Duration);
		_fadeTween?.Restart();
	}

	public void Stop()
	{
		_fadeTween?.Complete();
		_canvasGroup.gameObject.SetActive(false); 
	}

	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------

	private Tween GetFadeTween(float showTime)
	{
		var seq = DOTween.Sequence();
		seq.Append(_canvasGroup.DOFade(1f, 1f));
		seq.AppendInterval(showTime);
		seq.Append(_canvasGroup.DOFade(1f, 1f));
		seq.OnComplete(() => _canvasGroup.gameObject.SetActive(false));
		return seq.SetAutoKill(false).Pause();
	}
}
}