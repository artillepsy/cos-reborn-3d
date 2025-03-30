using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Shared.Util
{
public static class MathHelper
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 WithX(this Vector3 v, float x)
	{
		v.x = x;
		return v;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 WithY(this Vector3 v, float y)
	{
		v.y = y;
		return v;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 WithZ(this Vector3 v, float z)
	{
		v.z = z;
		return v;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 WithX(this Vector2 v, float x)
	{
		v.x = x;
		return v;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 WithY(this Vector2 v, float y)
	{
		v.y = y;
		return v;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 GetValidStepTo(this Vector2 from, Vector2 to, float stepDist)
	{	
		float dX      = to.x - from.x;
		float dY      = to.y - from.y;
		float distSqr = dX * dX + dY * dY;
		if (distSqr <= stepDist * stepDist)
			return to;
		var dist = (float)Math.Sqrt(distSqr);
		return new Vector2((dX / dist) * stepDist, (dY / dist) * stepDist);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 MoveTowardExt(
		this Vector2 from, 
		Vector2 to, 
		float stepDist,
		out bool isReached)
	{	
		float dX      = to.x - from.x;
		float dY      = to.y - from.y;
		float distSqr = dX * dX + dY * dY;
		
		if (distSqr <= stepDist * stepDist)
		{
			isReached = true;
			return to;
		}
		isReached = false;
		
		var dist = (float)Math.Sqrt(distSqr);
		return new Vector2(
			from.x + (dX / dist) * stepDist,
			from.y + (dY / dist) * stepDist
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsSame(this float a, float b)
	{
		return Math.Abs(a - b) <= float.Epsilon;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Min(float a, float b, float c, float d)
	{
		return Math.Min(a, Math.Min(b, Math.Min(c, d)));
	}

	public static float Remap(this float value, float from1, float to1, float from2, float to2)
	{
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}
}
}