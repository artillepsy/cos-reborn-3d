using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Configs.Match
{
/// <summary> Contains default input bindings </summary>
[CreateAssetMenu(menuName = "Configs/Input")]
public class ConfigInput : ScriptableObject
{
	public ConfigInputKeys Keys;
	
	[Serializable]
	public class ConfigInputKeys
	{
		[Header("Movement")]
		public KeyCode MoveLeft = KeyCode.A;
		public KeyCode MoveRight = KeyCode.D;
		public KeyCode MoveUp    = KeyCode.W;
		public KeyCode MoveDown  = KeyCode.S;

		[Header("Skills")]
		public List<KeyCode> Skills;
	}
}
}