using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class EAScene : MonoBehaviour
{
    public string controllerClassType;
    
    public static EAScene instance { get; private set; }

    protected static Dictionary<string, System.Type> sceneInfo = new Dictionary<string, System.Type>();

#if UNITY_EDITOR
    [HideInInspector] public MonoScript script = null;
#endif

    void Awake() 
    {
        if(instance != null && !object.ReferenceEquals(this,instance))
        {
            DestroyImmediate(gameObject);
            return;
        }

        instance = this;

        EAMainFrame mainframe = FindObjectOfType<EAMainFrame>();

        // When the first scene runs, it creates a MainFrame.
        if (mainframe == null)
        {
            mainframe = EAMainframeUtil.CreateMainFrameTree();
            Debug.Log("Create Mainframe Tree");
        }

        AudioListener audioListener = FindObjectOfType<AudioListener>();
        if (audioListener == null) mainframe.gameObject.AddComponent<AudioListener>();

        StandaloneInputModule inputModule = FindObjectOfType<StandaloneInputModule>();
        if (inputModule == null) EAFrameUtil.AddChild<StandaloneInputModule>(mainframe.gameObject);

        EAMainFrame.onUpdate.Remove(OnUpdate);
        EAMainFrame.onUpdate.Add(OnUpdate);

        Initialize();

        if (EASceneLoadingManager.IsSelfLoading())
        {
            OnSetting();
        }
    }

    protected virtual void Initialize()
    {
        if (sceneInfo.Count <= 0)
        {
            InitializeSceneInfo();
        }
    }

    protected virtual void OnSetting()
    {
    }

    public void InitializeSceneInfo() 
    {
        var types = from a in AppDomain.CurrentDomain.GetAssemblies()
                    from t in a.GetTypes()
                    where t.IsDefined(typeof(EASceneInfoAttribute), false)
                    select t;

        foreach(var type in types)
        {
            var attrs = type.GetCustomAttributes(typeof(EASceneInfoAttribute), false);
            if (attrs == null) continue;
            var attr = attrs[0] as EASceneInfoAttribute;
            sceneInfo.Add(attr.className, attr.classType);
        }
    }

    private void CreateSceneLogic()
    {
        sceneInfo.TryGetValue(controllerClassType, out Type t);
        EASceneLogic sm = (EASceneLogic)EAFrameUtil.AddChild(EAMainFrame.instance.gameObject,t,"gameLogic") as EASceneLogic;
        sm.Initialize();
        string info = (sm == null) ? "null" : "valid";
        Debug.Log($"EA SceneConfig.CreateSceneLogic sm is {info} controller class type : {controllerClassType}");
    }

    private void OnUpdate() 
    {
        if (!EAMainFrame.instance.IsEAMainFrameInitReady()) return;
        
        CreateSceneLogic();
        EAMainFrame.onUpdate.Remove(OnUpdate);
    }
}
