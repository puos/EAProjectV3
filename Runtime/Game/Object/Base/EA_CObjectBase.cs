using UnityEngine;
using EAObjID = System.UInt32;
using EATblID = System.UInt32;
using System;

public class EA_CObjectBase 
{
    protected ObjectInfo m_ObjInfo = new ObjectInfo();  //	object basic information
    protected EAObject   m_pEntity;  // game entity

    public EA_CObjectBase() 
    {    
    }

    public virtual eObjectKind GetKind() { return eObjectKind.CK_OBJECT; }

    public EAObjID GetObjID() { return m_ObjInfo.m_ObjId; }

    public bool SetObjInfo(ObjectInfo objInfo)
    {
        m_ObjInfo = objInfo;

        switch(m_ObjInfo.m_eObjState)
        {
            case eObjectState.CS_MYENTITY: break;
                    
            case eObjectState.CS_UNENTITY:
                {
                    Release();
                    if (m_pEntity != null) EA_ObjectFactory.EntityUnSetting(this);
                }
                break;
            case eObjectState.CS_SETENTITY:
                {
                    if (m_pEntity == null) EA_ObjectFactory.EntitySetting(this, m_ObjInfo);
                    ChangeModel(m_ObjInfo.m_ModelTypeIndex);
                }
                break;

            case eObjectState.CS_SHOW:
                if (m_pEntity != null) m_pEntity.SetActive(true);
                break;

            case eObjectState.CS_DEAD:
            case eObjectState.CS_HIDE:
                if (m_pEntity != null) m_pEntity.SetActive(false);
                break;
        }

        if(m_pEntity != null)
        {
            if (!string.IsNullOrEmpty(m_ObjInfo.m_strGameName)) m_pEntity.Name = m_ObjInfo.m_strGameName;
        }

        return true;
    }

    public ObjectInfo GetObjInfo() { return m_ObjInfo; }

    public virtual void SetLinkEntity(EAObject pLinkEntity)
    {
        if (m_pEntity != pLinkEntity) m_pEntity = pLinkEntity;
    }

    public EAObject GetLinkEntity() { return m_pEntity; }
    public virtual bool Initialize() { return true; }
    protected virtual bool Release() 
    {
        m_ObjInfo.m_ObjId = CObjGlobal.InvalidObjID;
        return true; 
    }
    public virtual bool ChangeModel(string modelType)  { return true; }
    public virtual bool ChangeParts() { return true;  }
    public virtual bool ChangeAnimation(EATblID _iAniType)  {  return true; }
    public virtual bool ChangeTexture(EATblID _iTexType) { return true;  } 
    public virtual bool ResetInfo(eObjectState eChangeState)
    {
        m_ObjInfo.m_eObjState = eChangeState;
        SetObjInfo(m_ObjInfo);
        return true;
    }
    // The ability to change the name of a character  [9/16/2009 jgb] 
    public void ChangeName(string strGameName)
    {
        if (m_ObjInfo.m_strGameName.Equals(strGameName, StringComparison.Ordinal)) return;

        m_ObjInfo.SetObjName(strGameName);
    }

    public virtual Transform GetObjectInActor(string szBoneName)  {  return null;  }
}
