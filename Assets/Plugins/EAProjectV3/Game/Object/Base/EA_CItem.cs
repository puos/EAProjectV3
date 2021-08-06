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
        SetObjInfo(m_ObjInfo);
        SetItemInfo(m_ItemInfo);
        return true;
    }

    public void SetLinkItem(EAItem item)
    {
        if (item == null) return;
        if (m_pLinkItem == null) return;

        m_pLinkItem = item;
        m_pLinkItem.SetItemBase(this);
    }

    public EAItem GetLinkItem() { return m_pLinkItem; }

    public override bool SetObjInfo(ObjectInfo objInfo)
    {
        m_ObjInfo = new ObjectInfo(objInfo);
        switch(m_ObjInfo.m_eObjState)
        {
            case eObjectState.CS_DEAD:
                {
                    if(m_pLinkItem != null)
                    {
                        m_pLinkItem.Release();
                        m_pLinkItem.SetItemBase(null);
                        m_pLinkItem = null;
                    }
                }
                break;
        }
        base.SetObjInfo(objInfo);
        switch(m_ObjInfo.m_eObjState)
        {
            case eObjectState.CS_SETENTITY:
                {
                    if(m_pLinkItem != null)
                    {
                        m_pLinkItem.Initialize();
                    }
                }
                break;
        }
        return true;
    }
}
