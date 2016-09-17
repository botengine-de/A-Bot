using System.Collections.Generic;

namespace Sanderling.ABot
{
	static public class EnumerableExtension
	{
		static public IEnumerable<IEnumerable<T>> EnumerateSubsequencesStartingWithFirstElement<T>(
			this IEnumerable<T> sequence)
		{
			var subsequence = new List<T>();

			foreach (var element in sequence)
			{
				subsequence.Add(element);
				yield return subsequence.ToArray();
			}
		}
	}
}
