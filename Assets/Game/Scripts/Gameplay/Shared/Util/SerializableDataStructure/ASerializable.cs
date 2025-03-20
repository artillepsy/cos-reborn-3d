using System;

namespace Game.Scripts.Gameplay.Shared.Util.SerializableDataStructure
{
[Serializable]
public abstract class ASerializable<T>
{
	public ASerializable(T data) { }

	public abstract T ToObject();
}
}