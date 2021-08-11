using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public  EAActor GetOwner()
    {
        if (GetItemBase() == null) return null;

        EAObjID objID = GetItemBase().GetItemInfo().m_HavenUser;
        EA_CCharBPlayer pCharBase = EACObjManager.instance.GetActor(objID);
        if (pCharBase != null) return pCharBase.GetLinkIActor();

        return null;
    }
    public virtual void OnAction(params object[] parms)
    {
    }

    public GameObject GetObjectInItem(string strObjectName)
    {
        return GetObjectInItem(tr, strObjectName);
    }
    protected GameObject GetObjectInItem(Transform tr, string strObjectNanme)
    {
        for(int i = 0; i < tr.childCount; ++i)
        {
            Transform child = tr.GetChild(i);

            if (strObjectNanme.Equals(child.gameObject.name,StringComparison.Ordinal))
                return child.gameObject;

            GameObject pFindObject = GetObjectInItem(child.gameObject.transform , strObjectNanme);
            if (pFindObject != null) return pFindObject;
        }
        return null;
    }
}
