using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ITEM_UNIT_INDEX = System.UInt32;
using EAObjID = System.UInt32;

// Equipment window slot location
public enum eEquipSlotSpot
{
    eESS_MainWeapon1,
    eESS_MainWeapon2,
    eESS_MainWeapon3,
    eESS_SkillWeapon,
    eESS_Max
}

// Character Equipment Slot Value
public enum eEquipmentSlotType
{
    eEST_BASE = 0,
    eEST_Weapon1 = 100,
    eEST_Weapon2,
    eEST_Weapon3,
    eEST_SkillWeapon,
    eEST_MAX,
};


// Weapon Accessories Slot Value
enum eWeaponAccessorySlotType
{
    eWAST_BASE = 0,
    eWAST_Silencer = 30,
    eWAST_Scope,
    eWAST_Barrel,
    eWAST_GunBody,
    eWAST_Option,
    eWAST_Disguise,
    eWAST_MAX
};

// Suit(Armor) Part Slot Value
enum eSuitPartSlotType
{
    eSPST_BASE = 0,
    eSPST_Head = 50,
    eSPST_Face,
    eSPST_Arm,
    eSPST_Breast,
    eSPST_Leg,
    eSPST_Waist,
    eSPST_MAX
};

// Existing Item System Area
enum eItemSaveSpot
{
    eISS_MyInven = 200,
    eISS_MyEquip,
    eISS_MyShop,
    eISS_Looting,
    eISS_WareHouse,
    eISS_MAX
};

////////////////////////////////////////////////////////////////////////////////////
/* Item unit storage location */
enum eItemKeepSpot
{
    eIKS_Base,
    eIKS_MyInven,
    eIKS_MyEquip,
    eIKS_OtherEquip,
    eIKS_Store,
    eIKS_Looting,
    eIKS_WareHouse,
    eIKS_MAX
};

/* Item Type  */
public enum eItemType
{
    eIT_Base,
    eIT_Weapon,
    eIT_Defense,
    eIT_Material,
    eIT_Potion,
    eIT_Accessory,
    eIT_Bullet,
    eIT_Max,
};

/* attach type */
public enum eAttachType
{
    eAT_Unarmed = 0,
    eAT_Sword,
    eAT_TwoHandSword,
    eAT_TwinSword,
    eAT_Bow,
    eAT_Rifle,
    eAT_Pistol,
    eAT_DualPistol,
    eWT_Turret,
    eWT_Breathing,
    eWT_Shield,
    eWT_Max,
};

public class EAItemInfo
{
    public ITEM_UNIT_INDEX m_EAItemId;
    public EAObjID         m_ObjId;
    public string m_ModelTypeIndex;
    public string m_szItemName;
    public eItemType m_eItemType;
    public uint m_nLevel;
    public uint m_nDurability;
    public uint m_nPrice;
    public uint m_nWeight;
    public uint m_nCount;

    public Type m_objClassType;  // Item class type

    public EAItemInfo()
    {
        m_EAItemId = CObjGlobal.InvalidItemID;
        m_ObjId = CObjGlobal.InvalidObjID;
        m_ModelTypeIndex = string.Empty;
        m_szItemName = string.Empty;
        m_eItemType = eItemType.eIT_Base;
        m_nLevel = 0;
        m_nDurability = 0;
        m_nPrice = 0;
        m_nWeight = 0;
        m_nCount = 0;
        m_objClassType = typeof(EAItem);
    }

    public EAItemInfo(EAItemInfo ib)
    {
        m_EAItemId = ib.m_EAItemId;
        m_ObjId = ib.m_ObjId;
        m_ModelTypeIndex = ib.m_ModelTypeIndex;
        m_szItemName = ib.m_szItemName;
        m_eItemType = ib.m_eItemType;
        m_nLevel = ib.m_nLevel;
        m_nDurability = ib.m_nDurability;
        m_nPrice = ib.m_nPrice;
        m_nWeight = ib.m_nWeight;
        m_nCount = ib.m_nCount;
        m_objClassType = ib.m_objClassType;
    }
}

public class EAItemAttackWeaponInfo
{
    public string id;
    public eAttachType attachType;
    public float fKillDistance;  //  [4/7/2014 puos] Range
    public float fFiringTime;     //  [4/7/2014 puos] Launch time
    public string uProjectileModelType;
    public Type m_objProjectileClassType; // Item class type
    public float fProjectileSpeed;
    public bool bAutoMode;
    public float m_fTargetDistance; // Target offset distance
    public EAObjID m_TargetActorId; // Target actor id
    public string m_strTargetActorBoneName;
    public float projectileRadius;
    
    public EAItemAttackWeaponInfo()
    {
        attachType = eAttachType.eAT_Unarmed;
        fKillDistance = 50.0f;
        fFiringTime = 0;
        uProjectileModelType = "";
        m_objProjectileClassType = typeof(EAProjectile);
        fProjectileSpeed = 0;
        bAutoMode = true;
        m_fTargetDistance = 0;
        m_TargetActorId = CObjGlobal.InvalidObjID;
        m_strTargetActorBoneName = null;
        projectileRadius = 0.0f;
    }

    public EAItemAttackWeaponInfo(EAItemAttackWeaponInfo obj)
    {
        attachType = obj.attachType;
        fKillDistance = obj.fKillDistance;
        fFiringTime = obj.fFiringTime;
        uProjectileModelType = obj.uProjectileModelType;
        m_objProjectileClassType = obj.m_objProjectileClassType;
        fProjectileSpeed = obj.fProjectileSpeed;
        bAutoMode = obj.bAutoMode;
        m_fTargetDistance = obj.m_fTargetDistance;
        m_TargetActorId = obj.m_TargetActorId;
        m_strTargetActorBoneName = obj.m_strTargetActorBoneName;
        projectileRadius = obj.projectileRadius;
    }
}

public class EAItemDefenseInfo
{
    public int m_nIndex;
    public int m_nDefType;
    public int m_nDefense;
    public int m_nRace;
    public int m_nPartNum;
    public eAttachType attachType;

    public EAItemDefenseInfo() 
    {
        m_nIndex = 0;
        m_nDefType = 0;
        m_nDefense = 0;
        m_nRace = 0;
        m_nPartNum = 0;
        attachType = eAttachType.eWT_Shield;
    }

    public EAItemDefenseInfo(EAItemDefenseInfo defenseInfo)
    {
        m_nIndex = defenseInfo.m_nIndex;
        m_nDefType = defenseInfo.m_nDefType;
        m_nDefense = defenseInfo.m_nDefense;
        m_nRace = defenseInfo.m_nRace;
        m_nPartNum = defenseInfo.m_nPartNum;
        attachType = defenseInfo.attachType;
    }
}
