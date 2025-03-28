using UnityEngine;

namespace Gameplay.Shared.Spawn
{
    /// <summary>Resolves random spawn point of spawn object type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">Subtype of <see cref="SpawnableObject"/> class.</typeparam>
    public class RandomSpawnPointResolver<T> : SpawnPointResolver<T> where T : SpawnableObject
    {
        public override SpawnPoint<T> ResolveSpawnPoint()
        {
            return _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        }
    }
}