using System;
using UnityEngine;

namespace Game.Scripts.Gameplay.Configs.Match.Skills
{
/// <summary> Base skill config. Used by the server (?) </summary>
public abstract class ConfigSkill : ScriptableObject
{
	[Header("Params")]
	public ushort Id;
	//public bool  IsInputHold;
	public float Cooldown;

	//todo: add destroy delay
	
	//--------------------------------------------------------
	//--------------------------------------------------------

	public abstract Type Type { get; }
}
}