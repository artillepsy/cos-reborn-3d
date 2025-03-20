using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Game.Scripts.Gameplay.Shared.Util
{
public static class BinarySerializer
{
	public static byte[] Serialize<T>(T dto)
	{
		IFormatter formatter = new BinaryFormatter();
		using MemoryStream stream = new MemoryStream();

		formatter.Serialize(stream, dto);
		var bytes = stream.ToArray();

		return bytes;
	}
	
	public static T Deserialize<T>(byte[] bytes)
	{
		using MemoryStream ms = new MemoryStream(bytes);
		IFormatter         br = new BinaryFormatter();
		return (T)br.Deserialize(ms);
	}
}
}