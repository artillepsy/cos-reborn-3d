﻿using System;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay.Shared.Item.Pick
{
    /// <summary>Component that allows to pick <see cref="PickableItem"/> objects.</summary>
    /// <typeparam name="T">Type of <see cref="PickableItem"/> that the picker will react to pick.</typeparam>
    public abstract class ItemPickerS<T> : NetworkBehaviour where T : PickableItem
    {
        [SerializeField]
        protected T _itemPrefab;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer)
            {
                return;
            }
            var pickableItemComponent = other.GetComponent<T>();
            if (pickableItemComponent == null)
            {
                return;
            }
            if (!pickableItemComponent.CanBePicked(this))
            {
                return;
            }
            Pick(pickableItemComponent);
        }

        /// <summary>Pick <paramref name="pickableItem"/> object.</summary>
        /// <param name="pickableItem">Item to pick.</param>
        public virtual void Pick(T pickableItem)
        {
            if (!IsServer)
            {
                throw new Exception("It's not server");
            }
            pickableItem.GetComponent<NetworkObject>()?.Despawn();
        }

        /// <summary>Drops previously picked item if such exists.</summary>
        public virtual void Drop()
        {
            if (!IsServer)
            {
                throw new Exception("It's not server");
            }
            var inst = Instantiate(_itemPrefab, transform.position, Quaternion.identity);
            inst.GetComponent<NetworkObject>().Spawn();
        }
    }
}