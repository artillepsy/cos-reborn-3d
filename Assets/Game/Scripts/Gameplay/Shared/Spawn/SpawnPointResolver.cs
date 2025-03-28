using System;
using UnityEngine;

namespace Gameplay.Shared.Spawn
{
    /// <summary>
    /// Finds all the spawn points of the spawn object type <typeparamref name="T"/> on the scene.
    /// Resolves in which spawn point the spawn object should be spawned by spawner.
    /// </summary>
    /// <typeparam name="T">Subtype of <see cref="SpawnableObject"/> class.</typeparam>
    public abstract class SpawnPointResolver<T> : MonoBehaviour where T : SpawnableObject
    {
        protected SpawnPoint<T>[] _spawnPoints;

        /// <summary>Finds all the spawn points of the spawn object type <typeparamref name="T"/>.</summary>
        /// <exception cref="Exception">
        /// If there is no spawn point for <typeparamref name="T"/> spawn object type.
        /// </exception>
        public void DetectSpawnPoints()
        {
            _spawnPoints = FindObjectsByType<SpawnPoint<T>>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            if (_spawnPoints.Length == 0)
            {
                throw new Exception($"There are no spawn points for " +
                                    $"{typeof(T)} objects");
            }
        }

        /// <summary>Resolves in which spawn point the spawn object should be spawned by spawner.</summary>
        /// <returns>
        /// <see cref="SpawnPoint{T}"/> object in which spawn object of type
        /// <typeparamref name="T"/> should be spawned by spawner.
        /// </returns>
        public abstract SpawnPoint<T> ResolveSpawnPoint();
    }
}