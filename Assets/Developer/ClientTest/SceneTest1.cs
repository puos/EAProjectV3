using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[EASceneInfo(typeof(SceneTest1))]
#endif

public class SceneTest1 : EASceneLogic
{
    private string[] mixPaths = new string[]
    {
        "Master/CV/Voice",
        "Master/SFX/UI",
        "Master/Main/BGM",
        "Master/Main/Movie"
    };

    protected override void OnClose()
    {
        
    }

    protected override void OnInit()
    {
        if(string.IsNullOrEmpty(prevSceneName))
        {
            OptionManager.instance.SetOptionValue(EASoundManager.bgmVolume, 100);
            OptionManager.instance.SetOptionValue(EASoundManager.sfxVolume, 100);
            EASoundManager.instance.LoadBGM();
            EASoundManager.instance.LoadMixerGroup("Sound/mix", mixPaths);
        }

        EASoundManager.instance.PlayBGM("testBGM");
        
    }

    protected override void OnUpdate()
    {
        
    }

    private void OnGUI()
    {
        GUILayoutOption w = GUILayout.Width(180.0f);
        GUILayoutOption h = GUILayout.Height(40f);

        if(GUILayout.Button("NextScene",w,h))
        {
            EASoundManager.instance.ChangeMixSnapShot("SceneTest2");
            m_sceneMgr.SetNextScene("SceneTest2","Empty");
        }

        int v = OptionManager.instance.Get(EASoundManager.bgmVolume);
        float fv = GUILayout.HorizontalScrollbar(v, 10f, 0f, 100f);
        OptionManager.instance.SetOptionValue(EASoundManager.bgmVolume, (int)fv);
    }
}
