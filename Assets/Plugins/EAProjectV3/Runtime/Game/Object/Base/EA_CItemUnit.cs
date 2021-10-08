using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ITEM_UNIT_INDEX = System.UInt32;
using EAObjID = System.UInt32;

public class EA_CItemUnit
{
    public const int LOOTING_SLOTMAX = 10;
    public const int BANKSLOTMAX = 20;
    public const int SHOP_SLOTMAX = 30;

    EAItemInfo             m_ItemInfo = new EAItemInfo();
    EAItemAttackWeaponInfo m_ItemAttackWeaponInfo = new EAItemAttackWeaponInfo();
    EAItemDefenseInfo      m_ItemDefenseInfo = new EAItemDefenseInfo();

    public EA_CItemUnit() 
    {
    }

    public void Release()
    {
        EA_ItemManager.instance.DeleteItemUnit(GetItemId());
    }

    // Get Item Unit ID
    public ITEM_UNIT_INDEX GetItemId() { return m_ItemInfo.m_EAItemId; }
    public EAObjID GetObjId() { return m_ItemInfo.m_ObjId; }
    public eItemType GetItemType() { return m_ItemInfo.m_eItemType; }
    // Get Item Quantity
    public uint GetCount() { return m_ItemInfo.m_nCount; }
    public eAttachType GetAttachType() 
    {
        if (GetItemType() == eItemType.eIT_Weapon) return m_ItemAttackWeaponInfo.attachType;
        if (GetItemType() == eItemType.eIT_Defense)return m_ItemDefenseInfo.attachType;
        return eAttachType.eWT_Max;
    }
    public eItemObjType GetItemObjectType()
    {
        if (GetItemType() == eItemType.eIT_Weapon) return eItemObjType.IK_WEAPON;
        if (GetItemType() == eItemType.eIT_Defense) return eItemObjType.IK_DEFENSE;
        return eItemObjType.IK_ETC;
    }
    public string GetModelTypeIdx() { return m_ItemInfo.m_ModelTypeIndex; }
    public Type GetObjClassType() { return m_ItemInfo.m_objClassType; }
    public void SetObjId(EAObjID objId) {  m_ItemInfo.m_ObjId = objId; }
    public void SetItemInfo(EAItemInfo itemInfo)
    {
        m_ItemInfo = new EAItemInfo(itemInfo);
    }
    public void SetAttackWeaponInfo(EAItemAttackWeaponInfo attackWeaponInfo)
    {
        m_ItemAttackWeaponInfo = new EAItemAttackWeaponInfo(attackWeaponInfo);
    }
    public void SetDefenseInfo(EAItemDefenseInfo defenseInfo)
    {
        m_ItemDefenseInfo = new EAItemDefenseInfo(defenseInfo);
    }
    public EAItemAttackWeaponInfo GetAttackWeaponinfo() { return m_ItemAttackWeaponInfo; }
}
