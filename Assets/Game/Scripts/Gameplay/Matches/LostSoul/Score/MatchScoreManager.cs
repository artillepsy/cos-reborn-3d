using System.Collections.Generic;
using Gameplay.Matches.LostSoul.Context;
using Gameplay.Matches.LostSoul.Ui.Scoreboard;
using Gameplay.Shared.Init;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay.Matches.LostSoul.Score
{
public class MatchScoreManager : NetworkBehaviour, IMatchInitServer, IMatchInitClient
{
	private UiScoreboardPanel _panelC;
	
	private Dictionary<ulong, PlayerScore> _clientIdToScoreMapS = new Dictionary<ulong, PlayerScore>();
	
	public override void OnNetworkSpawn()
	{
		if (IsServer)
		{
			MatchEventsS.EvPlayerDied       += OnPlayerDied;
			MatchEventsS.EvLostSoulAbsorbed += OnLostSoulAbsorbed;
		}
	}

	public override void OnNetworkDespawn()
	{
		if (IsServer)
		{
			MatchEventsS.EvPlayerDied       -= OnPlayerDied;
			MatchEventsS.EvLostSoulAbsorbed -= OnLostSoulAbsorbed;
		}
	}
	
	public void InitServer(MatchContext context)
	{
		foreach (var (clientId, _) in context.PlayersDataS)
		{
			_clientIdToScoreMapS.Add(clientId, new PlayerScore());
		}
	}

	public void InitClient(MatchContext context)
	{
		_panelC = FindAnyObjectByType<UiScoreboardPanel>(FindObjectsInactive.Include);
	}

	private void OnPlayerDied(ulong playerId, ulong? killerId)
	{
		var score1 = _clientIdToScoreMapS[playerId];
		score1.Deaths++;

		UpdatePlayerScoreClientRpc(playerId, score1.Points, score1.Kills, score1.Deaths);

		if (killerId != null && killerId != playerId)
		{
			var score2 = _clientIdToScoreMapS[killerId.Value];
			score2.Kills++;
			UpdatePlayerScoreClientRpc(killerId.Value, score2.Points, score2.Kills, score2.Deaths);
		}
	}

	private void OnLostSoulAbsorbed(ulong playerId)
	{
		var score = _clientIdToScoreMapS[playerId];
		score.Points++;		
		UpdatePlayerScoreClientRpc(playerId, score.Points, score.Kills, score.Deaths);
	}

	[Rpc(SendTo.NotServer)]
	private void UpdatePlayerScoreClientRpc(ulong playerId, int points, int kills, int deaths)
	{
		_panelC.SetScore(playerId, points, kills, deaths);
	}

	private class PlayerScore
	{
		public int Points;
		public int Kills;
		public int Deaths;
	}
}
}