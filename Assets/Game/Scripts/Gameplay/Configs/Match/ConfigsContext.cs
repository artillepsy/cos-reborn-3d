using Game.Scripts.Gameplay.Configs.Match.Skills;
using UnityEngine;

namespace Game.Scripts.Gameplay.Configs.Match
{
/// <summary> Contains all the configs. </summary>
[CreateAssetMenu(menuName = "Configs/Context")]
public class ConfigsContext : ScriptableObject
{
	public ConfigMatchProfile MatchProfile;
	public ConfigInput        Input;
	public ConfigPlayer       Player;
	public ConfigSkills       Skills;
}
}