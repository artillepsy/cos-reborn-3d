using UnityEngine;

namespace Game.Scripts.Gameplay.Configs.Match
{
[CreateAssetMenu(menuName = "Configs/Match Profile")]
public class ConfigMatchProfile : ScriptableObject
{
	public int SkillsNumber;
	public float PlayerRespawnCooldown = 4f;
	public float LostSoulRespawnCooldown = 4f;
}
}