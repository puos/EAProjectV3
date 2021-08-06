using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class EASceneLoadingManager : Singleton<EASceneLoadingManager>
{
    public static string prevSceneName { get; private set; }
  
    public Action<float> OnActLoading = null;
    public Action OnActLoadingComplete = null;
    public Action OnActReady = null;

    private AsyncOperation m_TaskLoad = null;
    private AsyncOperation m_TaskUnLoad = null;

    private UIManager m_uiMgr = null;

    private bool m_bProcRunning = false;
    private bool m_bLoadingComplete = false;
    private string m_strNextSceneName = string.Empty;
 
    public override GameObject GetSingletonParent()
    {
        return EAMainFrame.instance.gameObject;
    }
    protected override void Initialize()
    {
        base.Initialize();
        EAMainFrame.instance.OnMainFrameFacilityCreated(MainFrameAddFlags.SceneMgr);
    }
    // Check for self loading
    public static bool IsSelfLoading()
    {
        return string.IsNullOrEmpty(prevSceneName);
    }
    protected override void Close()
    {
        OnActLoading = null;
        OnActLoadingComplete = null;
        OnActReady = null;
    }
    public void SetNextScene(string sceneType,bool isWait = false)
    {
        if (m_uiMgr == null) m_uiMgr = UIManager.instance;

        prevSceneName = m_strNextSceneName;
        m_strNextSceneName = sceneType;

        StartCoroutine(CoProc(isWait));
    }
    public void SetReady()
    {
        if (m_TaskLoad == null) return;
        if (m_TaskLoad.allowSceneActivation == false) m_TaskLoad.allowSceneActivation = true;
    }
    IEnumerator CoProc(bool isWait)
    {
        while (m_bProcRunning)
            yield return new WaitForEndOfFrame();

        m_bProcRunning = true;
        m_uiMgr.Clear();

        if(EASceneLogic.instance != null) EASceneLogic.instance.Destroy();
        EACObjManager.instance.Destroy();
        EA_ItemManager.instance.Destroy();
        GameResourceManager.instance.Clear();
        
        GC.Collect();

        m_TaskLoad = SceneManager.LoadSceneAsync(m_strNextSceneName);
        if(isWait == true) m_TaskLoad.allowSceneActivation = false;

        yield return CoSceneLoading();
        yield return CoSceneUnloading();

        if (OnActReady != null) OnActReady.Invoke();

        yield return new WaitForEndOfFrame();

        m_TaskLoad = null;
        m_TaskUnLoad = null;
        m_bProcRunning = false;
        m_bLoadingComplete = false;

        if (EASceneLogic.instance != null) EASceneLogic.instance.Init();
    }

    IEnumerator CoSceneLoading()
    {
        while(!m_TaskLoad.isDone)
        {
            yield return new WaitForEndOfFrame();

            if (OnActLoading != null) 
                OnActLoading.Invoke(m_TaskLoad.progress);

            if(!m_bLoadingComplete && m_TaskLoad.progress >= 0.9f)
            {
                m_bLoadingComplete = true;
                if (OnActLoadingComplete != null) 
                    OnActLoadingComplete.Invoke();
            }
        }

        if (OnActLoading != null)
            OnActLoading.Invoke(m_TaskLoad.progress);
    }

    IEnumerator CoSceneUnloading() 
    {
        m_TaskUnLoad = Resources.UnloadUnusedAssets();

        while (!m_TaskUnLoad.isDone)
            yield return new WaitForEndOfFrame();
    }
}
