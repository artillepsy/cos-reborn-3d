using Gameplay.MatchLostSoul.Context;
using UnityEngine;

namespace Gameplay.MatchLostSoul.Ui.Shared
{
/// <summary>
/// Every root panel/screen on client side on scene must be inherited
/// from this to be initialized at the beginning of the match
/// </summary>
public abstract class UiInitCompBaseC : MonoBehaviour
{
	public abstract void InitClient(MatchContext context);
}
}