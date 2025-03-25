using System;
using System.Collections.Generic;
using Game.Scripts.Gameplay.Configs.Match;
using Game.Scripts.Gameplay.Configs.Match.Skills;
using Game.Scripts.Gameplay.MatchLostSoul.Context;
using Game.Scripts.Gameplay.MatchLostSoul.Player;
using Game.Scripts.Gameplay.MatchLostSoul.Spawn.LostSoul;
using Game.Scripts.Gameplay.MatchLostSoul.Spawn.LostSoulAbsorber;
using Game.Scripts.Gameplay.Shared.Spawn.Player;
using Unity.Netcode;
using UnityEngine;

namespace Game.Scripts.Gameplay.MatchLostSoul.Startup
{
public class MatchStartup : NetworkBehaviour
{
	[SerializeField]
	private ConfigsContext _configsContext;
	
	[Header("Spawners")]
	[SerializeField]
	private PlayerSpawnerServer _playerSpawnerServer;
	[SerializeField] 
	private LostSoulSpawnerServer _lostSoulSpawnerServer;
	[SerializeField] 
	private LostSoulAbsorberSpawnerServer _lostSoulAbsorberSpawnerServer;

	[Header("Setters")]
	[SerializeField]
	private MatchPlayerSkillsSetterS _playerSkillsSetter;
	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------

	private bool _isMatchStarted;
	
	private MatchContext _context;

	public MatchContext Context => _context;

	private void Awake()
	{
		Application.targetFrameRate = 60;
	}

	private void Update()
	{
		if (IsServer && !_isMatchStarted)
		{
			if (Input.GetKeyDown(KeyCode.S))
			{
				_isMatchStarted = true;
				StartMatchS();
			}
		}
	}

	/// <summary>
	/// Normally, all the players should be spawned by this moment. All the data should be taken from the database
	/// </summary>
	private void StartMatchS()
	{
		if (!IsServer)
		{
			return;
		}
		
		_context = new MatchContext()
		{
			Configs = _configsContext,
		};
			
		SetupPlayersDataServer();
		SetupClientContextRpc();

		_playerSpawnerServer.InitServer(_context);
		_playerSkillsSetter.InitServer(_context);
		_lostSoulSpawnerServer.InitServer(_context);
			
		_playerSpawnerServer.Spawn();
		_playerSkillsSetter.SetupStartSkills();
			
		_lostSoulSpawnerServer.Spawn();
		_lostSoulAbsorberSpawnerServer.Spawn();

		MatchEventsS.SendEvMatchStarted();
	}
 
	//TODO: Get unlocked skills from database
	private void SetupPlayersDataServer()
	{
		_context.PlayersData = new Dictionary<ulong, MatchPlayerData>();
		
		foreach (var (clientId, netClient) in NetworkManager.ConnectedClients)
		{
			var data = new MatchPlayerData()
			{
				UnlockedSkillsCollections = new List<List<ConfigSkill>>(),
			};

			for (var i = 0; i < _configsContext.Skills.Groups.Count; i++)
			{
				var collection = _configsContext.Skills.Groups[i];
				var list       = new List<ConfigSkill>();
				list.AddRange(collection.Skills);
				data.UnlockedSkillsCollections.Add(list);
			}

			_context.PlayersData.Add(clientId, data);	
		}
	}

	[Rpc(SendTo.NotServer)]
	private void SetupClientContextRpc()
	{
		_context = new MatchContext()
		{
			Configs = _configsContext,
		};
	}
}
}