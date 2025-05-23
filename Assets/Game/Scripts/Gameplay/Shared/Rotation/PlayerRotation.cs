using Gameplay.Shared.Util.Camera;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay.Shared.Rotation
{
public class PlayerRotation : NetworkBehaviour
{
	private Rigidbody _rb;
	
	private void Awake()
	{
		_rb = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (!IsLocalPlayer)
		{
			return;
		}

		var mousePos = CameraHelperC.GetMouseCursorWorldPos();
		_rb.rotation = Quaternion.LookRotation(mousePos - _rb.position, Vector3.up);
	}
}
}