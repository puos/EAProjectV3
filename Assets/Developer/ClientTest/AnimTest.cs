using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[EASceneInfo(typeof(AnimTest))]
public class AnimTest : EASceneLogic
{
    private CTestActor actor = null;
    private string modelIndex;
    protected override void OnClose()
    {
        actor.Release();
    }

    protected override void OnInit()
    {
        actor = CTestActor.Clone(modelIndex);
        EAFrameUtil.Call<EAFPSCounter>(EAFPSCounter.instance);
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

        if(GUILayout.Button("attack ani", w, h))
        {
            actor.PushAnimation("Attack");
        } 

        if (GUILayout.Button("stop ani", w, h))
        {
            actor.StopAnimation();
        }
    }
}
