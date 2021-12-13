using System.Collections;
using System.Collections.Generic;

using EAObjID = System.UInt32;
using ETTblID = System.UInt32;

public class EACObjManager : EAGenericSingleton<EACObjManager>
{
    Dictionary<EAObjID, EA_CCharUser> m_Players = new Dictionary<EAObjID, EA_CCharUser>();
    Dictionary<EAObjID, EA_CCharMob> m_Monsters = new Dictionary<EAObjID, EA_CCharMob>();
    Dictionary<EAObjID, EA_CCharNPC> m_NPCs = new Dictionary<EAObjID, EA_CCharNPC>();
    Dictionary<EAObjID, EA_CItem> m_Items = new Dictionary<EAObjID, EA_CItem>();
    Dictionary<EAObjID, EA_CMapObject> m_Objects = new Dictionary<EAObjID, EA_CMapObject>();

    List<EA_CObjectBase> m_entities = new List<EA_CObjectBase>();

    EA_CCharUser m_MainPlayer = null;  //	My character's information.

    //--------------------------------------------------------------------------
    //	Information for constructors and destructors
    EAIDGenerator m_IDGenerater = null;
    //--------------------------------------------------------------------------

    public EACObjManager() 
    {
        //Debug.Log("EACObjManager Init");
        //  [3/6/2014 puos] MainPlayer create
        m_MainPlayer = new EA_CCharUser();

        ObjectInfo mainObjInfo = new ObjectInfo();

        mainObjInfo.m_eObjState = eObjectState.CS_MYENTITY;
        mainObjInfo.m_eObjType = eObjectType.CT_MYPLAYER;
        mainObjInfo.m_ObjId = CObjGlobal.MyPlayerID;
        m_MainPlayer.SetObjInfo(mainObjInfo);

        CharInfo emptyCharInfo = new CharInfo();
        m_MainPlayer.SetCharInfo(emptyCharInfo);

        m_IDGenerater = new EAIDGenerator(50000);
    }

    public void Destroy() 
    {
        // player
        {
            var it = m_Players.GetEnumerator();
            while (it.MoveNext())
            {
                it.Current.Value.ResetInfo(eObjectState.CS_UNENTITY);
            }
            m_Players.Clear();
        }

        // monster
        {
            var it = m_Monsters.GetEnumerator();
            while(it.MoveNext())
            {
                it.Current.Value.ResetInfo(eObjectState.CS_UNENTITY);
            }
            m_Monsters.Clear();
        }

        // npc
        {
            var it = m_NPCs.GetEnumerator();
            while(it.MoveNext())
            {
                it.Current.Value.ResetInfo(eObjectState.CS_UNENTITY);
            }
            m_NPCs.Clear();
        }

        // item
        {
            var it = m_Items.GetEnumerator();
            while(it.MoveNext())
            {
                it.Current.Value.ResetInfo(eObjectState.CS_UNENTITY);
            }
        }

        // map object
        {
            var it = m_Objects.GetEnumerator();
            while(it.MoveNext())
            {
                it.Current.Value.ResetInfo(eObjectState.CS_UNENTITY);
            }
            m_Objects.Clear();
        }

        m_entities.Clear();

        m_IDGenerater.ReGenerate();
    }

