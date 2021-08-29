using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class EAEffectModule
{
    private EASfx m_pSfx = null;

    private EACEffectInfo m_effectInfo = new EACEffectInfo();

    private bool m_bAutoDelete = false;

    public EACEffectInfo GetEffectInfo() { return m_effectInfo;  }

    public EASfx GetSfx() { return m_pSfx; }

    public virtual void SetLinkEffect(EASfx sfx)
    {
        if (sfx == m_pSfx) return;
        m_pSfx = sfx;
    }

    public void Initialize() 
    {
        EAMainFrame.onUpdate.Remove(OnUpdate);
        EAMainFrame.onUpdate.Add(OnUpdate);
    }
    protected void OnUpdate()
    {
        if (m_bAutoDelete == false) return;
        if (m_pSfx == null) return;
        if (m_pSfx.IsAlive() == true) return;

        
    }
    public void Release() 
    {
        m_effectInfo.m_EAEffectId = CObjGlobal.InvalidEffectID;
        m_pSfx = null;
        EAMainFrame.onUpdate.Remove(OnUpdate);
    }
    
    public virtual bool SetObjInfo(EACEffectInfo effectInfo)
    {
        m_effectInfo = effectInfo;

        switch(m_effectInfo.m_eEffectState)
        {
            case eEffectState.ES_Load: { if (m_pSfx == null) EffectSetting(this); } break;
            case eEffectState.ES_UnLoad: { Release(); if (m_pSfx != null) EffectUnSetting(this);  } break;
            case eEffectState.ES_Start: { if (m_pSfx != null) m_pSfx.StartFx(); } break;
            case eEffectState.ES_Stop: { if (m_pSfx != null) m_pSfx.StopFx(); } break;
            case eEffectState.ES_Skip: { if (m_pSfx != null) m_pSfx.SkipFx(); } break;
        }
        return true;
    }
    public virtual bool ResetInfo(eEffectState eChangeState)
    {
        m_effectInfo.m_eEffectState = eChangeState;
        SetObjInfo(m_effectInfo);
        return true;
    }

    public static bool EffectSetting(EAEffectModule pEffectNode)
    {
        EACEffectInfo effectInfo = pEffectNode.GetEffectInfo();

        if (effectInfo.m_eEffectState != eEffectState.ES_Load) return false;

        switch(effectInfo.m_eAttachType)
        {
            case eEffectAttachType.eWorld: 
                {
                    EAObject obj = GameResourceManager.instance.CreateEAObject(EResourceGroup.Sfx, typeof(EASfx), effectInfo.m_EffectTableIndex);
                    pEffectNode.SetLinkEffect(obj.GetComponent<EASfx>());
                } 
                break;
            case eEffectAttachType.eLinkOffset: 
                {
                    EA_CObjectBase pObjectBase = EACObjManager.instance.GetGameObject(effectInfo.m_AttachObjectId);
                    EAObject obj = GameResourceManager.instance.CreateEAObject(EResourceGroup.Sfx, typeof(EASfx), effectInfo.m_EffectTableIndex);
                    if (pObjectBase != null) 
                    {
                        EAObject parent = pObjectBase.GetLinkEntity();
                        EAFrameUtil.SetParent(obj.tr, parent.tr);
                    }
                    pEffectNode.SetLinkEffect(obj.GetComponent<EASfx>());
                }
                break;
            case eEffectAttachType.eLinkBone: 
                {
                    //  [4/11/2014 puos]  attach to the actor bone
                    EA_CObjectBase pObjectBase = EACObjManager.instance.GetGameObject(effectInfo.m_AttachObjectId);
                    EAObject obj = GameResourceManager.instance.CreateEAObject(EResourceGroup.Sfx, typeof(EASfx), effectInfo.m_EffectTableIndex);
                    if (pObjectBase != null)
                    {
                       UnityEngine.Transform tr = pObjectBase.GetObjectInActor(effectInfo.m_AttachBoneName);
                       EAFrameUtil.SetParent(obj.tr, tr);
                    }
                    pEffectNode.SetLinkEffect(obj.GetComponent<EASfx>());
                } 
                break;
        }

        return true;
    }

    public static bool EffectUnSetting(EAEffectModule pDelEffectNode)
    {
        if (pDelEffectNode == null) return false;

        EAObject pGameObject = pDelEffectNode.GetSfx() as EAObject;

        if (pGameObject == null) return false;

        pGameObject.SetParent(null);
        GameResourceManager.instance.ReleaseEAObject(pGameObject);
        pDelEffectNode.SetLinkEffect(null);

        return true;
    }
}
