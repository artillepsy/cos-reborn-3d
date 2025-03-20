using System;
using UnityEngine;

namespace Game.Scripts.Gameplay.Shared.Util.SerializableDataStructure
{
[Serializable]
public class Vector3Serializable : ASerializable<Vector3>
{
	private float _x;
	private float _y;
	private float _z;
	
	public Vector3Serializable(Vector3 originalVector) : base(originalVector)
	{
		_x = originalVector.x;
		_y = originalVector.y;
		_z = originalVector.z;
	}

	public override Vector3 ToObject()
	{
		return new Vector3(_x, _y, _z);
	}

	public override string ToString()
	{
		return $"({_x}, {_y}, {_z})";
	}
}
}