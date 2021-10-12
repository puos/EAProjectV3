using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Flags]
public enum MainFrameAddFlags
{
    UiManager   =  1 << 1,
    ClockMgr    =  1 << 2,
    ResourceMgr =  1 << 3,
    SceneMgr    =  1 << 4,
    SoundMgr    =  1 << 5, 

    // add above
    AllFlags = ResourceMgr | SceneMgr  | UiManager | SoundMgr,
}


public class EAMainFrame : Singleton<EAMainFrame>
{
    [Flags]
    public enum LazyUpdateType
    {
        Every60s = 1 << 0,
        Every10s = 1 << 1,
        Every5s = 1 << 2,
        Every1s = 1 << 3,
        Every25ms = 1 << 4,
        Every50ms = 1 << 5,
        Every100ms = 1 << 6,
        Every500ms = 1 << 7,
    }

    public bool postInitCall { get; private set; }

    public bool started { get; private set; }

    public static bool isApplicationQuit { get; private set; }

    public static float screenX { get; private set; }

    public static float screenY { get; private set; }

    public static IEADataManager iDataManager { get; set; } 

    MainFrameAddFlags facilityCreatedFlags = 0;
    
    private float nextLazyUpdateCheckTime = 0;
    long lazyUpdateSeq = 0;

    // Lazy' updates to reduce load
    public delegate void OnLazyUpdate(LazyUpdateType type);
    public static List<OnLazyUpdate> onLazyUpdate = new List<OnLazyUpdate>();

    public delegate void OnUpdate();
    public static List<OnUpdate> onUpdate = new List<OnUpdate>();

    public delegate void OnSceneLoad();
    public static OnSceneLoad onSceneLoad = delegate () { };

    // Application released in paused state
    public static Action onApplicationResumed = delegate () { };

    public delegate void OnSceneWillChange();
    public static OnSceneWillChange onSceneWillChange = delegate () { };

    protected void Start()
    {
        started = true;

        if(!postInitCall)
            throw new Exception("Do not initialize MainFrame yet. see MainFrame.OnMainFrameFacilityCreated()");
    }
    
    protected override void Close()
    {
        base.Close();

        onUpdate.Clear();
        onLazyUpdate.Clear();

        Debug.Assert(m_instance != null);
    }

    // Update is called once per frame
    private void Update()
    {
        if (EASceneLogic.instance != null) 
            EASceneLogic.instance.OnPostUpdate();

        for (int i = 0; i < onUpdate.Count; ++i) 
            if (onUpdate[i] != null) onUpdate[i]();

        if (nextLazyUpdateCheckTime < Time.time)
        {
            nextLazyUpdateCheckTime += 0.025f; // Starting at least 25ms

            if (nextLazyUpdateCheckTime < Time.time) 
                nextLazyUpdateCheckTime = Time.time;

            lazyUpdateSeq++;

            LazyUpdateType type = LazyUpdateType.Every25ms;

            if (lazyUpdateSeq % 2 == 0)
                type |= LazyUpdateType.Every50ms;
            if (lazyUpdateSeq % 4 == 0)
                type |= LazyUpdateType.Every100ms;
            if (lazyUpdateSeq % 20 == 0)
                type |= LazyUpdateType.Every500ms;
            if (lazyUpdateSeq % 40 == 0)
                type |= LazyUpdateType.Every1s;
            if (lazyUpdateSeq % 200 == 0)
                type |= LazyUpdateType.Every5s;
            if (lazyUpdateSeq % 400 == 0)
                type |= LazyUpdateType.Every10s;
            if (lazyUpdateSeq % 2400 == 0)
                type |= LazyUpdateType.Every60s;

            for (int i = 0; i < onLazyUpdate.Count; ++i) 
                if (onLazyUpdate[i] != null) onLazyUpdate[i](type);

            if (EASceneLogic.instance != null) 
                EASceneLogic.instance.SceneLogicOnLazyUpdate(type);
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
           if(onApplicationResumed != null) onApplicationResumed();
        }
    }

    private void OnApplicationQuit()
    {
        isApplicationQuit = true;

        if (EASceneLogic.instance != null) EASceneLogic.instance.Destroy();

        Debug.Log("EAMainframe.OnApplicationQuit");
    }

    public void OnSceneLoaded()
    {
         if (onSceneLoad != null) onSceneLoad();

        Debug.Log("EAMainframe.OnSceneLoaded");
    }

    public void QuitApplication()
    {
        Debug.Log("EAMainFrame.QuitApplication");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID
        using (AndroidJavaClass javaSystemClass = new AndroidJavaClass("java.lang.System"))
        {
            javaSystemClass.CallStatic("exit", 0);
        } 
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// Called after subframe managers of MainFrame are initialized
    /// </summary>
    public void PostInit() 
    {
        if (Application.isPlaying)
            Debug.Assert(IsEAMainFrameInitReady());

        postInitCall = true;
    }

    public bool IsEAMainFrameInitReady() 
    {
        return facilityCreatedFlags == MainFrameAddFlags.AllFlags;
    }

    public void OnMainFrameFacilityCreated(MainFrameAddFlags flag)
    {
        // In the case of instant test, this is the code to determine when all GameObjects of MainFrame are awakened because MainFrame is already created.
        facilityCreatedFlags |= flag;
       
        // When all MainFrame devices are awake, we immediately proceed with PostInit () of MainFrame.
        if (IsEAMainFrameInitReady())
        {
            if (!postInitCall) PostInit();
        }
    }

    void HandleInput()
    {
        if(Input.GetKeyUp("escape"))
        {
            HandleEscapeKey();
        }
    }

    public void HandleEscapeKey() 
    {
        if (EASceneLogic.instance != null) EASceneLogic.instance.HandleEscapeKey();
    }

    public static void SetRefResolution(float width,float height)
    {
        screenX = width;
        screenY = height;
    }

    public static void ApplyDeviceOption(int level = 0,int frameRateType = 3, bool keepScreenOn = false)
    {
        float[] resolution = new float[] { 0 , 1280, 1500, 1920 };

        level = Math.Min(level, resolution.Length - 1);

        if(level > 0)
        {
            float windowX = resolution[level];
            float windowY = (Screen.height / (float)Screen.width) * windowX;
            Screen.SetResolution((int)windowX, (int)windowY, true);
        }

        ApplyFrameRate(frameRateType);

        Screen.sleepTimeout = keepScreenOn ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;

        if (Application.isMobilePlatform) Application.runInBackground = false;
    }

    static void ApplyFrameRate(int frameRateType)
    {
        switch(frameRateType)
        {
            case 0:
                Application.targetFrameRate = 20;
                QualitySettings.vSyncCount = Application.isMobilePlatform ? 0 : 0;
                break;

            case 1:
                Application.targetFrameRate = 30;
                QualitySettings.vSyncCount = Application.isMobilePlatform ? 1 : 0;
                break;

            case 2:
                Application.targetFrameRate = 40;
                QualitySettings.vSyncCount = Application.isMobilePlatform ? 1 : 0;
                break;

            case 3:
                Application.targetFrameRate = 60;
                QualitySettings.vSyncCount = Application.isMobilePlatform ? 1 : 0;
                break;
        }

        Debug.Log("FrameRate:" + Application.targetFrameRate + " IsMobile : " + Application.isMobilePlatform);
    }
}
