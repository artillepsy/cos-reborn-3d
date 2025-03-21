using Game.Scripts.Gameplay.MatchLostSoul.Context;
using Game.Scripts.Gameplay.Shared.Init;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Scripts.Gameplay.Shared.Camera
{
public class PlayerCameraFollow : MonoBehaviour, IMatchInitClient
{
	public void InitClient(MatchContext context)
	{
		var cam = FindAnyObjectByType<CinemachineCamera>();
		cam.Follow = transform;
	}
}
}