    /// <summary>
    /// The game object from the server is applied first and the Cry Entity is created by FirstUpdate ().
    /// </summary>
    /// <param name="GameObjInfo"></param>
    /// <returns></returns>
    public EA_CObjectBase CreateGameObject(ObjectInfo gameObjInfo)
    {
        EA_CObjectBase returnObject = null;

        gameObjInfo.m_eObjState = eObjectState.CS_SETENTITY;
     
        //	Temporarily specify ObjId (sometimes temporary use by external system)
        //	Apply the information to the generated number
        if (gameObjInfo.m_ObjId == CObjGlobal.InvalidObjID) gameObjInfo.m_ObjId = (EAObjID)m_IDGenerater.GenerateID();

        switch(gameObjInfo.m_eObjType)
        {
            case eObjectType.CT_MYPLAYER:
                {
                    GetMainPlayer().SetObjInfo(gameObjInfo);
                    returnObject = GetMainPlayer();
                }
                break;
            case eObjectType.CT_PLAYER:
                {
                   EA_CCharUser pCharPlayer = GetPlayer(gameObjInfo.m_ObjId);

                    if (pCharPlayer == null)
                    {
                        pCharPlayer = new EA_CCharUser();
                        m_Players.Add(gameObjInfo.m_ObjId, pCharPlayer);
                    }

                    pCharPlayer.SetObjInfo(gameObjInfo);
                    returnObject = pCharPlayer;
                }
                break;
            case eObjectType.CT_NPC:
                {
                    EA_CCharNPC pCharNPC = GetNPC(gameObjInfo.m_ObjId);

                    if(pCharNPC == null)
                    {
                        pCharNPC = new EA_CCharNPC();
                        m_NPCs.Add(gameObjInfo.m_ObjId, pCharNPC);
                    }
              
                    pCharNPC.SetObjInfo(gameObjInfo);
                    returnObject = pCharNPC;
                }
                break;
            case eObjectType.CT_MONSTER:
                {
                    EA_CCharMob pCharMob = GetMob(gameObjInfo.m_ObjId);

                    if(pCharMob == null)
                    {
                        pCharMob = new EA_CCharMob();
                        m_Monsters.Add(gameObjInfo.m_ObjId, pCharMob);
                    }

                    //	Apply the information to the generated number
                    //			assert( pCharInfo && "No Character Info" );
                    pCharMob.SetObjInfo(gameObjInfo);
                    returnObject = pCharMob;
                }
                break;
            case eObjectType.CT_ITEMOBJECT:
                {
                    EA_CItem pItem = GetItem(gameObjInfo.m_ObjId);

                    if(pItem == null)
                    {
                        pItem = new EA_CItem();
                        m_Items.Add(gameObjInfo.m_ObjId, pItem);
                    }
                    pItem.SetObjInfo(gameObjInfo);
                    returnObject = pItem;
                }
                break;
            case eObjectType.CT_MAPOBJECT:
                {
                    EA_CMapObject pMap = GetMapObject(gameObjInfo.m_ObjId);

                    if(pMap == null)
                    {
                        pMap = new EA_CMapObject();
                        m_Objects.Add(gameObjInfo.m_ObjId, pMap);
                    }
                    pMap.SetObjInfo(gameObjInfo);
                    returnObject = pMap;
                }
                break;

        }
        //assert( pReturnObject );
        //	Apply the information to the generated number
        if (returnObject != null) m_entities.Add(returnObject);

        return returnObject;
    }

    public bool DeleteGameObject(eObjectType delObjectType, EAObjID id)
    {
        if (CObjGlobal.InvalidObjID == id) return false;

        EA_CObjectBase obj = null;

        switch(delObjectType)
        {
            default:
            case eObjectType.CT_MYPLAYER:
                break;
            case eObjectType.CT_PLAYER: 
                {
                    obj = RemovePlayer(id);
                }
                break;
            case eObjectType.CT_NPC:
                {
                    obj = RemoveNPC(id);
                }
                break;
            case eObjectType.CT_MONSTER:
                {
                    obj = RemoveMob(id);
                }
                break;
            case eObjectType.CT_ITEMOBJECT:
                {
                    obj = RemoveItem(id);
                }
                break;

            case eObjectType.CT_MAPOBJECT:
                {
                    obj = RemoveMapObject(id);
                }
                break;
        }

        RemoveEntity(obj);
        
        return true;
    }

    public EA_CCharUser CreateUser(ObjectInfo GameObjInfo, CharInfo charInfo)
    {
        GameObjInfo.m_eObjType = eObjectType.CT_PLAYER;
        EA_CCharUser user = CreateGameObject(GameObjInfo) as EA_CCharUser;

        if(user != null)
        {
            user.SetCharInfo(charInfo);
            user.Initialize();
        }
        return user;
    }

    public EA_CCharMob CreateMob(ObjectInfo GameObjInfo, MobInfo mobInfo)
    {
        GameObjInfo.m_eObjType = eObjectType.CT_MONSTER;
        EA_CCharMob mob = CreateGameObject(GameObjInfo) as EA_CCharMob;

        if (mob != null)
        {
            mob.SetMobInfo(mobInfo);
            mob.Initialize();
        }
        return mob;
    }

    public EA_CCharNPC CreateNPC(ObjectInfo GameObjInfo, NPCInfo npcInfo)
    {
        GameObjInfo.m_eObjType = eObjectType.CT_NPC;
        EA_CCharNPC npc = CreateGameObject(GameObjInfo) as EA_CCharNPC;

        if(npc != null)
        {
            npc.SetNPCInfo(npcInfo);
            npc.Initialize();
        }
        return npc;
    }

