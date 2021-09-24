using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTestActor : EAActor
{
    private EAActorAnim actorAnim = null;

    public override void Initialize()
    {
        base.Initialize();

        actorAnim = GetComponent<EAActorAnim>();
    }

    public override void Release()
    {
        base.Release();
    }

    public void PushAnimation(string aniName)
    {
        if (actorAnim != null) actorAnim.PushAnimation(aniName);
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
