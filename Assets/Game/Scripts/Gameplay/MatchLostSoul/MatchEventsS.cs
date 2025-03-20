using System;

namespace Game.Scripts.Gameplay.MatchLostSoul
{
/// <summary>
/// Contains events related to the match. Should be used only on server side,
/// listeners should subscribe and unsubscribe.
/// </summary>
public static class MatchEventsS
{
	public static event Action<ulong> EvPlayerDied;
	public static event Action<ulong> EvLostSoulAbsorbed;
	public static event Action<ulong> EvPlayerSpawned;
	public static event Action        EvMatchStarted;

	public static void SendEvPlayerDied(ulong playerId)
	{
		EvPlayerDied?.Invoke(playerId);
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
}
}