using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Gameplay.Configs.Match;
using Gameplay.Matches.LostSoul.Context;
using Gameplay.Matches.LostSoul.Score;
using Gameplay.Matches.LostSoul.Ui.Shared;
using Gameplay.Shared.Init;
using Shared.Extensions;
using TMPro;
using UnityEngine;

namespace Gameplay.Matches.LostSoul.Ui.MatchEventsHints
{
public class UiMatchEventsHintsPanel : UiCanvasBaseC
{
	[SerializeField]
	private List<HintMapping> _mappings;

	private ConfigUx.ConfigUxHints _configHints;
	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------
	private HintMapping  _mappingCurr;
	private MatchContext _context;

	private Tween _fadeTween;

	public bool IsGoing => _fadeTween.IsActive() && _fadeTween.IsPlaying();

	public override void InitClient(MatchContext context, ulong localPlayerId)
	{
		base.InitClient(context, localPlayerId);
		
		Debug.Log("initialized");
		_context     = context;
		_configHints = context.Configs.Ux.Hints;

		foreach (var mapping in _mappings)
		{
			mapping.CanvasGroup.alpha = 0f;
			mapping.CanvasGroup.gameObject.SetActiveSafe(true);
		}
	}

	public void LaunchDeadHint(ulong? otherPlayerId)
	{
		_mappingCurr = _mappings.First(m => m.Type == EScoreType.Deaths);

		if (otherPlayerId != null)
		{
			var otherNickname = _context.PlayersDataC[otherPlayerId.Value].Nickname;
			_mappingCurr.TextsDynamic[0].text = $"{otherNickname} killed you";
		}
		else
		{
			_mappingCurr.TextsDynamic[0].text = $"you were slain";
		}

		Launch();
	}

	public void LaunchKilledHint(int kills, ulong otherPlayerId)
	{
		_mappingCurr                      = _mappings.First(m => m.Type == EScoreType.Kills);
		_mappingCurr.TextsDynamic[0].text = $"{kills} kills";

		var otherNickname = _context.PlayersDataC[otherPlayerId].Nickname;
		_mappingCurr.TextsDynamic[1].text = $"Killed player {otherNickname}"; 
		Launch();
	}

	public void LaunchSoulAbsorbedHint(int points)
	{
		_mappingCurr                  = _mappings.First(m => m.Type == EScoreType.Points);
		_mappingCurr.TextsDynamic[0].text = $"{points} points"; 
		Launch();
	}

	private void Launch()
	{
		if (_mappingCurr == null)
		{
			Debug.LogError("No current mapping set!");
			return;
		}
		
		_mappingCurr.CanvasGroup.alpha = 0f;
		_mappingCurr.CanvasGroup.gameObject.SetActive(true);

		
		Debug.Log($"Launching tween for {_mappingCurr.Type}");
    
		// Kill any existing tween first
		if (_fadeTween != null && _fadeTween.IsActive())
		{
			_fadeTween.Kill();
		}
		
		_fadeTween = GetFadeTween(_configHints.Duration);
		_fadeTween.OnPlay(() => Debug.Log("Tween started playing"))
		   .OnComplete(() => Debug.Log("Tween completed"));
		_fadeTween?.Restart();
	}

	public void Stop()
	{
		_fadeTween?.Complete();
		_mappingCurr.CanvasGroup.gameObject.SetActive(false); 
	}

	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------

	private Tween GetFadeTween(float showTime)
	{
		Debug.Log($"Creating fade tween for {showTime} seconds");
		var seq = DOTween.Sequence();
		seq.Append(_mappingCurr.CanvasGroup.DOFade(1f, 1f))
		   .AppendInterval(showTime)
		   .Append(_mappingCurr.CanvasGroup.DOFade(0f, 1f))
		   .OnComplete(() =>
			{
				Debug.Log("Tween sequence completed");
				_mappingCurr.CanvasGroup.gameObject.SetActive(false);
				_fadeTween?.Kill();
				_fadeTween = null;
			});
		return seq;
	}

	[Serializable]
	public class HintMapping
	{
		public EScoreType            Type;
		public CanvasGroup           CanvasGroup;
		public List<TextMeshProUGUI> TextsDynamic;
	}
}
}