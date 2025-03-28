using System;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay.Shared.Item.Pick
{
    /// <summary>
    /// Object that absorb <see cref="PickableItem"/> objects when their triggers colliders make collision.
    /// </summary>
    /// <typeparam name="T"><see cref="PickableItem"/> subtype that can be absorbed by this object.</typeparam>
    public abstract class PickableItemAbsorberS<T> : NetworkBehaviour where T : PickableItem
    {
        private PickableItemDeliveryTargetS<T> _deliveryTargetS;
        public event Action<ulong> EvItemAbsorbed;

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                return;
            }
            _deliveryTargetS = GetComponent<PickableItemDeliveryTargetS<T>>();
            if (_deliveryTargetS is null)
            {
                return;
            }
            _deliveryTargetS.EvItemDelivered += InvokeEvItemAbsorbedEvent;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer)
            {
                return;
            }
            if (_deliveryTargetS is null)
            {
                return;
            }
            _deliveryTargetS.EvItemDelivered -= InvokeEvItemAbsorbedEvent;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer)
            {
                return;
            }
            TryAbsorbPickableItem(other);
        }

        /// <summary>Absorbs <paramref name="pickableItem"/> object.</summary>
        /// <param name="pickableItem">Item to absorb.</param>
        protected virtual void Absorb(T pickableItem)
        {
            pickableItem.GetComponent<NetworkObject>().Despawn();
        }

        private void TryAbsorbPickableItem(Component other)
        {
            var pickableItemComponent = other.GetComponent<T>();
            if (pickableItemComponent == null)
            {
                return;
            }

            var netObj = other.GetComponent<NetworkObject>();
            if (netObj == null)
            {
                throw new Exception($"{pickableItemComponent.GetType()} " +
                                    $"object must be a {nameof(NetworkObject)} object");
            }
            Absorb(pickableItemComponent);
            EvItemAbsorbed?.Invoke(netObj.OwnerClientId);
        }

        private void InvokeEvItemAbsorbedEvent(ulong ownerId)
        {
            EvItemAbsorbed?.Invoke(ownerId);
        }
    }
}