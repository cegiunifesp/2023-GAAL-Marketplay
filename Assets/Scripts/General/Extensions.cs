using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static T GetRandomValue<T>(this List<T> list, bool delete = false)
    {
        if (list.Count == 0) return default(T);

        int index = Random.Range(0, list.Count);
        var item = list[index];
        if (delete) list.RemoveAt(index);

        return item;
    }

    public static T GetRandomValue<T>(this T[] array, bool delete = false)
    {
        if (array.Length == 0) return default(T);

        int index = Random.Range(0, array.Length);
        var item = array[index];
        if (delete)
        {
            if (index != array.Length - 1)
            {
                for(int i = index; i < array.Length - 1; i++)
                {
                    array[i] = array[i + 1];
                }
            }
            else
            {
                T[] newArray = new T[array.Length - 1];
            }
        }

        return item;
    }

    public static void DeleteChildren(this Transform parent)
    {
        foreach (Transform child in parent) Object.Destroy(child.gameObject);
    }

    public static Vector3 LastChildPosition(this Transform parent)
    {
        return parent.GetChild(parent.childCount - 1).position;
    }
}
