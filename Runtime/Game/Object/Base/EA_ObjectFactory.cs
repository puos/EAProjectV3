using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EAObjID = System.UInt32;
using EATblID = System.UInt32;

/// <summary>
/// Create game object and register in pool
/// </summary>
static class EA_ObjectFactory 
{
    public static bool EntitySetting(EA_CObjectBase pSetObject , ObjectInfo setObjInfo)
    {
        if(pSetObject == null) return false;
        if (setObjInfo.m_eObjState != eObjectState.CS_SETENTITY) return false;

        string prefabName = setObjInfo.m_ModelTypeIndex;
        EAObject eaObj = GameResourceManager.instance.CreateEAObject(EResourceGroup.Object, setObjInfo.m_objClassType, prefabName);

        if (eaObj == null)
        {
            GameObject obj = new GameObject(prefabName, setObjInfo.m_objClassType);
            eaObj = obj.GetComponent<EAObject>();
        }

        pSetObject.SetLinkEntity(eaObj);

        //	Create around object table
        switch (setObjInfo.m_eObjType)
        {
            case eObjectType.CT_NPC:
            case eObjectType.CT_MONSTER:
            case eObjectType.CT_PLAYER:
            case eObjectType.CT_MYPLAYER:
                {
                    EAActor actor = eaObj.GetComponent<EAActor>();
                    if(actor == null)
                    {
                        actor = eaObj.gameObject.AddComponent<EAActor>();
                        setObjInfo.m_objClassType = typeof(EAActor);
                    }
                    ((EA_CCharBPlayer)pSetObject).SetLinkActor(actor);
                }
                break;
            case eObjectType.CT_MAPOBJECT:
                {
                    EAMapObject mapObject = eaObj.GetComponent<EAMapObject>();
                    if(mapObject == null)
                    {
                        mapObject = eaObj.gameObject.AddComponent<EAMapObject>();
                        setObjInfo.m_objClassType = typeof(EAMapObject);
                    }
                    ((EA_CMapObject)pSetObject).SetLinkMapObject(mapObject);
                }
                break;
            case eObjectType.CT_ITEMOBJECT:
                {
                    EAItem itemObject = eaObj.GetComponent<EAItem>();
                    if(itemObject == null)
                    {
                        itemObject = eaObj.gameObject.AddComponent<EAItem>();
                        setObjInfo.m_objClassType = typeof(EAItem);
                    }
                    ((EA_CItem)pSetObject).SetLinkItem(itemObject);
                }
                break;
        }

        return true;
    }
    public static bool EntityUnSetting(EA_CObjectBase pDelObjBase)
    {
        if (pDelObjBase == null) return false;
        if (pDelObjBase.GetObjInfo().m_eObjState == eObjectState.CS_SETENTITY) return false;

        EAObject pGameObject = pDelObjBase.GetLinkEntity();

        if (pGameObject == null) return false;

        pGameObject.SetParent(null);

        GameResourceManager.instance.ReleaseEAObject(pGameObject);

        pDelObjBase.SetLinkEntity(null);

        return true;
    }
}
