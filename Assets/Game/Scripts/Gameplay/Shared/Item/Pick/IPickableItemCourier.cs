namespace Game.Scripts.Gameplay.Shared.Item.Pick
{
    /// <summary>
    /// An object of this class attempts to automatically transfer an item
    /// to the client as soon as their triggers create a collision.
    /// Before transferring the object, a request for such an action is made by client 
    /// using the <see cref="CanDeliverToClient"/> method.
    /// </summary>
    /// <typeparam name="T">Type of pickable item that supports by needed client.</typeparam>
    public interface IPickableItemCourier<T> where T : PickableItem
    {
        /// <summary>Decides whether <paramref name="targetS"/> can absorb the pickable item.</summary>
        /// <param name="targetS">Client that request a pickable item.</param>
        /// <returns><code>true</code> if can deliver to client. Otherwise - <code>false</code>.</returns>
        public bool CanDeliverToClient(PickableItemDeliveryTargetS<T> targetS);
        
        /// <summary>Deliver <typeparamref name="T"/> pickable item to <paramref name="targetS"/>.</summary>
        /// <param name="targetS"></param>
        public void DeliverToClient(PickableItemDeliveryTargetS<T> targetS);
    }
}