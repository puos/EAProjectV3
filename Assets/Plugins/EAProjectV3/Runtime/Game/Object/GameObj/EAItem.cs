using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using EAObjID = System.UInt32;

public class EAItem : EAObject
{
    protected EA_CItem m_ItemBase = null;

    public uint Id { get { return (m_ItemBase != null) ? m_ItemBase.GetObjID() : CObjGlobal.InvalidObjID; } }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Release()
    {
        base.Release();
        EACObjManager.instance.DeleteGameObject(eObjectType.CT_ITEMOBJECT, Id);
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
        var pActor = EACObjManager.instance.GetActor(objID);
        if (pActor != null) return pActor;

        return null;
    }
  
    public override uint GetObjId() { return Id; }

    public Transform GetObjectInItem(string strObjectName)
    {
        return GetObjectInItem(tr, strObjectName);
    }
    protected Transform GetObjectInItem(Transform tr, string strObjectNanme)
    {
        for(int i = 0; i < tr.childCount; ++i)
        {
            Transform child = tr.GetChild(i);

            if (strObjectNanme.Equals(child.gameObject.name,StringComparison.Ordinal))
                return child;

            Transform pFindTr = GetObjectInItem(child.gameObject.transform , strObjectNanme);
            if (pFindTr != null) return pFindTr;
        }
        return null;
    }
}
