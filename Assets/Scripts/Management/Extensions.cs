using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

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

    public static int Roll(this Vector2 range)
    {
        return (int)UnityEngine.Random.Range(range.x, range.y);
    }
}