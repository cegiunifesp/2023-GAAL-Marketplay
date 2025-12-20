using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Extensions
{
    public static void AddList<T>(this List<T> list, List<T> listToAdd)
    {
        if (list == null) list = new List<T>();

        foreach(T item in listToAdd) 
            list.Add(item);
    }

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
                for (int i = index; i < array.Length - 1; i++)
                {
                    newArray[i] = array[i + 1];
                }
                array = newArray;
            }
        }

        return item;
    }

    public static void FadesIn(this AudioSource audio, float time = 1)
    {
        if (audio == null) return;

        float volume = audio.volume;
        audio.Play();
        LeanTween.value(0, volume, time).setOnUpdate((value) =>
        {
            audio.volume = value;
        });
    }

    public static void FadesOut(this AudioSource audio, Action action, float time = 1)
    {
        if (audio == null) return;

        float volume = audio.volume;
        LeanTween.value(volume, 0, time).setOnUpdate((value) =>
        {
            audio.volume = value;
        }).setOnComplete(() => action?.Invoke());
    }

    public static void DeleteChildren(this Transform parent)
    {
        foreach (Transform child in parent) UnityEngine.Object.DestroyImmediate(child.gameObject);
    }

    public static Vector3 LastChildPosition(this Transform parent)
    {
        return parent.GetChild(parent.childCount - 1).position;
    }
}
