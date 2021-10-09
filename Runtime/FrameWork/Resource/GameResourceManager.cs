using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public struct EResourceType
{
    private int id;
    private string resourcePath;

    public EResourceType(EResourceType e)
    {
        id = e.id;
        resourcePath = e.resourcePath;
    }

    public EResourceType(int id,string resourcePath)
    {
        this.id = id;
        this.resourcePath = resourcePath;
    }
    public static implicit operator int(EResourceType e) => e.id;
    public override string ToString() { return resourcePath; }
}

public struct EAtlasType
{
    private int id;
    private string atlasPath;

    public EAtlasType(EAtlasType e)
    {
        this.id = e.id;
        this.atlasPath = e.atlasPath;
    }
    public static implicit operator int(EAtlasType e) => e.id;
    public override string ToString() { return atlasPath; }
}


public class GameResourceManager : Singleton<GameResourceManager>
{
    private Transform m_tRootObjectPool = null;
    private Dictionary<int, Queue<GameObject>> objectPool = new Dictionary<int, Queue<GameObject>>();
    private Dictionary<int, SpriteAtlas> m_dicAtlas = new Dictionary<int, SpriteAtlas>();
    private Dictionary<int, Sprite> m_dicCachedSprite = new Dictionary<int, Sprite>();
    private Dictionary<int, Dictionary<string,Object>> m_dicCachedObject = new Dictionary<int, Dictionary<string, Object>>();

    public override GameObject GetSingletonParent()
    {
        return EAMainFrame.instance.gameObject;
    }

    protected override void Initialize()
    {
        base.Initialize();
        EAMainFrame.instance.OnMainFrameFacilityCreated(MainFrameAddFlags.ResourceMgr);
    }

    protected override void Close() 
    {
        m_dicAtlas.Clear();
        m_dicCachedSprite.Clear();
        objectPool.Clear();

        var it = m_dicCachedObject.GetEnumerator();

        while(it.MoveNext())
        {
            it.Current.Value.Clear();
        }

        m_dicCachedObject.Clear();
    }

    public void Clear()
    {
        objectPool.Clear();
        if (null != m_tRootObjectPool)
            m_tRootObjectPool.DestroyChildren(false);

        m_dicAtlas.Clear();
        m_dicCachedSprite.Clear();

        var it = m_dicCachedObject.GetEnumerator();

        while (it.MoveNext())
        {
            it.Current.Value.Clear();
        }

        m_dicCachedObject.Clear();
    }

    public T Load<T>(string path) where T : Object
    {
        T obj = null;
        obj = (T)Resources.Load<T>(path);
        Debug.Assert(obj != null, "resource Load is null path : " + path);
        return obj;
    }

    public IEnumerator LoadAsync<T>(string path,System.Action<T> callBack) where T : UnityEngine.Object
    {
        ResourceRequest req = Resources.LoadAsync<T>(path);
        yield return req;
        if (req.asset) callBack(req.asset as T);
        if (req.asset) yield break;
    }

    public GameObject Create(EResourceType eType , string strPrefName)
    {
        if (null == m_tRootObjectPool)
        {
            GameObject goPool = new GameObject("ObjectPool");
            m_tRootObjectPool = goPool.transform;
            m_tRootObjectPool.position = Vector3.zero;
        }

        var key = CRC32.GetHashForAnsi(strPrefName);
        if (!objectPool.TryGetValue(key,out Queue<GameObject> value))
            objectPool.Add(key, new Queue<GameObject>());

        GameObject go = null;

        if(0 < objectPool[key].Count)
        {
            go = objectPool[key].Dequeue();
            if (go != null) go.transform.SetParent(null);
        }

        if (go != null)
        {
            go.SetActive(true);
            return go;
        }
              
        GameObject goPref = LoadPrefabs(eType, strPrefName);
        if (goPref == null) return null;
        
        go = Instantiate(goPref) as GameObject;
        go.name = strPrefName;
        if(go.activeSelf == false) go.SetActive(true);
        
        return go;
    }

    public void ReleaseObject(GameObject go, bool bInsertPool = true)
    {
        if (null == go) return;
        if (string.IsNullOrEmpty(go.name)) return;

        try 
        {
            if(bInsertPool)
            {
                var key = CRC32.GetHashForAnsi(go.name);
                if (!objectPool.TryGetValue(key, out Queue<GameObject> value))
                    objectPool.Add(key, new Queue<GameObject>());

                go.SetActive(false);

                objectPool[key].Enqueue(go);
                go.transform.SetParent(m_tRootObjectPool);
            }
            else
            {
                go.transform.SetParent(null);
                Destroy(go);
            }
        }
        catch(System.Exception e)
        {
            Debug.LogError("Exception GameObject Name : " + go.name);
            Debug.LogError(e);
        }
    }

