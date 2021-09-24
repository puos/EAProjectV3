using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[EASceneInfo(typeof(AnimTest))]
public class AnimTest : EASceneLogic
{
    private CTestActor actor = null;
    protected override void OnClose()
    {
        actor.Release();
    }

    protected override void OnInit()
    {
        actor = CTestActor.Clone();
    }

    protected override IEnumerator OnPostInit()
    {
        yield return null;
    }

    protected override void OnUpdate()
    {
    }

    public void OnGUI()
    {
        GUILayoutOption w = GUILayout.Width(160);
        GUILayoutOption h = GUILayout.Height(50);

        if(GUILayout.Button("run ani",w,h))
        {
            actor.PushAnimation("Move");
        }

        if(GUILayout.Button("idle ani",w,h))
        {
            actor.PushAnimation("Idle");
        }

        if (GUILayout.Button("stop ani", w, h))
        {
            actor.StopAnimation();
        }
    }
}
