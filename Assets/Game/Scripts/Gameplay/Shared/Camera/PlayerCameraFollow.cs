using Cinemachine;
using Gameplay.Matches.LostSoul.Context;
using Gameplay.Shared.Init;
using UnityEngine;

namespace Gameplay.Shared.Camera
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