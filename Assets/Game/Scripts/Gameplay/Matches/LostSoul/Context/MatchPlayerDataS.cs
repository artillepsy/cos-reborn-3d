using System.Collections.Generic;
using Gameplay.Configs.Match.Skills;
using Gameplay.Matches.LostSoul.Score;

namespace Gameplay.Matches.LostSoul.Context
{
/// <summary>
/// Data should be taken from server database, e.g. skills params
/// </summary>
public class MatchPlayerDataS
{
	public ulong       ClientId;
	public string      Nickname;
	public PlayerScore Score;
	
	public List<List<ConfigSkill>> UnlockedSkillsCollections;
}
}