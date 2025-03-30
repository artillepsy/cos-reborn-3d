using System;
using System.Collections.Generic;
using Gameplay.Configs.Match;
using Gameplay.Matches.LostSoul.Context;
using Gameplay.Matches.LostSoul.Ui.Shared;
using Shared.Extensions;
using Shared.Ui.Layouts;
using UnityEngine;

namespace Gameplay.Matches.LostSoul.Ui.Scoreboard
{
public class UiScoreboardPanel : UiCanvasBaseC
{
	[SerializeField]
	private UiScoreboardItem _itemPrefab;
	[SerializeField]
	private UiVerticalAligner _itemsAligner;

	private ConfigInput _configInput;
	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------
	private bool _isInitialized;
	
	private Dictionary<ulong, UiScoreboardItem> _clientIdToItemMap = new Dictionary<ulong, UiScoreboardItem>();
	
	public override void InitClient(MatchContext context, ulong localPlayerId)
	{
		base.InitClient(context, localPlayerId);
		_configInput   = context.Configs.Input;
		_itemPrefab.gameObject.SetActiveSafe(false);

		foreach (var (clientId, data) in context.PlayersDataC)
		{
			var inst = Instantiate(_itemPrefab, _itemsAligner.transform);
			inst.InitClient(context, data.Nickname, data.PlayerId == localPlayerId);
			inst.SetScore(0, 0, 0);
			inst.gameObject.SetActiveSafe(true);
			_clientIdToItemMap.Add(clientId, inst);
		}
		_itemsAligner.Align();
		_isInitialized = true;
	}

	private void Update()
	{
		if (!_isInitialized)
		{
			return;
		}
		
		if (Input.GetKeyDown(_configInput.Keys.Scoreboard))
		{
			if (IsShown)
			{
				Hide();
			}
			else
			{
				Show();
			}
		}
	}

	public void SetScore(ulong playerId, int points, int kills, int deaths)
	{
		_clientIdToItemMap[playerId].SetScore(points, kills, deaths);
	}
}
}