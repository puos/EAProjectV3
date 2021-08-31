using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EA_CItem : EA_CObjectBase
{
    ItemObjInfo m_ItemInfo = new ItemObjInfo();
    protected EAItem m_pLinkItem = null;
    public EA_CItem() 
    {
    }
    public override eObjectKind GetKind() { return eObjectKind.CK_ITEM; }

    public override bool Initialize()
    {
        base.Initialize();

        if (m_pLinkItem != null) m_pLinkItem.Initialize();
        return true;
    }

    protected override bool Release()
    {
        base.Release();

        if (m_pLinkItem != null) m_pLinkItem.Release();
        if (m_pLinkItem != null) m_pLinkItem.SetItemBase(null);
        m_pLinkItem = null;

        return true;
    }

    public bool SetItemInfo(ItemObjInfo itemInfo)
    {
        m_ItemInfo = new ItemObjInfo(itemInfo);
        return true;
    }

    public virtual bool Use()
    {
        if (m_pLinkItem == null) return false;

        m_pLinkItem.Use();
        return true;
    }

    public ItemObjInfo GetItemInfo() 
    {
        return m_ItemInfo;
    }

    public override bool ResetInfo(eObjectState eChangeState)
    {
        m_ObjInfo.m_eObjState = eChangeState;
        m_ObjInfo.isSpawn = false;
        SetObjInfo(m_ObjInfo);
        SetItemInfo(m_ItemInfo);
        return true;
    }

    public void SetLinkItem(EAItem item)
    {
        if (item == null) return;
        if (m_pLinkItem == item) return;

        m_pLinkItem = item;
        m_pLinkItem.SetItemBase(this);
    }

    public EAItem GetLinkItem() { return m_pLinkItem; }
}
