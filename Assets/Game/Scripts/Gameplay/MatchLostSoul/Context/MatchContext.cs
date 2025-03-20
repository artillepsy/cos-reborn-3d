using System.Collections.Generic;
using Game.Scripts.Gameplay.Configs.Match;

namespace Game.Scripts.Gameplay.MatchLostSoul.Context
{
/// <summary> Contains all init data. May be divided into client and server contexts in the future </summary>
public class MatchContext
{
	public ConfigsContext Configs;
	
	/// <summary> Should be visible only for server </summary>
	public Dictionary<ulong, MatchPlayerData> PlayersData;
}
}