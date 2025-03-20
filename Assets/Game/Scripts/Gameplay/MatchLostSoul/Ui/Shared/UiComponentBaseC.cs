using Game.Scripts.Gameplay.MatchLostSoul.Context;
using UnityEngine;

namespace Game.Scripts.Gameplay.MatchLostSoul.Ui.Shared
{
/// <summary>
/// Every panel on client side on scene must inherit from this to be initialized at the beginning of the match
/// </summary>
public abstract class UiComponentBaseC : MonoBehaviour
{
	public abstract void InitClient(MatchContext context);
}
}