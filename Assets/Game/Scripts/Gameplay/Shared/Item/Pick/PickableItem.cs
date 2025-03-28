using Unity.Netcode;

namespace Gameplay.Shared.Item.Pick
{
    /// <summary> Base abstract class for items, that can be picked by <see cref="ItemPickerS{T}"/>.</summary>
    public abstract class PickableItem : NetworkBehaviour
    {
        /// <summary>Decides whether <paramref name="itemPickerS"/> can pick this item or not.</summary>
        /// <param name="itemPickerS">The <see cref="ItemPickerS{T}"/> object that tries to pick the item.</param>
        /// <typeparam name="T">Type param for <see cref="ItemPickerS{T}"/>.</typeparam>
        /// <returns>
        /// <code>true</code> if <paramref name="itemPickerS"/> can pick this item.
        /// <code>false</code> - otherwise.
        /// </returns>
        public abstract bool CanBePicked<T>(ItemPickerS<T> itemPickerS) where T : PickableItem;
    }
}