using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[EASceneInfo(typeof(SceneTest1))]
#endif

public class SceneTest1 : EASceneLogic
{
    protected override void OnClose()
    {
        
    }

    protected override void OnInit()
    {
        
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
            m_sceneMgr.SetNextScene("SceneTest2","Empty");
        }
    }
}
