using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EASceneLogic : MonoBehaviour 
{
    public static EASceneLogic instance;
    
    protected enum SceneLoadingState
    {
        None,
        Inited,
        PostInited,
        DoUpdate,
        WillDestroy,
    }

    protected SceneLoadingState sceneLoadingState { get; private set; }

    public string prevSceneName { get; private set; }

    public string sceneName { get; private set; }

    public System.Action onSceneDestroy = null;

    bool selfLoading = false;

    protected EAEventManager m_eventMgr = null;
    protected EAUIManager      m_uiMgr = null;
    protected EASfxManager m_sfxMgr = null;
    protected EASceneLoadingManager m_sceneMgr = null;

    abstract protected void OnInit();

    protected virtual IEnumerator OnPostInit() => null;

    abstract protected void OnUpdate();

    abstract protected void OnClose();

    protected virtual void OnLazyUpdate(EAMainFrame.LazyUpdateType type) { }

    public void Initialize()
    {
        instance = this;
        prevSceneName = EASceneLoadingManager.prevSceneName;
        sceneLoadingState = SceneLoadingState.None;
        m_eventMgr = EAEventManager.instance;
        m_uiMgr = EAUIManager.instance;
        m_sceneMgr = EASceneLoadingManager.instance;
        m_sfxMgr = EASfxManager.instance;
    }

    public void Init()
    {
        if (sceneLoadingState >= SceneLoadingState.Inited) return;
        
        sceneLoadingState = SceneLoadingState.Inited;

        EAMainFrame.instance.OnSceneLoaded();

        Debug.Log("EASceneLogic.Init");

        OnInit();
    }

    public void Destroy()
    {
       if (!EAMainFrame.isApplicationQuit) EAMainFrame.onSceneWillChange();

        OnClose();

        gameObject.transform.SetParent(null);
        GameObject.Destroy(gameObject);

        if (onSceneDestroy != null) onSceneDestroy();

        instance = null;

        Debug.Log("EASceneLogic.Destroy");
    }

    public IEnumerator DoPostInit()
    {
        if (sceneLoadingState == SceneLoadingState.PostInited) 
            yield break;

        sceneLoadingState = SceneLoadingState.PostInited;

        Debug.Log("EASceneLogic.DoPostInit");

        yield return OnPostInit();

        sceneLoadingState = SceneLoadingState.DoUpdate;
    }

    public void OnPostUpdate() 
    {
        //None -> Inited -> PostInited
        if (sceneLoadingState == SceneLoadingState.None)
        {
            // If there is no previous scene, it is judged as a single scene call
            selfLoading = EASceneLoadingManager.IsSelfLoading();
            if(selfLoading) Init();
        }
        else if(sceneLoadingState == SceneLoadingState.Inited)
        {
            StartCoroutine(DoPostInit());
        }
        else if(sceneLoadingState == SceneLoadingState.DoUpdate)
        {
            OnUpdate();
        }
    }

    // run in mainframe
    public void SceneLogicOnLazyUpdate(EAMainFrame.LazyUpdateType lazyType)
    {
        if (sceneLoadingState != SceneLoadingState.PostInited)
            return;

        OnLazyUpdate(lazyType);
    }

    protected virtual bool OnEscapeKey() {  return false; }

    public bool HandleEscapeKey()
    {
        if (sceneLoadingState != SceneLoadingState.PostInited) 
            return true;

        return OnEscapeKey();
    }

    public void WillDestroy()
    {
        sceneLoadingState = SceneLoadingState.WillDestroy;
    }
}
