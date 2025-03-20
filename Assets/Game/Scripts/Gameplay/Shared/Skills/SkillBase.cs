using System;
using Game.Scripts.Gameplay.Configs.Match.Skills;
using Game.Scripts.Gameplay.MatchLostSoul.Context;
using UnityEngine;

namespace Game.Scripts.Gameplay.Shared.Skills
{
//todo: replace with Server postfix
public abstract class SkillBase : ISkillState
{
	protected PlayerSkillsManager _manager;
	protected Transform           _tf;

	protected Plane     _groundPlaneC;
	protected Camera    _camC;

	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------
	public ConfigSkill Config { get; private set; }
	
	public int   SkillIndex   { get; protected set; }
	public float CooldownMax  { get; protected set; }
	public float CooldownCurr { get; private set; }
	public bool  IsReady      => CooldownCurr <= 0f;

	/// <summary>
	/// Initializes class on server side.
	/// </summary>
	/// <param name="context">MatchContext object</param>
	/// <param name="config">Config skill object</param>
	/// <param name="manager">PlayerSkillsManager object</param>
	/// <param name="skillIndex">Skill index in the array of skills</param>
	public virtual void InitServer(MatchContext context, ConfigSkill config, PlayerSkillsManager manager, int skillIndex)
	{
		SkillIndex = skillIndex;
		Config     = config;
		_manager   = manager;
		
		_tf = _manager.transform;

		CooldownMax  = config.Cooldown;
		StartCooldown();
	}
	
	/// <summary>
	/// Initializes class on client side.
	/// </summary>
	/// <param name="context">MatchContext object</param>
	/// <param name="config">Config skill object</param>
	/// <param name="manager">PlayerSkillsManager object</param>
	/// <param name="skillIndex">Skill index in the array of skills</param>
	public virtual void InitClient(MatchContext context, ConfigSkill config, PlayerSkillsManager manager, int skillIndex)
	{
		SkillIndex = skillIndex;
		Config     = config;
		_manager   = manager;
		_tf        = _manager.transform;

		_groundPlaneC = new Plane(Vector3.up, Vector3.zero);
		_camC         = Camera.main;

		CooldownMax = config.Cooldown;
		StartCooldown();
	}
	
	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------

	//todo: check for different components between client-server
	
	/// <summary> Calls when PlayerSkillsManager when skill's input key is pressed on client side. </summary>
	public virtual void OnKeyPressC() { }
	
	/// <summary> Calls when PlayerSkillsManager when skill's input key is down on client side. </summary>
	public virtual void OnKeyDownC() { }
	
	/// <summary> Calls when PlayerSkillsManager when skill's input key is up on client side. </summary>
	public virtual void OnKeyUpC() { }

	/// <summary> Updates cooldown of the skill. Now works only on server </summary>
	public void UpdateCooldown()
	{
		if (CooldownCurr <= 0f)
		{
			return;
		}

		CooldownCurr -= Time.deltaTime;

		if (CooldownCurr < 0f)
		{
			CooldownCurr = 0f;
		}
	}
	
	/// <summary> Starts skill cooldown. Skill can't be used at that moment </summary>
	protected void StartCooldown()
	{
		CooldownCurr = CooldownMax;
	}

	protected Vector3 GetMouseCursorPosC()
	{
		var ray = _camC.ScreenPointToRay(Input.mousePosition);

		if (_groundPlaneC.Raycast(ray, out var dist))
		{
			var hitPoint = ray.GetPoint(dist);
			return hitPoint;
		}

		throw new Exception("There is no point for mouse cursor on the ground");
	}
	
	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------

	/// <summary>
	/// Receives a dto from the server which can contain any serializable data (direction, position, etc)
	/// </summary>
	/// <param name="bytesDto">byte array with serializable data</param>
	public virtual void OnDtoReceivedServer(byte[] bytesDto) { }
}
}