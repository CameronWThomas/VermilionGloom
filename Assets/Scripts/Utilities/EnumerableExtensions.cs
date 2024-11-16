using System.Collections.Generic;
using System.Linq;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Randomize<T>(this IEnumerable<T> enumerable)
    {
        var originalList = enumerable.ToList();
        var randomList = new List<T>();

        while (originalList.Any())
        {
            var randomItem = originalList[UnityEngine.Random.Range(0, originalList.Count)];
            randomList.Add(randomItem);
            originalList.Remove(randomItem);
        }

        return randomList;
    }
}