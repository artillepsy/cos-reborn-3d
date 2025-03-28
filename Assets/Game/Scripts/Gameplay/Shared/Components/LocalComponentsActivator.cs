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
	
	public void SetActive(bool isActive)
	{
		if (!isActive)
		{
			_disabledBehs      = GetComponentsInChildren<Behaviour>(false).Except(_excludedComps).ToList();
			_disabledRenderers = GetComponentsInChildren<Renderer>(false).ToList();

			_disabledBehs.ForEach(b => b.enabled      = false);
			_disabledRenderers.ForEach(r => r.enabled = false);
		}
		else
		{
			if (_disabledBehs == null || _disabledRenderers == null)
			{
				throw new Exception("activation should be called after deactivation");
			}
			_disabledBehs.ForEach(b => b.enabled      = true);
			_disabledRenderers.ForEach(b => b.enabled = true);
			
			_disabledBehs      = null;
			_disabledRenderers = null;
		}
	}
}
}