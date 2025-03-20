using System.Collections.Generic;
using Game.Scripts.Gameplay.Configs.Match.Skills;

namespace Game.Scripts.Gameplay.MatchLostSoul.Context
{
/// <summary>
/// Data should be taken from server database, e.g. skills params
/// </summary>
public class MatchPlayerData
{
	public List<List<ConfigSkill>> UnlockedSkillsCollections;
}
}