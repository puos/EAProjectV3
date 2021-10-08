using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EAMapObject : EAObject
{
    EA_CMapObject m_pDiaObjectBase = null;

    public uint Id { get { return (m_pDiaObjectBase != null) ? m_pDiaObjectBase.GetObjID() : CObjGlobal.InvalidObjID; } }

    public void SetMapBase(EA_CMapObject pDiaMapBase)
    {
        m_pDiaObjectBase = pDiaMapBase;
    }

    public EA_CMapObject GetMapBase()
    {
        return m_pDiaObjectBase;
    }

    public override uint GetObjId() { return Id; }

    public override void Release()
    {
        base.Release();

        EACObjManager.instance.DeleteGameObject(eObjectType.CT_MAPOBJECT , Id);
    }

}
