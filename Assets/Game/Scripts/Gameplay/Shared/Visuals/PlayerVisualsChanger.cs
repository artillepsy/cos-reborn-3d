using System;
using System.Collections.Generic;
using Game.Scripts.Gameplay.Configs.Match;
using Game.Scripts.Gameplay.MatchLostSoul.Context;
using Game.Scripts.Gameplay.Shared.Init;
using Game.Scripts.Gameplay.Shared.Util;
using Unity.Netcode;
using UnityEngine;

namespace Game.Scripts.Gameplay.Shared.Visuals
{
public class PlayerVisualsChanger : NetworkBehaviour, IMatchInitServer, IMatchInitClient
{
	[SerializeField]
	private bool _isRightOriented;
	[SerializeField]
	private SpriteRenderer _spriteRederer;
	[SerializeField]
	private List<SideVisual> _visuals;
	
	private ConfigInput _configInput;
	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------
	private Dictionary<EDir, Sprite> _dirToSpriteMap = new Dictionary<EDir, Sprite>();

	public NetworkVariable<Vector2Int> _moveDir = new NetworkVariable<Vector2Int>(
		default,
		NetworkVariableReadPermission.Everyone,
		NetworkVariableWritePermission.Owner
	);
	
	public void InitServer(MatchContext context)
	{
		_configInput = context.Configs.Input;
	}

	public void InitClient(MatchContext context)
	{
		_configInput = context.Configs.Input;
	}

	public override void OnNetworkSpawn()
	{
		foreach (var visual in _visuals)
		{
			if (!_dirToSpriteMap.TryAdd(visual.Dir, visual.Sprite))
			{
				throw new Exception($"The key {visual.Dir} is already added to the dictionary");
			}
		}

		_moveDir.OnValueChanged += OnMoveDirChanged;
	}

	private void OnMoveDirChanged(Vector2Int prevMoveDir, Vector2Int newMoveDir)
	{
		if (prevMoveDir == newMoveDir || newMoveDir == Vector2Int.zero)
		{
			return;
		}

		var dir = DirectionsHelper.GetDir(newMoveDir);
		_spriteRederer.sprite = _dirToSpriteMap[dir];
		_spriteRederer.flipX  = !(DirectionsHelper.IsRight(dir) && _isRightOriented);
	}

	private void Update()
	{
		if (!IsLocalPlayer)
		{
			return;
		}

		_moveDir.Value = GetMoveInputDelta();
	}

	private Vector2Int GetMoveInputDelta()
	{
		int x = 0;
		int y = 0;
		
		if (Input.GetKey(_configInput.Keys.MoveLeft))
		{
			x -= 1;
		}
		if (Input.GetKey(_configInput.Keys.MoveRight))
		{
			x += 1;
		}
		if (Input.GetKey(_configInput.Keys.MoveUp))
		{
			y += 1;
		}
		if (Input.GetKey(_configInput.Keys.MoveDown))
		{
			y -= 1;
		}
		return new Vector2Int(x, y);
	}

	[Serializable]
	public class SideVisual
	{
		public EDir Dir;
		public Sprite   Sprite;
	}

}
}