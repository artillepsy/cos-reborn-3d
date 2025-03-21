using System;
using UnityEngine;

namespace Game.Scripts.Gameplay.Shared.Util.Camera
{
public static class CameraHelperC
{
	private static UnityEngine.Camera _cam;
	private static Vector3            _mousePos;
	
	private static UnityEngine.Camera Cam
	{
		get
		{
			if (!_cam)
			{
				_cam = UnityEngine.Camera.main;
			}

			return _cam;
		}
	}
	
	private static readonly Plane _groundPlane = new Plane(Vector3.up, Vector3.zero);

	public static Vector3 GetMouseCursorWorldPos()
	{
		var ray = Cam.ScreenPointToRay(Input.mousePosition);

		if (_groundPlane.Raycast(ray, out var dist))
		{
			var hitPoint = ray.GetPoint(dist);
			_mousePos = hitPoint;
		}
		
		return _mousePos;
	}
}
}