using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EA_CMapObject : EA_CObjectBase
{
    public EA_CMapObject()
    {
        m_pLinkMapObject = null;
    }

    public override eObjectKind GetKind(){ return eObjectKind.CK_MAP; }

    public void SetLinkMapObject(EAMapObject pMapObject)
    {
        if (pMapObject == null) return;
        if (m_pLinkMapObject == pMapObject) return;

        m_pLinkMapObject = pMapObject;
        m_pLinkMapObject.SetMapBase(this);
    }




    protected EAMapObject m_pLinkMapObject = null;
}