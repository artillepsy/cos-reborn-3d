using Cinemachine;
using Game.Scripts.Gameplay.MatchLostSoul.Context;
using Game.Scripts.Gameplay.Shared.Init;
using UnityEngine;

namespace Game.Scripts.Gameplay.Shared.Camera
{
public class PlayerCameraFollow : MonoBehaviour, IMatchInitClient
{
	public void InitClient(MatchContext context)
	{
		var cam = FindAnyObjectByType<CinemachineVirtualCamera>();
		cam.Follow = transform;
	}
}
}