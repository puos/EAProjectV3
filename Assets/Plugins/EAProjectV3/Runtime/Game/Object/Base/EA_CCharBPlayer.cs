﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EA_CCharBPlayer : EA_CObjectBase
{
    protected EAActor m_pLinkActor = null;

    public EA_CCharBPlayer() 
    {
    }

    public override eObjectKind GetKind() { return eObjectKind.CK_ACTOR; }

    public virtual EAActor GetLinkIActor() { return m_pLinkActor; }

    public virtual void SetLinkActor(EAActor pLinkActor)
    {
        if (pLinkActor == null) return;

        if (m_pLinkActor != pLinkActor)
        {
            m_pLinkActor = pLinkActor;
            m_pLinkActor.SetCharBase(this);
        }
    }

    public override bool Initialize()
    {
        base.Initialize();

        if (m_pLinkActor != null) m_pLinkActor.Initialize();
        return true;
    }

    protected override bool Release()
    {
        base.Release();

        if (m_pLinkActor != null && m_pLinkActor.Initialized) m_pLinkActor.Release();
        if (m_pLinkActor != null) m_pLinkActor.SetCharBase(null);
        m_pLinkActor = null;

        return true;
    }

    public void DoAttachItem(eAttachType attachType,eItemType itemType = eItemType.eIT_Base)
    {
        if (m_pLinkActor == null) return;
        m_pLinkActor.DoAttachItem(attachType,itemType);
    }

    public void SetItemAttachment(eAttachType attachType, EAObject gameObject)
    {
        if (m_pLinkActor == null) return;
        m_pLinkActor.SetItemAttachment(attachType, gameObject);
    }
    
    // Find an object inside an actor.
    public override Transform GetObjectInActor(string szBoneName)
    {
        if(m_pLinkActor != null)
        {
            EAActor pActor = m_pLinkActor.gameObject.GetComponent<EAActor>();
            if(pActor != null) return pActor.GetObjectInActor(szBoneName);
        } 
        return null;
    }
}
