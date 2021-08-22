using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EA_CMapObject : EA_CObjectBase
{
    MapObjInfo m_mapObjInfo = new MapObjInfo();
   
    protected EAMapObject m_pLinkMapObject = null;

    public EA_CMapObject()
    {      
    }

    public override eObjectKind GetKind(){ return eObjectKind.CK_MAP; }

    public override bool Initialize()
    {
        base.Initialize();
        if (m_pLinkMapObject != null) m_pLinkMapObject.Initialize();
        return true;
    }

    public void SetMapInfo(MapObjInfo mapInfo)
    {
        m_mapObjInfo = new MapObjInfo(mapInfo);
    }

    public void SetLinkMapObject(EAMapObject pMapObject)
    {
        if (pMapObject == null) return;
        if (m_pLinkMapObject == pMapObject) return;

        m_pLinkMapObject = pMapObject;
        m_pLinkMapObject.SetMapBase(this);
    }
}