using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    protected static T m_instance;

    /// <summary>
    ///  DontDestroyOnLoad is not used if there is a parent object.
    /// </summary>
    public virtual GameObject GetSingletonParent() 
    {
        return null;
    }

    public static T instance
    {
        get
        {
            if(m_instance == null) CreateInstance();
            return m_instance;
        }
    }

    public static void CreateInstance()
    {
        if (m_instance)
            return;

        GameObject go = new GameObject(typeof(T).Name, typeof(T));
        m_instance = go.GetComponent<T>();
        m_instance.Initialize();

        GameObject parentGo = m_instance.GetSingletonParent();

        if (parentGo != null) go.transform.parent = parentGo.transform;
        else DontDestroyOnLoad(go);
    }

    protected virtual void Initialize() 
    {        
    }

    protected virtual void Close()
    {

    }

    protected void OnDestroy()
    {
        Close();
    }
}
