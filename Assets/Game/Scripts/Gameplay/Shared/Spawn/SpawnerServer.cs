using Unity.Netcode;
using UnityEngine;

namespace Gameplay.Shared.Spawn
{
    /// <summary>
    /// Spawns <see cref="SpawnableObject"/> object in some spawn point. The <see cref="SpawnPointResolver{T}"/> desides
    /// in which spawn point the object should be spawned.
    /// </summary>
    /// <typeparam name="T">Type of <see cref="SpawnableObject"/> object that could be spawned by the spawner.</typeparam>
    public class SpawnerServer<T> : NetworkBehaviour where T : SpawnableObject
    {
        /// <summary>
        /// <see cref="SpawnPointResolver{T}"/> object. Desides n which spawn point the object should be spawned.
        /// </summary>
        [SerializeField] protected SpawnPointResolver<T> _spawnPointResolver;
        [SerializeField] protected T _spawnObject;

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                return;
            }
            _spawnPointResolver.DetectSpawnPoints();
        }

        /// <summary>Spawns the object in one of the spawn points using the spawn point resolver.</summary>
        public virtual void Spawn()
        {
            _spawnPointResolver.ResolveSpawnPoint().SpawnObject(_spawnObject);
        }
    }
}