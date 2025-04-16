using Gameplay.Matches.LostSoul.Context;
using Gameplay.Matches.LostSoul.Ui.Scoreboard;
using Gameplay.Shared.Init;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay.Matches.LostSoul.Score
{
public class MatchScoreManager : NetworkBehaviour, IMatchInitServer, IMatchInitClient
{
	private MatchContext _context;
	
	private UiScoreboardPanel _panelC;

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
		_context = context;
	}

	public void InitClient(MatchContext context)
	{
		_context = context;
		_panelC  = FindAnyObjectByType<UiScoreboardPanel>(FindObjectsInactive.Include);
	}

	private void OnPlayerDied(ulong playerId, ulong? killerId)
	{
		var score1 = _context.PlayersDataS[playerId].Score;
		score1.Deaths++;

		UpdatePlayerScoreClientRpc(playerId, score1.Points, score1.Kills, score1.Deaths);
		MatchEventsS.SendEvScoreUpdated(playerId, EScoreType.Deaths, killerId);
		
		if (killerId != null && killerId != playerId)
		{
			var score2 = _context.PlayersDataS[killerId.Value].Score;
			score2.Kills++;
			UpdatePlayerScoreClientRpc(killerId.Value, score2.Points, score2.Kills, score2.Deaths);
			MatchEventsS.SendEvScoreUpdated(killerId.Value, EScoreType.Kills, playerId);
		}
	}

	private void OnLostSoulAbsorbed(ulong playerId)
	{
		var score = _context.PlayersDataS[playerId].Score;
		score.Points++;		
		UpdatePlayerScoreClientRpc(playerId, score.Points, score.Kills, score.Deaths);
		MatchEventsS.SendEvScoreUpdated(playerId, EScoreType.Points, null);
	}

	[Rpc(SendTo.NotServer)]
	private void UpdatePlayerScoreClientRpc(ulong playerId, int points, int kills, int deaths)
	{
		var score = _context.PlayersDataC[playerId].Score;
		score.Points = points;
		score.Kills  = kills;
		score.Deaths = deaths;
		_panelC.SetScore(playerId, points, kills, deaths);
	}
}
}