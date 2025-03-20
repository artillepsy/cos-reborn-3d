using System;
using Unity.Netcode;
using UnityEngine;

namespace Game.Scripts.Gameplay.Shared.Item.Pick
{
    public abstract class PickableItemDeliveryTargetS<T> : NetworkBehaviour where T : PickableItem
    {

        public event Action<ulong> EvItemDelivered;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsServer)
            {
                return;
            }
            TryAbsorbFromCourier(other);
        }

        private void TryAbsorbFromCourier(Component other)
        {
            var seekerComponent = other.GetComponent<IPickableItemCourier<T>>();
            if (seekerComponent == null)
            {
                return;
            }
            if (!seekerComponent.CanDeliverToClient(this))
            {
                return;
            }
            
            var netObj = other.GetComponent<NetworkObject>();
            if (netObj == null)
            {
                throw new Exception($"{seekerComponent.GetType()} " +
                                    $"object must be a {nameof(NetworkObject)} object");
            }
            seekerComponent.DeliverToClient(this);
            EvItemDelivered?.Invoke(netObj.OwnerClientId);
        }
    }
}