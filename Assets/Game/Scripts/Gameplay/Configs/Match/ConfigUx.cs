using System;
using BlahEditor.Attributes;
using UnityEngine;

namespace Gameplay.Configs.Match
{
/// <summary> Contains settings related to UI/UX. </summary>
[CreateAssetMenu(menuName = "Configs/Ux")]
public class ConfigUx : ScriptableObject
{
	[NoFoldout, Header("Hints")]
	public ConfigUxHints Hints;
	
	[Serializable]
	public class ConfigUxHints
	{
		public float Duration;
		[Space]
		public Color DefaultColor      = Color.white;
		public Color DeadColor         = Color.white;
		public Color SoulAbsorbedColor = Color.white;
	}
}
}