using System;
using System.Collections.Generic;
using Gameplay.Configs.Match;
using Gameplay.Configs.Match.Skills;
using Gameplay.MatchLostSoul.Context;
using Gameplay.Shared.Init;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay.Shared.Skills
{
/// <summary> Responsible for skills. Attach to the player gameObject </summary>
public class PlayerSkillsManager : NetworkBehaviour, IMatchInitServer, IMatchInitClient
{
	private MatchContext _context;
	private ConfigSkills _configSkills;
	private ConfigInput  _configInputC;
	//--------------------------------------------------------
	//--------------------------------------------------------
	private List<SkillBase> _currSkills = new List<SkillBase>();
	
	public void InitServer(MatchContext context)
	{
		_context      = context;
		_configSkills = context.Configs.Skills;
	}

	public void InitClient(MatchContext context)
	{
		_context      = context;
		_configSkills = context.Configs.Skills;
		_configInputC = context.Configs.Input;
	}
	
	//--------------------------------------------------------
	//--------------------------------------------------------

	private void Update()
	{
		HandleSkillsCooldown();
		HandleInputC();
	}

	private void HandleSkillsCooldown()
	{
		if (!IsServer && !IsOwner)
		{
			return;
		}
		foreach (var skill in _currSkills)
		{
			skill.UpdateCooldown();
		}
	}

	private void HandleInputC()
	{
		if (!IsOwner)
		{
			return;
		}
		for (int i = 0; i < _context.Configs.MatchProfile.SkillsNumber; i++)
		{
			if (Input.GetKeyDown(_configInputC.Keys.Skills[i]))
			{
				_currSkills[i].OnKeyDownC();
			}
			if (Input.GetKey(_configInputC.Keys.Skills[i]))
			{
				_currSkills[i].OnKeyPressC();
			}
			if (Input.GetKeyUp(_configInputC.Keys.Skills[i]))
			{
				_currSkills[i].OnKeyUpC();
			}
		}
	}

	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------

	public void SetupSkillsS(List<ConfigSkill> skills)
	{
		_currSkills.Clear();
		var skillsId = new List<ushort>();

		for (var i = 0; i < skills.Count; i++)
		{
			var config = skills[i];
			var inst = Activator.CreateInstance(config.Type) as SkillBase;
			inst.InitServer(_context, config, this, i);
			_currSkills.Add(inst);
			skillsId.Add(config.Id);
		}

		// set skills also on the client side, replace configs with skills params in the future
		SetupSkillsClientRpc(skillsId.ToArray());
	}
	
	public void ReplaceSkillS(int skillIndex, ConfigSkill config)
	{
		var inst = Activator.CreateInstance(config.Type) as SkillBase;
		inst.InitClient(_context, config, this,  skillIndex);
		_currSkills[skillIndex] = inst;
		
		ReplaceSkillsClientRpc(skillIndex, config.Id);
	}

	[Rpc(SendTo.Owner)]
	private void ReplaceSkillsClientRpc(int skillIndex, ushort skillId)
	{
		var config = _configSkills.Get(skillId);
		var inst   = Activator.CreateInstance(config.Type) as SkillBase;
		inst.InitClient(_context, config, this, skillIndex);
		_currSkills[skillIndex] = inst;
	}
	
	[Rpc(SendTo.Owner)]
	private void SetupSkillsClientRpc(ushort[] skillsId)
	{
		for (var i = 0; i < skillsId.Length; i++)
		{
			var id     = skillsId[i];
			var config = _configSkills.Get(id);
			var inst   = Activator.CreateInstance(config.Type) as SkillBase;
			inst.InitClient(_context, config, this,  i);
			_currSkills.Add(inst);
		}
	}
	
	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------
	
	
	/// <summary>
	/// call these methods from SkillBase class to use the skill. If the skill is not ready yet, server will not start usage
	/// </summary>
	///<remarks>Check for skill is ready is removed for fake shots cases due to high latency</remarks>
	/// <param name="dto">Additional byte data to send</param>
	public void ReceiveSkillDtoC(int skillIndex, byte[] dto)
	{
		SkillInvokeMethodServerRpc(skillIndex, dto);
	}

	/// <summary>
	/// Sends a request to use a skill to the server. If the skill is not ready yet, server will not start usage
	/// </summary>
	/// <param name="dto">Additional byte data to send</param>
	[Rpc(SendTo.Server)]
	private void SkillInvokeMethodServerRpc(int skillIndex, byte[] dto)
	{
		if (_currSkills[skillIndex].IsReady)
		{
			_currSkills[skillIndex].OnDtoReceivedServer(dto);
		}
	}
	
	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------

	public IEnumerable<ISkillState> EnumAllSkillStates()
	{
		foreach (var skill in _currSkills)
		{
			yield return skill;
		}
	}
}
}