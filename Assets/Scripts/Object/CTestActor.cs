using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTestActor : EAActor
{
    private EAActorAnim actorAnim = null;

    public override void Initialize()
    {
        base.Initialize();

        actorMover.AIOn = false;
        
        actorAnim = GetComponent<EAActorAnim>();
        actorAnim.Initialize();
        actorAnim.eventCallback = (EAActorAnim anim, AnimationEventType eventType, string slotName, string slotValue) =>
        {
            int.TryParse(slotName, out int value);

            if (value == AnimatorStateId.Idle) Debug.Log($"Idle {slotValue}");
            if (value == AnimatorStateId.Run) Debug.Log($"Run {slotValue}");

            Debug.Log($"eventType : {eventType} slotName : {slotName} ");        
        };
    }

    public override void Release()
    {
        base.Release();
    }

    public void PushAnimation(string aniName)
    {
        if (actorAnim != null) actorAnim.PushAnimation(aniName);
    }

    public void StopAnimation()
    {
        if (actorAnim != null) actorAnim.StopAnimation();
    }

    public static CTestActor Clone() 
    {
        ObjectInfo objectInfo = new ObjectInfo()
        {
            m_ModelTypeIndex = "Character/Amon/Amon",
            m_objClassType   = typeof(CTestActor)
        };

        NPCInfo nPCInfo = new NPCInfo();
        EA_CCharNPC npc = EACObjManager.instance.CreateNPC(objectInfo, nPCInfo);

        return npc.GetLinkIActor() as CTestActor;
    }
}
