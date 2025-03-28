using Gameplay.Matches.LostSoul.Context;
using UnityEngine;

namespace Gameplay.Matches.LostSoul.Ui.Shared
{
/// <summary>
/// Every root panel/screen on client side on scene must be inherited
/// from this to be initialized at the beginning of the match
/// </summary>
public abstract class AUiInitCompBaseC : MonoBehaviour
{
	public abstract void InitClient(MatchContext context);
}
}