using System;
using Gameplay.Matches.LostSoul.Score;

namespace Gameplay.Matches.LostSoul
{
/// <summary>
/// Contains events related to the match. Should be used only on server side,
/// listeners should subscribe and unsubscribe.
/// </summary>
public static class MatchEventsS
{
	public static event Action<ulong, ulong?>             EvPlayerDied;
	public static event Action<ulong>                     EvLostSoulAbsorbed;
	public static event Action<ulong>                     EvPlayerSpawned;
	public static event Action                            EvMatchStarted;
	public static event Action<ulong, EScoreType, ulong?> EvSoreUpdated;

	public static void SendEvPlayerDied(ulong playerId, ulong? killerPlayerId)
	{
		EvPlayerDied?.Invoke(playerId, killerPlayerId);
	}
	
	public static void SendEvPlayerSpawned(ulong playerId)
	{
		EvPlayerSpawned?.Invoke(playerId);
	}
	public static void SendEvLostSoulAbsorbed(ulong playerId)
	{
		EvLostSoulAbsorbed?.Invoke(playerId);
	}
	
	public static void SendEvMatchStarted()
	{
		EvMatchStarted?.Invoke();
	}
	
	public static void SendEvScoreUpdated(ulong playerId, EScoreType type, ulong? otherPlayerId)
	{
		EvSoreUpdated?.Invoke(playerId, type, otherPlayerId);
	}
}
}