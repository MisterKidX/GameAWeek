using System;
using System.Collections.Generic;
using System.Linq;

public static class Extensions
{
    private static Random _random = new Random();

    public static T GetRandom<T>(this IEnumerable<T> collection)
    {
        if (collection == null || collection.Count() == 0)
            throw new ArgumentException("Collection must have at least one item.");

        var index = _random.Next(collection.Count());
        
        return collection.ElementAt(index);
    }
}