﻿using System;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay.Shared.Spawn.Player
{
    public class PlayerSpawnPoint : SpawnPoint<PlayerSpawnableObject>
    {
        public override PlayerSpawnableObject SpawnObject(PlayerSpawnableObject spawnableObject)
        {
            var netObj = spawnableObject.GetComponent<NetworkObject>();
            if (netObj is null)
            {
                throw new Exception($"{nameof(PlayerSpawnableObject)} object must be an {nameof(NetworkObject)}");
            }
            var inst = Instantiate(netObj, transform.position, Quaternion.identity);
            inst.SpawnAsPlayerObject(spawnableObject.ClientIdS);

            return inst.GetComponent<PlayerSpawnableObject>();
        }
    }
}