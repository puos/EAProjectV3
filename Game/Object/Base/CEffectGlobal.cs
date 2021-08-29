using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using EAEffectID = System.UInt32;
using EAObjID = System.UInt32;

public enum eEffectLiftType // life type of node
{
    eLoop = 0,   // loop
    eLimitTime,  // limit
}

public enum eEffectAttachType // attachment type of node
{
    eLinkBone = 0,   // link bone
    eLinkOffset,     // Link to Object / Avatar Offset
    eWorld,          // game world
}

public enum eEffectState
{
    ES_Start,
    ES_Load,
    ES_Stop,
    ES_Skip,
    ES_UnLoad,
}

public class EACEffectInfo
{
    public EAEffectID m_EffectId;
    public EAObjID    m_AttachObjectId;
    public float      m_lifeTime;
    public string     m_AttachBoneName;
    public string     m_EffectTableIndex;
    public Vector3    m_EmitPos;
    public Vector3    m_EmitAngle;
    public eEffectState m_eEffectState;
    public eEffectAttachType m_eAttachType;

    public EACEffectInfo() { }

    public EACEffectInfo(EACEffectInfo info)
    {
        m_EffectId = info.m_EffectId;
        m_AttachObjectId = info.m_AttachObjectId;
        m_AttachBoneName = info.m_AttachBoneName;
        m_EffectTableIndex = info.m_EffectTableIndex;
        m_EmitPos = info.m_EmitPos;
        m_EmitAngle = info.m_EmitAngle;
        m_eEffectState = info.m_eEffectState;
        m_eAttachType = info.m_eAttachType;
        m_lifeTime = info.m_lifeTime;
    }
}
