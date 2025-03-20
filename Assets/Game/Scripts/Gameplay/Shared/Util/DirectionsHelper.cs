using System;
using Game.Scripts.Gameplay.Shared.Visuals;
using UnityEngine;

namespace Game.Scripts.Gameplay.Shared.Util
{
public static class DirectionsHelper
{
	public static EDir GetDir(Vector2Int moveDir)
	{
		if (moveDir == Vector2Int.up)
		{
			return EDir.Up;
		}
		if (moveDir == Vector2Int.right)
		{
			return EDir.Right;
		}
		if (moveDir == Vector2Int.down)
		{
			return EDir.Down;
		}
		if (moveDir == Vector2Int.left)
		{
			return EDir.Left;
		}

		if (moveDir == new Vector2Int(1, 1))
		{
			return EDir.UpRight;
		}
		if (moveDir == new Vector2Int(1, -1))
		{
			return EDir.DownRight;
		}
		if (moveDir == new Vector2Int(-1, -1))
		{
			return EDir.DownLeft;
		}
		if (moveDir == new Vector2Int(-1, 1))
		{
			return EDir.UpLeft;
		}

		throw new Exception($"Can not convert vector values: {moveDir} into any direction");
	}

	public static bool IsRight(EDir dir)
	{
		return dir is EDir.UpRight or EDir.Right or EDir.DownRight;
	}
}
}