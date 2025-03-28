using UnityEngine;

namespace Gameplay.Configs.Match
{
/// <summary> Contains player base info. </summary>
[CreateAssetMenu(menuName = "Configs/Player")]
public class ConfigPlayer : ScriptableObject
{
	public float HealthMax;
	public float Speed;
}
}