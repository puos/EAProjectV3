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
    public float screenX = 1280f;
    public float screenY = 720f;
            
    public static EAScene instance { get; private set; }

    protected static Dictionary<string, System.Type> sceneInfo = new Dictionary<string, System.Type>();

    void Awake() 
    {
        if(instance != null && !object.ReferenceEquals(this,instance))
        {
            DestroyImmediate(gameObject);
            return;
        }

        instance = this;

        EAMainFrame.onUpdate.Remove(OnUpdate);
        EAMainFrame.onUpdate.Add(OnUpdate);

        Initialize();

        if (EASceneLoadingManager.IsSelfLoading())
        {
            OnSetting();

            // When the first scene runs, it creates a MainFrame.
            EAMainFrame mainframe = EAMainframeUtil.CreateMainFrameTree();

            EAMainFrame.SetRefResolution(screenX, screenY);

            Debug.Log("Create Mainframe Tree");

            AudioListener[] audioListener = FindObjectsOfType<AudioListener>();
            for(int i = 0; i < audioListener.Length; ++i)
            {
                if (i == 0) continue;
                audioListener[i].enabled = false;
            } 
            
            StandaloneInputModule[] inputModule = FindObjectsOfType<StandaloneInputModule>();
            for(int i = 0; i < inputModule.Length;++i)
            {
                if (i == 0) continue;
                inputModule[i].enabled = false;
            }

            
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
        var sm = GetComponent<EASceneLogic>();

        if (sm == null)
            return;

        var logicType = sm.GetType().ToString();

        bool scenceCheck = sceneInfo.TryGetValue(logicType , out Type t);

        if (!scenceCheck)
            return;

        EAFrameUtil.AddChild(EAMainFrame.instance.gameObject, 
                             sm.gameObject , false);
        sm.Initialize();

        Debug.Log($"EA SceneConfig.CreateSceneLogic class type : {logicType}");
    }

    private void OnUpdate() 
    {
        if (!EAMainFrame.instance.IsEAMainFrameInitReady()) return;
        
        CreateSceneLogic();
        EAMainFrame.onUpdate.Remove(OnUpdate);
    }
}
