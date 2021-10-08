using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EA_CCharNPC : EA_CCharBPlayer
{
    NPCInfo m_NPCInfo = new NPCInfo();

    public EA_CCharNPC() { }

    public bool SetNPCInfo(NPCInfo npcInfo)
    {
        m_NPCInfo = npcInfo;
        return true;
    }
    public NPCInfo GetNPCInfo() 
    {
        return m_NPCInfo;
    }
    public override bool ResetInfo(eObjectState eChangeState)
    {
        m_ObjInfo.m_eObjState = eChangeState;
        m_ObjInfo.isSpawn = false;
        SetObjInfo(m_ObjInfo);
        SetNPCInfo(m_NPCInfo);
        return true;
    }
}