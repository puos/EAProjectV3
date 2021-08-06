using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EAObjID = System.UInt32;
using EATblID = System.UInt32;
using ITEM_UNIT_INDEX = System.UInt32;
using EAEffectID = System.UInt32;


public enum eObjectState
{
    CS_SETENTITY,
    CS_UNENTITY,
    CS_DEAD,
    CS_MYENTITY,
    CS_HIDE,
    CS_SHOW,
    CS_MAXNUM,
}

//	Object classification
public enum eObjectType
{
    CT_NPC = 1,
    CT_MONSTER,
    CT_MAPOBJECT,
    CT_ITEMOBJECT,
    CT_PLAYER,
    CT_MYPLAYER,
    CT_Vehicle,
    CT_MAXNUM
};

//	Object type
public enum eObjectKind
{
    CK_ACTOR,
    CK_Vehicle,
    CK_ITEM,
    CK_OBJECT,
    CK_MAP,
    CK_MAXNUM
};

//	Part division (character + vehicle)
public enum eCharParts
{
    CP_HEAD,
    CP_UPBODY,
    CP_DOWNBODY,
    CP_ARM,
    CP_LEG,
    CP_HANDS,
    CP_FEET,
    CP_MAX,
}

// Item type 
public enum eItemObjType
{
    IK_DEFENSE,
    IK_WEAPON,
    IK_Projectile,
    IK_ETC,
}

public static class CObjGlobal
{
    public static EAObjID InvalidObjID = 0;
    public static EATblID InvalidTblID = 0;
    public static ITEM_UNIT_INDEX InvalidItemID = 0;
    public static EAObjID MyPlayerID = 100000;
    public static EAEffectID InvalidEffectID = 0;
}

public class ObjectInfo
{
    public EAObjID m_ObjId ;		//	Unique number of the object specified by the server

    public eObjectType m_eObjType;		//	Object type (character, mob, npc, vehicle, etc.)
    public eObjectState m_eObjState;	//	The state of the object
   
    public string m_ModelTypeIndex;	  // Model table index
    public Type m_objClassType;           // Object class type

    public string m_strGameName;	//	Name to be used in the game
    
    public Vector3 spawnPos;
    public Vector3 spawnAngle;

    public ObjectInfo() 
    {
        m_objClassType = typeof(EAObject);
    }

    public ObjectInfo(ObjectInfo obj) 
    {
        m_ObjId = obj.m_ObjId;
        m_eObjType = obj.m_eObjType;
        m_eObjState = obj.m_eObjState;
        m_ModelTypeIndex = obj.m_ModelTypeIndex;
        m_strGameName = obj.m_strGameName;
        spawnPos = obj.spawnPos;
        spawnAngle = obj.spawnAngle;
        m_objClassType = obj.m_objClassType;
    }
    public void SetObjName(string szNewName)
    {
        m_strGameName = szNewName;
    }
}

/// <summary>
/// ObjectInfo and other character information
/// </summary>
public class CharInfo
{
    public uint m_ClassType;	    //	Character occupation
    public string[] m_PartTblId;    //	Index number of the part you are currently wearing (obtained from Item Manager)

    public float Hp;
    public float Atk;

    public float maxHp;
    public float maxAtk;

    public CharInfo() { }

    public CharInfo(CharInfo charInfo)
    {
        m_ClassType = charInfo.m_ClassType;
        m_PartTblId = new string[charInfo.m_PartTblId.Length];
        for(int i = 0; i < charInfo.m_PartTblId.Length; ++i)
        {
            m_PartTblId[i] = charInfo.m_PartTblId[i];
        }
        Hp = charInfo.Hp;
        Atk = charInfo.Atk;
        maxHp = charInfo.maxHp;
        maxAtk = charInfo.maxAtk;
        m_ClassType = charInfo.m_ClassType;
    }
}

//	ObjectInfo and other mob information
public class MobInfo : CharInfo
{
    public int level;
    public int nPoint;

    public MobInfo() { }

    public MobInfo(MobInfo mobInfo) : base(mobInfo)
    {
        level   = mobInfo.level;
        nPoint = mobInfo.nPoint;
    }
}

/// ObjectInfo and NPC Information
public class NPCInfo : CharInfo
{
    public EATblID m_Npcindex = CObjGlobal.InvalidObjID; //	Feature table index on Npc
    public uint m_OwnerId = 0;

    public NPCInfo() { }

    public NPCInfo(NPCInfo npcInfo) : base(npcInfo) 
    {
        m_Npcindex = npcInfo.m_Npcindex;
        m_OwnerId = npcInfo.m_OwnerId;
    }
}

public class ItemObjInfo
{
    public EAObjID m_HavenUser; // Feature table index on Npc

    public ITEM_UNIT_INDEX m_iItemIndex;
    public int m_iItemCount;
    public uint m_itemRemainTime;
    public eItemObjType m_eItemType;
    public uint m_Score;

    public ItemObjInfo() 
    {
        m_HavenUser = CObjGlobal.InvalidObjID;
        m_iItemIndex = CObjGlobal.InvalidItemID;
        m_iItemCount = 0;
        m_itemRemainTime = 0;
        m_eItemType = eItemObjType.IK_ETC;
        m_Score = 0;
    }

    public ItemObjInfo(ItemObjInfo itemInfo)
    {
        m_HavenUser = itemInfo.m_HavenUser;
        m_iItemIndex = itemInfo.m_iItemIndex;
        m_iItemCount = itemInfo.m_iItemCount;
        m_itemRemainTime = itemInfo.m_itemRemainTime;
        m_eItemType = itemInfo.m_eItemType;
        m_Score = itemInfo.m_Score;
    }
}

public class MapObjInfo
{
    public int m_nDrability;

    public MapObjInfo() 
    {
        m_nDrability = 0;
    }
    public MapObjInfo(MapObjInfo mapInfo)
    {
        m_nDrability = mapInfo.m_nDrability;
    }
}
