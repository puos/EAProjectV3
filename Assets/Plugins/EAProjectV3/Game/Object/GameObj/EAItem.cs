using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EAObjID = System.UInt32;

public class EAItem : EAObject
{
    protected EA_CItem m_ItemBase = null;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Release()
    {
        base.Release();
    }

    public virtual bool Use()
    {
        return true;
    }

    public void SetItemBase(EA_CItem pItemBase) { m_ItemBase = pItemBase; }
    public EA_CItem GetItemBase() { return m_ItemBase;  }
    public  EAObject GetOwner()
    {
        if (GetItemBase() == null) return null;

        EAObjID objID = GetItemBase().GetItemInfo().m_HavenUser;
        EA_CCharBPlayer pActor = EACObjManager.instance.GetActor(objID);
        if (pActor != null) return pActor.GetLinkEntity();

        return null;
    }

    public GameObject GetObjectInItem(string strObjectName)
    {
        return GetObjectInItem(cachedTransform, strObjectName);
    }

    protected GameObject GetObjectInItem(Transform tr, string strObjectNanme)
    {
        return null;
    }
}
