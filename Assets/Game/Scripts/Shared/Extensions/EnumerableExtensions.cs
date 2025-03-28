using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Extensions
{
public static class EnumerableExtensions
{
	public static T Random<T>(this IList<T> collection)
	{
		return collection[UnityEngine.Random.Range(0, collection.Count)];
	}
	
	public static T Random<T>(this IList<T> collection, T exception)
	{
		var newCollection = collection.Except(new List<T>() {exception}).ToList();

		if (newCollection.Count == 0)
		{
			throw new Exception(
				$"the collection [{nameof(collection)}] contains 0 items after exception item [{exception}]");
		}
		
		return newCollection[UnityEngine.Random.Range(0, newCollection.Count)];
	}
	
	public static T Random<T>(this IList<T> collection, List<T> exceptions)
	{
		var newCollection = collection.Except(exceptions).ToList();
		return newCollection[UnityEngine.Random.Range(0, newCollection.Count)];
	}
}
}