using Gameplay.Shared.Item.Pick;

namespace Gameplay.Matches.LostSoul.Item.Pick.LostSoul
{
    /// <summary><see cref="PickableItem"/> subclass.</summary>
    public class LostSoulPickableItem : PickableItem
    {
        
        
        public override bool CanBePicked<T>(ItemPickerS<T> itemPickerS)
        {
            return true;
        }
    }
}