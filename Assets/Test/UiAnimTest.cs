using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[EASceneInfo(typeof(UiAnimTest))]
#endif

public class UiAnimTest : EASceneLogic
{
    protected override void OnClose()
    {
       
    }

    protected override void OnInit()
    {
        m_uiMgr.OpenPage<TestUI>(CustomUIPage.uianimUI);
    }

    protected override void OnUpdate()
    {
       
    }
}
