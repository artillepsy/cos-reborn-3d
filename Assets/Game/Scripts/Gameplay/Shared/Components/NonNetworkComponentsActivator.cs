using UnityEngine;

namespace Game.Scripts.Gameplay.Shared.Components
{
public class NonNetworkComponentsActivator : LocalComponentsActivator
{
	private void Awake()
	{
		Debug.Assert(_excludedComps.Count > 0, "There is no excluded network components");
	}
}
}