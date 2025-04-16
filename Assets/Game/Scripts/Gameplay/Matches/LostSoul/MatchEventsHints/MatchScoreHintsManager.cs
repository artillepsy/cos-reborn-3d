using System;
using System.Collections.Generic;
using Gameplay.Matches.LostSoul.Context;
using Gameplay.Matches.LostSoul.Score;
using Gameplay.Matches.LostSoul.Ui.MatchEventsHints;
using Gameplay.Shared.Init;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay.Matches.LostSoul.MatchEventsHints
{
public class MatchScoreHintsManager : NetworkBehaviour, IMatchInitServer, IMatchInitClient
{
	private MatchContext            _context;
	private UiMatchEventsHintsPanel _panelC;
	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------
	private Queue<(EScoreType type, ulong? otherPlayerId)> _hintsQueueC = new Queue<(EScoreType type, ulong? otherPlayerId)>();

	private bool  _isInitializedC;
	private ulong _localClientIdC;

	public void InitServer(MatchContext context)
	{
		_context = context;
	}

	public void InitClient(MatchContext context)
	{
		_context        = context;
		_panelC         = FindAnyObjectByType<UiMatchEventsHintsPanel>(FindObjectsInactive.Include);
		_isInitializedC = true;
		_localClientIdC = NetworkManager.Singleton.LocalClientId;
	}
	
	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------

	public override void OnNetworkSpawn()
	{
		if (IsServer)
		{
			MatchEventsS.EvSoreUpdated += LaunchScoreHintS;
		}
	}

	public override void OnNetworkDespawn()
	{
		if (IsServer)
		{
			MatchEventsS.EvSoreUpdated -= LaunchScoreHintS;
		}
	}

	private void LaunchScoreHintS(ulong playerId, EScoreType type, ulong? otherPlayerId)
	{
		if (otherPlayerId != null)
		{
			ProceedClientRpc(playerId, type, otherPlayerId.Value);
		}
		else
		{
			ProceedClientRpc(playerId, type);
		}
	}
	
	[Rpc(SendTo.NotServer)]
	private void ProceedClientRpc(ulong playerId, EScoreType type, ulong otherPlayerId)
	{
		if (_localClientIdC != playerId)
		{
			return;
		}
		
		_hintsQueueC.Enqueue(new (type, otherPlayerId));
	}
	
	[Rpc(SendTo.NotServer)]
	private void ProceedClientRpc(ulong playerId, EScoreType type)
	{
		if (_localClientIdC != playerId)
		{
			return;
		}
		
		_hintsQueueC.Enqueue(new (type, null));
	}

	private void Update()
	{
		if (IsServer || !_isInitializedC)
		{
			return;
		}

		if (!_panelC.IsGoing && _hintsQueueC.Count > 0)
		{
			var hint = _hintsQueueC.Dequeue();
			
			switch (hint.type)
			{
				case EScoreType.Kills:
					_panelC.LaunchKilledHint(_context.PlayersDataC[_localClientIdC].Score.Kills, hint.otherPlayerId.Value);
					break;
				case EScoreType.Deaths:
					_panelC.LaunchDeadHint(hint.otherPlayerId);
					break;
				case EScoreType.Points:
					_panelC.LaunchSoulAbsorbedHint(_context.PlayersDataC[_localClientIdC].Score.Points);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
}