    public EA_CItem CreateItem(ObjectInfo GameObjInfo, ItemObjInfo itemInfo)
    {
        GameObjInfo.m_eObjType = eObjectType.CT_ITEMOBJECT;
        EA_CItem item = CreateGameObject(GameObjInfo) as EA_CItem;

        if(item != null)
        {
            item.SetItemInfo(itemInfo);
            item.Initialize();
        }

        return item;
    }

    public EA_CMapObject CreateMapObject(ObjectInfo GameObjInfo, MapObjInfo mapInfo)
    {
        GameObjInfo.m_eObjType = eObjectType.CT_MAPOBJECT;
        EA_CMapObject pMapObject = CreateGameObject(GameObjInfo) as EA_CMapObject;

        if(pMapObject != null)
        {
            pMapObject.SetMapInfo(mapInfo);
            pMapObject.Initialize();
        }

        return pMapObject;
    }

    public EA_CCharUser GetMainPlayer() { return m_MainPlayer; }

    EA_CCharUser GetPlayer(EAObjID id)
    {
        if (m_MainPlayer.GetObjID() == id) return m_MainPlayer;

        m_Players.TryGetValue(id, out EA_CCharUser user);
        return user;
    }

    EA_CCharMob GetMob(EAObjID id)
    {
        m_Monsters.TryGetValue(id, out EA_CCharMob mob);
        return mob;
    }

    EA_CCharNPC GetNPC(EAObjID id)
    {
        m_NPCs.TryGetValue(id, out EA_CCharNPC npc);
        return npc;
    }

    EA_CItem GetItem(EAObjID id)
    {
        m_Items.TryGetValue(id, out EA_CItem item);
        return item;
    }
    
    EA_CMapObject GetMapObject(EAObjID id)
    {
        m_Objects.TryGetValue(id, out EA_CMapObject mapObj);
        return mapObj;
    }

    private EA_CObjectBase RemovePlayer(EAObjID id)
    {
        EA_CCharUser user = GetPlayer(id);

        if (user == null) return null;

        user.ResetInfo(eObjectState.CS_UNENTITY);
        m_Players.Remove((uint)id);
        m_IDGenerater.FreeID(id);
        
        return user;
    }

    private EA_CObjectBase RemoveMob(EAObjID id)
    {
        EA_CCharMob mob = GetMob(id);

        if (mob == null) return null;

        mob.ResetInfo(eObjectState.CS_UNENTITY);
        m_Monsters.Remove((uint)id);
        m_IDGenerater.FreeID(id);

        return mob;
    }

    private EA_CObjectBase RemoveNPC(EAObjID id)
    {
        EA_CCharNPC npc = GetNPC(id);

        if (npc == null) return null;

        npc.ResetInfo(eObjectState.CS_UNENTITY);
        m_NPCs.Remove((uint)id);
        m_IDGenerater.FreeID(id);

        return npc;
    }

    private EA_CObjectBase RemoveItem(EAObjID id)
    {
        EA_CItem item = GetItem(id);

        if (item == null) return null;

        item.ResetInfo(eObjectState.CS_UNENTITY);
        m_Items.Remove((uint)id);
        m_IDGenerater.FreeID(id);

        return item;
    }

    private EA_CObjectBase RemoveMapObject(EAObjID id)
    {
        EA_CMapObject mapObject = GetMapObject(id);

        if (mapObject == null) return null;

        mapObject.ResetInfo(eObjectState.CS_UNENTITY);
        m_Objects.Remove((uint)id);
        m_IDGenerater.FreeID(id);

        return mapObject;
    }

    private void RemoveEntity(EA_CObjectBase gameEntity)
    {
        if (gameEntity == null) return;

        m_entities.Remove(gameEntity);
    }

    public EA_CCharBPlayer GetActor(EAObjID id)
    {
        EA_CCharBPlayer pActor = null;
        pActor = (pActor == null) ? GetPlayer(id) : pActor;
        pActor = (pActor == null) ? GetMob(id) : pActor;
        pActor = (pActor == null) ? GetNPC(id) : pActor;
        
        return pActor;
    }

    public EA_CObjectBase GetGameObject(EAObjID id)
    {
        EA_CObjectBase obj = null;
        obj = (obj == null) ? GetActor(id) : obj;
        obj = (obj == null) ? GetMapObject(id) : obj;
        obj = (obj == null) ? GetItem(id) : obj;

        return obj;
    }
}
