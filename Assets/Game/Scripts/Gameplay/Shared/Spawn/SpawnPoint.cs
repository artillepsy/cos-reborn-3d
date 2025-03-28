using System;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay.Shared.Spawn
{
    /// <summary>
    /// Base class for spawn point. Spawn point is one of points where <typeparamref name="T"/>
    /// <see cref="SpawnObject"/> can be spawned by spawner.
    /// </summary>
    /// <typeparam name="T">Type of <see cref="SpawnObject"/> that can be spawner in this spawn point.</typeparam>
    /// <seealso cref="SpawnerS{T}"/>.
    public abstract class SpawnPoint<T> : MonoBehaviour where T : SpawnableObject
    {
        /// <summary>Spawns the spawn object.</summary>
        /// <param name="spawnObject">Spawn object to spawn.</param>
        /// <exception cref="Exception">If spawn object is not a <see cref="NetworkObject"/> object.</exception>
        public virtual void SpawnObject(T spawnObject)
        {
            var go = Instantiate(spawnObject);
            go.transform.position = transform.position;
            var netObj = go.GetComponent<NetworkObject>();
            if (netObj is null)
            {
                throw new Exception($"Spawn object must be a {nameof(NetworkObject)}");
            }
            netObj.Spawn();
        }
    }
}