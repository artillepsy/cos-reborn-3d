using Gameplay.Matches.LostSoul.Context;
using UnityEngine;

namespace Gameplay.Matches.LostSoul.Ui.Shared
{
/// <summary>
/// Every root panel/screen on client side on scene must be inherited
/// from this to be initialized at the beginning of the match
/// </summary>
public abstract class UiCanvasBaseC : MonoBehaviour
{
	protected Canvas _canvas;

	//---------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------
	
	public bool IsShown => _canvas.enabled;

	public virtual void InitClient(MatchContext context, ulong localPlayerId)
	{
		_canvas = GetComponent<Canvas>();
	}

	public void Show()
	{
		_canvas.enabled = true;
	}

	public void Hide()
	{
		_canvas.enabled = false;
	}
}
}