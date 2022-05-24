using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[EASceneInfo(typeof(BridgeSceneTest))]
#endif

public class BridgeSceneTest : EASceneLogic
{
    protected override void OnClose()
    {
        Debug.Log("BridgeSceneTest OnClose");
    }

    protected override void OnInit()
    {
        Debug.Log("BridgeSceneTest OnInit");
    }

    protected override void OnUpdate()
    {
       
    }
}
