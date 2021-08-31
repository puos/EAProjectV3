using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_CCharUser : EA_CCharBPlayer
{
    CharInfo m_ActorInfo = null;
    public EA_CCharUser()
    {
    }
    public bool SetCharInfo( CharInfo charInfo )
    {
        m_ActorInfo = charInfo;
        return true;
    }
    public CharInfo GetCharInfo()
    {   return m_ActorInfo;
    }
    public override bool ResetInfo(eObjectState eChangeState)
    {
        m_ObjInfo.m_eObjState = eChangeState;
        m_ObjInfo.isSpawn = false;
        SetObjInfo(m_ObjInfo);
        SetCharInfo(m_ActorInfo);
        return true;
    }

    /// <summary>
    /// Change if there are set parts
    /// </summary>
    /// <returns></returns>
    public override bool ChangeParts()
    {
        EAActor actor = GetLinkIActor();
        if(actor != null)
        {
            
        }

        return true;
    }
}
