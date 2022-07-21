using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoUnit : EAActor
{
    private UserCam userCam = null;
    public override void Initialize()
    {
        base.Initialize();
        userCam = GameResourceManager.instance.Create(EResourceGroup.Object, nameof(UserCam)).GetComponent<UserCam>();
        userCam.Initialize();
        userCam.SetFollow(tr);
    }

    public override void Release()
    {
        base.Release();
    }

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

    public void SwitchCam()
    {
        if (userCam) userCam.SwitchCam();
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