    private GameObject LoadPrefabs(EResourceType eType, string strPrefName)
    {
        if (!m_dicCachedObject.TryGetValue(eType, out Dictionary<string, Object> value))
            m_dicCachedObject.Add(eType, new Dictionary<string, Object>());

        if(!m_dicCachedObject[eType].TryGetValue(strPrefName,out Object goPref))
        {
            string strPath = $"{eType}/{strPrefName}";
            goPref = Load<GameObject>(strPath);
            if (null == goPref) 
            {
                Debug.LogError("Resource Path Error == Type : " + eType + " Path : " + strPath);
                return null; 
            }
            m_dicCachedObject[eType].Add(strPrefName, goPref);
        }

        return goPref as GameObject;
    }

    private SpriteAtlas GetAtlas(EAtlasType eAtlas)
    {
        if (m_dicAtlas.TryGetValue(eAtlas, out SpriteAtlas value))
            return value;

        string strPath = $"{eAtlas}";
        SpriteAtlas atlas = Load<SpriteAtlas>(strPath);
        if (atlas != null) m_dicAtlas.Add((int)eAtlas, atlas);

        return atlas;
    }

    public Sprite LoadSprite(EAtlasType eAtlas,string strSpriteName)
    {
        var nKey = CRC32.GetHashForAnsi(strSpriteName);
        if (m_dicCachedSprite.TryGetValue(nKey, out Sprite value))
            return value;

        SpriteAtlas atlas = GetAtlas(eAtlas);
        if (atlas == null) return null;

        Sprite spr = atlas.GetSprite(strSpriteName);
        if (null != spr) m_dicCachedSprite.Add(nKey, spr);
        return spr;

    }

    public AudioClip LoadAudioClip(string strAudioName)
    {
        string eType = "audios";
        var nKey = CRC32.GetHashForAnsi(eType);
        if (!m_dicCachedObject.TryGetValue(nKey, out Dictionary<string, Object> value))
            m_dicCachedObject.Add(nKey, new Dictionary<string, Object>());

        if (!m_dicCachedObject[nKey].TryGetValue(strAudioName, out Object goPref))
        {
            string strPath = $"{eType}/{strAudioName}";
            goPref = Load<AudioClip>(strPath);
            if (null == goPref)
            {
                Debug.LogError("Resource Path Error == Type : " + eType + " Path : " + strPath);
                return null;
            }
            m_dicCachedObject[nKey].Add(strAudioName, goPref);
        }
        return goPref as AudioClip;
    }
    
    public Texture LoadTexture(string strTextureName)
    {
        string eType = "Textures";
        var nKey = CRC32.GetHashForAnsi(eType);
        if (!m_dicCachedObject.TryGetValue(nKey, out Dictionary<string, Object> value))
            m_dicCachedObject.Add(nKey, new Dictionary<string, Object>());

        if (!m_dicCachedObject[nKey].TryGetValue(strTextureName, out Object goPref))
        {
            string strPath = $"{eType}/{strTextureName}";
            goPref = Load<Texture>(strPath);
            if (null == goPref)
            {
                Debug.LogError("Resource Path Error == Type : " + eType + " Path : " + strPath);
                return null;
            }
            m_dicCachedObject[nKey].Add(strTextureName, goPref);
        }
        return goPref as Texture;
    }

    public Material LoadMaterial(string strMaterialName)
    {
        string eType = "materials";
        var nKey = CRC32.GetHashForAnsi(eType);
        if (!m_dicCachedObject.TryGetValue(nKey, out Dictionary<string, Object> value))
            m_dicCachedObject.Add(nKey, new Dictionary<string, Object>());

        if (!m_dicCachedObject[nKey].TryGetValue(strMaterialName, out Object goPref))
        {
            string strPath = $"{eType}/{strMaterialName}";
            goPref = Load<Material>(strPath);
            if (null == goPref)
            {
                Debug.LogError("Resource Path Error == Type : " + eType + " Path : " + strPath);
                return null;
            }
            m_dicCachedObject[nKey].Add(strMaterialName, goPref);
        }
        return goPref as Material;
    }

    public EAObject CreateEAObject(EResourceType eType ,System.Type objClassType, string strPrefName)
    {
        GameObject eaObj = Create(eType, strPrefName);
      
        if (eaObj == null) return null;

        // Modify class creation logic [3/30/2018 puos]
        if (!eaObj.gameObject.TryGetComponent(objClassType, out Component comp))
            eaObj.gameObject.AddComponent(objClassType);

        EAObject obj = eaObj.GetComponent<EAObject>();
        if (obj != null) obj.Initialize();
        return obj;
    }

    public void ReleaseEAObject(EAObject go)
    {
        ReleaseObject(go.gameObject);
    }


}
