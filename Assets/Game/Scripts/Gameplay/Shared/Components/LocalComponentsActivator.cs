using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.Shared.Components
{
public class LocalComponentsActivator : MonoBehaviour
{
	[SerializeField]
	protected List<Behaviour> _excludedComps;

	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------
	private List<Behaviour> _disabledBehs;
	private List<Renderer>  _disabledRenderers;
	private List<Cloth>     _disabledCloths;
	
	public void SetActive(bool isActive)
	{
		if (!isActive)
		{
			// SAFE: Get all behaviours, then filter manually to avoid LINQ issues
			var allBehaviours = GetComponentsInChildren<Behaviour>(false);
			_disabledBehs = new List<Behaviour>();

			foreach (var b in allBehaviours)
			{
				if (b == null || !b.enabled || b.GetType() != typeof(Behaviour) || _excludedComps.Contains(b))
				{
					continue;
				}

				Debug.Log($"behaviour type to add: {b.GetType()}, name: {b.name}");
				
				_disabledBehs.Add(b);
			}
			
			_disabledRenderers = GetComponentsInChildren<Renderer>(false).ToList();
			_disabledCloths    = GetComponentsInChildren<Cloth>(false).ToList();

			_disabledBehs.ForEach(b => b.enabled      = false);
			_disabledRenderers.ForEach(r => r.enabled = false);
			_disabledCloths.ForEach(c => c.enabled    = false);
		}
		else
		{
			if (_disabledBehs == null || _disabledRenderers == null)
			{
				throw new Exception("activation should be called after deactivation");
			}
			_disabledBehs.ForEach(b => b.enabled      = true);
			_disabledRenderers.ForEach(r => r.enabled = true);
			_disabledCloths.ForEach(c => c.enabled    = true);
			
			_disabledBehs      = null;
			_disabledRenderers = null;
			_disabledCloths    = null;
		}
	}
}
}