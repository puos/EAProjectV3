using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ITEM_UNIT_INDEX = System.UInt32;
using EAObjID = System.UInt32;

public class EA_Equipment
{
    public uint nCurrEquipSlot = 0;  // Current weapon slot number
    Dictionary<uint, EA_CItemUnit> m_EquipmentList = new Dictionary<uint, EA_CItemUnit>();

    public EA_Equipment() 
    {
        for(uint slot = 0; slot < (uint)eEquipSlotSpot.eESS_Max; ++slot)
        {
            m_EquipmentList.Add(slot, null);
        }
    }
    // Storing Item Units in Equipment Slot Locations
    public bool InsertEquipItem(uint equip_slot,EA_CItemUnit item)
    {
        if (!m_EquipmentList.TryGetValue(equip_slot, out EA_CItemUnit value)) return false;

        EA_CItemUnit prevItemUnit = m_EquipmentList[equip_slot];

        if (prevItemUnit != null) prevItemUnit.Release();

        m_EquipmentList[equip_slot] = item;
        return true;
    }
    uint GetEquipSlotByIndex(ITEM_UNIT_INDEX itemIdx)
    {
        var it = m_EquipmentList.GetEnumerator();
        while(it.MoveNext())
        {
            if (it.Current.Value == null) continue;
            if (it.Current.Value.GetItemId() == itemIdx) return it.Current.Key;
        }
        return (uint)eEquipSlotSpot.eESS_Max;
    }
    // Remove from the equipment window with the item index
    public bool RemoveEquipItem(ITEM_UNIT_INDEX itemIdx)
    {
        uint slot = GetEquipSlotByIndex(itemIdx);
        return RemoveEquipItembySlot(slot);
    }
    // Remove items installed in slots
    public bool RemoveEquipItembySlot(uint equip_slot)
    {
        if (equip_slot < 0 || equip_slot >= (uint)eEquipSlotSpot.eESS_Max) return false;

        EA_CItemUnit prevItemUnit = m_EquipmentList[equip_slot];
        if (prevItemUnit != null) prevItemUnit.Release();
        m_EquipmentList[equip_slot] = null;
        return true;
    }
    public bool RemoveAllItem() 
    {
        for (uint slot = 0; slot < (uint)eEquipSlotSpot.eESS_Max; ++slot)
        {
            EA_CItemUnit itemUnit = m_EquipmentList[slot];
            if (itemUnit != null) itemUnit.Release();
            itemUnit = null;
        }
        return true;
    }
    
    // Find item units by item index
    public EA_CItemUnit FindEquipItem(ITEM_UNIT_INDEX itemIdx)
    {
        uint slot = GetEquipSlotByIndex(itemIdx);
        if (slot < 0 || slot >= (uint)eEquipSlotSpot.eESS_Max) return null;
        return m_EquipmentList[slot];
    }
    public EA_CItemUnit GetCurrentItem() 
    {
        if (nCurrEquipSlot < 0 || nCurrEquipSlot >= (uint)eEquipSlotSpot.eESS_Max) return null;
        return m_EquipmentList[nCurrEquipSlot];
    }
    public void EquipItem(EA_CCharBPlayer user,uint equip_slot)
    {
        if (equip_slot < 0 || equip_slot >= (uint)eEquipSlotSpot.eESS_Max) return;

        EA_CItemUnit itemUnit = m_EquipmentList[equip_slot];

        if (itemUnit == null) // Attempt to equip even when no weapon is present
        {
            nCurrEquipSlot = equip_slot;
            user.DoAttachItem(eAttachType.eWT_Max);
            return;
        }

        if (itemUnit.GetAttachType() == eAttachType.eWT_Max) return;

        EAObjID objId = SetActorItem(user, itemUnit);
        itemUnit.SetObjId(objId);
        nCurrEquipSlot = equip_slot;
        user.DoAttachItem(itemUnit.GetAttachType());
    }
    EAObjID SetActorItem(EA_CCharBPlayer user,EA_CItemUnit itemUnit)
    {
        if (user == null || itemUnit == null) return CObjGlobal.InvalidObjID;

        EAObjID objID = CObjGlobal.InvalidObjID;

        ObjectInfo objInfo = new ObjectInfo();
        
        objInfo.m_ModelTypeIndex = itemUnit.GetModelTypeIdx();
        objInfo.m_objClassType = itemUnit.GetObjClassType();

        if (itemUnit.GetAttachType() == eAttachType.eWT_Max) return objID;

        ItemObjInfo itemInfo = new ItemObjInfo();

        itemInfo.m_HavenUser = user.GetObjID();
        itemInfo.m_iItemIndex = itemUnit.GetItemId(); //  [4/7/2014 puos] The item ID of the item object 
        itemInfo.m_eItemType = itemUnit.GetItemObjectType();

        //  [3/21/2018 puos] If there is an item object created in the current slot, it is not created.
        //  Fixed bug that was created even with created item object
        EA_CItem pItem = EACObjManager.instance.GetGameObject(itemUnit.GetObjId()) as EA_CItem;

        if (pItem == null) pItem = EACObjManager.instance.CreateItem(objInfo,itemInfo);
        if (pItem == null) return objID;

        user.SetItemAttachment(itemUnit.GetAttachType(), pItem.GetLinkEntity());
        return objID;
    }
}
