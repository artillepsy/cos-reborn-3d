using Game.Scripts.Gameplay.Shared.Util.Camera;
using Unity.Netcode;
using UnityEngine;

namespace Game.Scripts.Gameplay.Shared.Rotation
{
public class PlayerRotation : NetworkBehaviour
{
	private void Update()
	{
		if (!IsLocalPlayer)
		{
			return;
		}

		var mousePos = CameraHelperC.GetMouseCursorWorldPos();
		transform.LookAt(mousePos, Vector3.up);
		
		Debug.DrawRay(mousePos, Vector3.up * 3, Color.red);
	}
}
}