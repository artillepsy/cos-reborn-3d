using System.Collections.Generic;
using System.Linq;
using Gameplay.Configs.Match.Skills;
using Gameplay.MatchLostSoul.Context;
using Gameplay.MatchLostSoul.Startup;
using Gameplay.Shared.Init;
using Gameplay.Shared.Skills;
using Shared.Extensions;
using Shared.Logging;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay.MatchLostSoul.Player
{
/// <summary>
/// If there is more than one group of replaceable skills,
/// the class will change only first skill group (for simplification yet)
/// </summary>
public class MatchPlayerSkillsSetterS : NetworkBehaviour, IMatchInitServer
{
	private MatchContext _context;
	private ConfigSkills _configSkills;
	//--------------------------------------------------------
	//--------------------------------------------------------
	//todo: netObject as key
	private Dictionary<ulong, List<ConfigSkill>> _playerIdToSkillsMap = new Dictionary<ulong, List<ConfigSkill>>();
	
	public void InitServer(MatchContext context)
	{
		_context      = context;
		_configSkills = context.Configs.Skills;
	}
	
	public override void OnNetworkSpawn()
	{
		if (!IsServer)
		{
			return;
		}
		MatchEventsS.EvLostSoulAbsorbed += TrySetNewRandSkill;
	}

	public override void OnNetworkDespawn()
	{
		if (!IsServer)
		{
			return;
		}
		MatchEventsS.EvLostSoulAbsorbed -= TrySetNewRandSkill;
	}

	//--------------------------------------------------------
	//--------------------------------------------------------

	public void SetupStartSkills()
	{
		foreach (var (clientId, netClient) in NetworkManager.ConnectedClients)
		{
			_playerIdToSkillsMap.Add(clientId, new List<ConfigSkill>());

			var data = _context.PlayersData[clientId];
			
			for (int i = 0; i < _context.Configs.MatchProfile.SkillsNumber; i++)
			{
				_playerIdToSkillsMap[clientId].Add(data.UnlockedSkillsCollections[i].Random());
			}
			Log.Inf(nameof(MatchStartup), $"init skills map for player, id: {clientId}");
		}

		var managers = FindObjectsByType<PlayerSkillsManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
		
		foreach (var manager in managers)
		{
			var clientId = manager.NetworkObject.OwnerClientId;
			Log.Inf(nameof(MatchPlayerSkillsSetterS), $"init player skills with id: {clientId}");
			manager.SetupSkillsS(_playerIdToSkillsMap[clientId]);
		}
	}
	
	private void TrySetNewRandSkill(ulong playerId)
	{
		var manager = FindObjectsByType<PlayerSkillsManager>(FindObjectsInactive.Include, FindObjectsSortMode.None)
		   .First(a => a.OwnerClientId == playerId);

		var data = _context.PlayersData[playerId];
		
		for (int i = 0; i < _context.Configs.MatchProfile.SkillsNumber; i++)
		{
			if (!_configSkills.Groups[i].IsReplaceable)
			{
				continue;
			}

			var currSkillToRemove = _playerIdToSkillsMap[playerId][i];
			var newSkill = data.UnlockedSkillsCollections[i].Random(currSkillToRemove);

			_playerIdToSkillsMap[playerId][i] = newSkill;
			
			manager.ReplaceSkillS(i, newSkill);
			
			Log.Inf(nameof(MatchPlayerSkillsSetterS), 
			        $"Player {playerId} skill {currSkillToRemove} was replaced by {newSkill}");

			return;
		}
		
		Log.Err(nameof(MatchPlayerSkillsSetterS), $"iThere is no skill to replace");
	}
}
}