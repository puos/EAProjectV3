using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EAMapObject : EAObject
{
    EA_CMapObject m_pDiaObjectBase = null;

    public void SetMapBase(EA_CMapObject pDiaMapBase)
    {
        m_pDiaObjectBase = pDiaMapBase;
    }

    public EA_CMapObject GetMapBase()
    {
        return m_pDiaObjectBase;
    }

}
