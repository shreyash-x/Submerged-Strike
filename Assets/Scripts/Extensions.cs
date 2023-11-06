using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static Vector2 xy(this Vector3 vec) => new Vector2(vec.x, vec.y);
    public static Vector2 xz(this Vector3 vec) => new Vector2(vec.x, vec.z);
    public static Vector2 yz(this Vector3 vec) => new Vector2(vec.y, vec.z);

    public static bool IsPartOfLayer(this GameObject go, string layer)
    {
        return go.layer == LayerMask.NameToLayer(layer);
    }
    
    public static bool IsPartOfLayer<T>(this T go, string layer) where T : Behaviour
    {
        return go.gameObject.layer == LayerMask.NameToLayer(layer);
    }

    public static T SelectRandom<T>(this IReadOnlyList<T> list)
    {
        if (list == null || list.Count == 0) return default;
        var rand = Random.Range(0, list.Count);
        return list[rand];
    }
    
    public static T SelectRandomP<T>(this IReadOnlyList<(float probablity, T obj)> list)
    {
        float random = Random.Range(0f, 1f);
        float sum = 0;
        foreach (var tuple in list)
        {
            sum += tuple.probablity;
            if (sum >= random) return tuple.obj;
        }

        return default;
    }
}