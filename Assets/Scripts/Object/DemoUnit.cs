using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoUnit : EAActor
{
    public override void InitializeAI()
    {
        base.InitializeAI();

        AddAgent("demoUnit");
    }

    public override void UpdateAI()
    {
        base.UpdateAI();
        steering.LookWhereYourGoing();
    }
   
    public static DemoUnit Clone()
    {
        ObjectInfo objectInfo = new ObjectInfo()
        {
            m_ModelTypeIndex = nameof(DemoUnit),
            m_objClassType = typeof(DemoUnit)
        };

        NPCInfo nPCInfo = new NPCInfo();
        EA_CCharNPC npc = EACObjManager.instance.CreateNPC(objectInfo, nPCInfo);
        return npc.GetLinkIActor() as DemoUnit;
    }
}
