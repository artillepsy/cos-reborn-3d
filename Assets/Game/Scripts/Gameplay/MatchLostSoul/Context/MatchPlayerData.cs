using System.Collections.Generic;
using Gameplay.Configs.Match.Skills;

namespace Gameplay.MatchLostSoul.Context
{
/// <summary>
/// Data should be taken from server database, e.g. skills params
/// </summary>
public class MatchPlayerData
{
	public List<List<ConfigSkill>> UnlockedSkillsCollections;
}
}