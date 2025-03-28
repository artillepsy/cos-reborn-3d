using UnityEngine;

namespace Shared.Extensions
{
public static class GameObjectExtensions
{
	/// <summary> Optimized for (de)activating (de)activated go </summary>
	public static void SetActiveSafe(this GameObject go, bool isActive)
	{
		if (go.activeSelf != isActive)
			go.SetActive(isActive);
	}
}
}