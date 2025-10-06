using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtentionMethods
{
    public static void ResetTransform(this Transform trans)
    {
        trans.position = Vector3.zero;
        trans.rotation = Quaternion.identity;
        trans.localScale = Vector3.one;
    }
    public static T GetRandomElement<T>(this List<T> list)
    {
        int RandomIndex = Random.Range(0, list.Count);
        return list[RandomIndex];
    }
    public static T GetRandomElement<T>(this T[] list)
    {
        int RandomIndex = Random.Range(0, list.Length);
        return list[RandomIndex];
    }
}
