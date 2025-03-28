using System.Collections.Generic;
using Gameplay.Configs.Match;

namespace Gameplay.Matches.LostSoul.Context
{
/// <summary> Contains all init data. May be divided into client and server contexts in the future </summary>
public class MatchContext
{
	public ConfigsContext Configs;
	
	/// <summary> Should be accessible only for server </summary>
	public Dictionary<ulong, MatchPlayerDataS> PlayersDataS;
	
	/// <summary> Should be accessible only for client </summary>
	public Dictionary<ulong, MatchPlayerDataC> PlayersDataC;
}
}