using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static T GetRandom<T>(this List<T> list, bool delete = false)
    {
        var item = list[Random.Range(0, list.Count)];
        if (delete) list.Remove(item);

        return item;
    }
}
