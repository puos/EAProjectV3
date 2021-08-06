using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ITEM_UNIT_INDEX = System.UInt32;
using EAObjID = System.UInt32;

public class EA_ItemManager : EAGenericSingleton<EA_ItemManager>
{
    Dictionary<ITEM_UNIT_INDEX, EA_CItemUnit> m_mapItemUnitList = new Dictionary<ITEM_UNIT_INDEX, EA_CItemUnit>();
    
    Dictionary<EAObjID, EA_Equipment> m_PCEquipMap = new Dictionary<EAObjID, EA_Equipment>(); //  [4/7/2014 puos] npc and mob item management
    EA_Equipment m_pMyEquipment = new EA_Equipment();  //  [4/7/2014 puos] My Item Management

    //	unique index for create and release
    EAIDGenerator m_IDGenerater = new EAIDGenerator(10000);

    public void Destroy()
    {
        m_pMyEquipment.RemoveAllItem();
        m_IDGenerater.ReGenerate();
        var it = m_PCEquipMap.GetEnumerator();
        while(it.MoveNext()) 
        {
            it.Current.Value.RemoveAllItem();  
        }
        m_PCEquipMap.Clear();
    }
    // Create the corresponding actor equipment item with id.
    public bool InsertPCEquip(EAObjID id, EA_Equipment pPLEquip)
    {
        if (m_PCEquipMap.TryGetValue(id, out EA_Equipment pEquipment)) return false;

        m_PCEquipMap.Add(id, pPLEquip);
        return true;
    }
    // Delete the PC device by id.
    public bool RemovePCEquip(EAObjID id)
    {
        if (!m_PCEquipMap.TryGetValue(id, out EA_Equipment pEquipment)) return false;
        
        pEquipment.RemoveAllItem();
        m_PCEquipMap.Remove(id);
        return true;
    }
    // Find the player equipment by id
    public EA_Equipment GetPCEquipItem(EAObjID id)
    {
        m_PCEquipMap.TryGetValue(id, out EA_Equipment pEquipment);
        return pEquipment;
    }
    public bool RemoveEquip(EAObjID id)
    {
        EA_CCharBPlayer actor = EACObjManager.instance.GetActor(id);
        
        if (actor == null) return false;
        if (actor.GetObjInfo().m_eObjType == eObjectType.CT_MYPLAYER) return false;
        
        RemovePCEquip(id);
        return true;
    }
    EA_CItemUnit CreateItemUnit(EAItemInfo info)
    {
        if (CObjGlobal.InvalidItemID == info.m_EAItemId) info.m_EAItemId = (EAObjID)m_IDGenerater.GenerateID();

        EA_CItemUnit itemUnit = new EA_CItemUnit();
        itemUnit.SetItemInfo(info);
        m_mapItemUnitList.Add(info.m_EAItemId, itemUnit);
        return itemUnit;
    }
    public bool DeleteItemUnit(ITEM_UNIT_INDEX id)
    {
        if (!m_mapItemUnitList.TryGetValue(id, out EA_CItemUnit itemUnit)) return false;

        if (itemUnit == null) return false;
        if (itemUnit.GetObjId() == CObjGlobal.InvalidObjID) return false;

        EACObjManager.instance.RemoveItem(itemUnit.GetObjId());

        m_mapItemUnitList.Remove(id);
        m_IDGenerater.FreeID(id);

        return true;
    }
}
