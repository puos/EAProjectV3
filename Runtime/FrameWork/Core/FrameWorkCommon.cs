using System;
using System.Collections.Generic;
using UnityEngine;

public static class EAFrameUtil
{
    public static int EnumTo(object value) { return System.Convert.ToInt32(value); }

    public static Component AddChild(GameObject parent, Type type, string name = null)
    {
        GameObject go = AddChild(parent);
        if (!string.IsNullOrEmpty(name)) go.name = name;
        return go.AddComponent(type);
    }

    public static GameObject AddChild(GameObject parent,string name = null)
    {
        GameObject go = AddChild(parent);
        if (!string.IsNullOrEmpty(name)) go.name = name;
        return go;
    }

    /// <summary>
    /// Add a child object to the specified parent and attaches the specified script to it.
    /// </summary>
    public static T AddChild<T>(GameObject parent,T prefab) where T : Component
    {
        GameObject go = AddChild(parent, prefab.gameObject);
        return go.GetComponent<T>();
    }

    public static GameObject AddChild(GameObject parent, GameObject prefab)
    {
        GameObject go = GameObject.Instantiate(prefab) as GameObject;

        if(go != null && parent != null)
        {
            Transform t = go.transform;
            t.SetParent(parent.transform);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            go.layer = parent.layer;
        }
        return go;
    }

    
    // Add a child object to the specified parent and attaches the specified script to it.
    static public T AddChild<T>(GameObject parent) where T : Component
    {
        GameObject go = AddChild(parent);

        string s = typeof(T).ToString();
        if (s.StartsWith("UI")) s = s.Substring(2);
        else if (s.StartsWith("UnityEngine.")) s = s.Substring(12);
        go.name = s;

        return go.AddComponent<T>();
    }

    public static GameObject AddChild(GameObject parent)
    {
        GameObject go = new GameObject();

        if(parent != null)
        {
            Transform t = go.transform;
            t.SetParent(parent.transform);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            go.layer = parent.layer;
        }

        return go;
    }

    public static void DestroyObject(EAObject obj)
    {
        GameObject.Destroy(obj.gameObject);
    }

    public static void DestroyChildren(this Transform tf,bool bInsertPool = true)
    {
        while(0 != tf.childCount)
        {
            Transform tfChild = tf.GetChild(0);
            tfChild.SetParent(null);
            GameResourceManager.instance.ReleaseObject(tfChild.gameObject, bInsertPool);
        }
    }

    public static Transform FindChildRecursively(Transform parent, string name)
    {
        if (name.Equals(parent.name,StringComparison.Ordinal))
        {
            return parent;
        }

        for (int i = 0; i < parent.childCount; ++i)
        {
            Transform child = parent.GetChild(i);

            Transform result = FindChildRecursively(child, name);

            if (result != null) return result;
        }

        return null;
    }

    public static void Reset(this Transform tf)
    {
        tf.localPosition = Vector3.zero;
        tf.localRotation = Quaternion.identity;
        tf.localScale = Vector3.one;
    }

    public static void Reset(this RectTransform tf)
    {
        tf.anchoredPosition3D = Vector3.zero;
        tf.sizeDelta = Vector2.zero;
        tf.localScale = Vector3.one;
    }

    public static void SetParent(Transform tf,Transform tParent)
    {
        tf.SetParent(tParent);
        RectTransform tRect = tf.GetComponent<RectTransform>();
        if (tRect == null) tf.Reset();
        if (tRect != null) tRect.Reset();
    }

    /// <summary>
    /// Used when you want to call a certain property getter without using return value.
    /// 예) Bounds bounds = scrollView.bounds; // 'unused variable' Warning triggered.
    ///      EAFrameUtil.Call(scrollView.bounds);   // No warning is issued.
    /// </summary>
    public static T Call<T>(T t)
    {
        return t;
    }

    // Significant Digits Rounding in Vector
    public static Vector3 Round(this Vector3 vector3, int decimalPlaces)
    {
        float multiplier = Mathf.Pow(10f, decimalPlaces);
        return new Vector3(Mathf.Round(vector3.x * multiplier) / multiplier, Mathf.Round(vector3.y * multiplier) / multiplier, Mathf.Round(vector3.z * multiplier) / multiplier);
    }

    public static int FindIndex<T>(this T[] source,Predicate<T> match)
    {
        if (source == null) return -1;
        return Array.FindIndex<T>(source, match);
    }
    public static int IndexOf<T>(this T[] source,T value)
    {
        if (source == null) return -1;
        return Array.IndexOf(source, value);
    }

    // Changes the layer.
    public static void setLayer(this Transform t ,int layer,bool includeChildren = true)
    {
        t.gameObject.layer = layer;
        if (includeChildren == false) return;

        int count = t.childCount;
        for(int i = 0; i < count; ++i)
        {
            Transform child = t.GetChild(i);
            child.gameObject.layer = layer;

            if (child.childCount != 0) setLayer(child, layer, includeChildren);
        }
    }

    public static List<T> Unique<T>(this IList<T> source,Func<T, T, bool> match)
    {
        if (source == null) return null;
        List<T> uniques = new List<T>();
        foreach(T item in source)
        {
            int idx = uniques.FindIndex(x => match(item, x));
            if (idx == -1) uniques.Add(item);
        }
        return uniques;
    }

    public static void Shuffle<T>(this List<T> source,System.Random rand = null)
    {
        if (rand == null) rand = new System.Random();
        for (var i = 0; i < source.Count; ++i)
            Swap(source, i, rand.Next(i, source.Count));
    }

    public static void Swap<T>(List<T> source,int i, int j)
    {
        var temp = source[i];
        source[i] = source[j];
        source[j] = temp;
    }

    // size setting
    public static void setSize(this RectTransform rt,Vector2 newSize)
    {
        var pivot = rt.pivot;
        var dist = newSize - rt.rect.size;
        rt.offsetMin = rt.offsetMin - new Vector2(dist.x * pivot.x, dist.y * pivot.y);
        rt.offsetMax = rt.offsetMax + new Vector2(dist.x * (1f - pivot.x), dist.y * (1 - pivot.y));
    }

    class FloatComparer : IEqualityComparer<float>
    {
        bool IEqualityComparer<float>.Equals(float x, float y)
        {
            return x == y;
        }
        int IEqualityComparer<float>.GetHashCode(float obj)
        {
            return obj.GetHashCode();
        }
    }
    public static readonly WaitForEndOfFrame  WaitForEndOfFrame  = new WaitForEndOfFrame();
    public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();
    private static readonly Dictionary<float, WaitForSeconds> _timeInterval = new Dictionary<float, WaitForSeconds>(new FloatComparer());

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        if (!_timeInterval.TryGetValue(seconds, out WaitForSeconds wfs))
        {
            _timeInterval.Add(seconds, wfs = new WaitForSeconds(seconds));
        }
        return wfs;
    }
}

