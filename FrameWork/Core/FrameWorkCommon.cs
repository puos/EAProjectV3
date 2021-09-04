using System;
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
}

