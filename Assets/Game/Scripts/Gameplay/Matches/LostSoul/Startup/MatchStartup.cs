using System.Collections.Generic;
using Gameplay.Configs.Match;
using Gameplay.Configs.Match.Skills;
using Gameplay.Matches.LostSoul.Context;
using Gameplay.Matches.LostSoul.Score;
using Gameplay.Matches.LostSoul.Skills;
using Gameplay.Matches.LostSoul.Spawn.LostSoul;
using Gameplay.Matches.LostSoul.Spawn.LostSoulAbsorber;
using Gameplay.Shared.Spawn.Player;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay.Matches.LostSoul.Startup
{
public class MatchStartup : NetworkBehaviour
{
	[SerializeField]
	private ConfigsContext _configsContext;
	
	[Header("Spawners")]
	[SerializeField]
	private PlayerSpawnerS _playerSpawnerS;
	[SerializeField] 
	private LostSoulSpawnerS _lostSoulSpawnerS;
	[SerializeField] 
	private LostSoulAbsorberSpawnerS _lostSoulAbsorberSpawnerS;

	[Header("Setters")]
	[SerializeField]
	private PlayersSkillsSetterS _playerSkillsSetterS;

	[Header("Managers")]
	[SerializeField]
	private MatchScoreManager _matchScoreManager;

	
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

		_playerSpawnerS.InitServer(_context);
		_playerSkillsSetterS.InitServer(_context);
		_lostSoulSpawnerS.InitServer(_context);
			
		_matchScoreManager.InitServer(_context);
		
		_playerSpawnerS.Spawn();
		_playerSkillsSetterS.SetupStartSkills();
			
		_lostSoulSpawnerS.Spawn();
		_lostSoulAbsorberSpawnerS.Spawn();

		StartMatchClientRpc();
		MatchEventsS.SendEvMatchStarted();
	}
 
	//TODO: Get unlocked skills from database
	private void SetupPlayersDataServer()
	{
		_context.PlayersDataS = new Dictionary<ulong, MatchPlayerDataS>();
		
		foreach (var (clientId, _) in NetworkManager.ConnectedClients)
		{
			var data = new MatchPlayerDataS()
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

			_context.PlayersDataS.Add(clientId, data);	
		}
	}

	[Rpc(SendTo.NotServer)]
	private void SetupClientContextRpc()
	{
		var playersData = new Dictionary<ulong, MatchPlayerDataC>();

		foreach (var clientId in NetworkManager.ConnectedClients.Keys)
		{
			playersData.Add(clientId, new MatchPlayerDataC()
			{
				Nickname = $"Player {clientId}",
				PlayerId =  clientId,
			});
		}

		_context = new MatchContext
		{
			Configs      = _configsContext,
			PlayersDataC = playersData
		};
	}

	[Rpc(SendTo.NotServer)]
	private void StartMatchClientRpc()
	{
		_matchScoreManager.InitClient(_context);

	}
}
}