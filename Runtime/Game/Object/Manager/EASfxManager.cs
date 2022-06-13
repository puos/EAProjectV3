using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using EAEffectID = System.UInt32;

public struct EFxTag
{
    private int id;
    private string name;

    public EFxTag(EFxTag e)
    {
        id = e.id;
        name = e.name;
    }

    public EFxTag(int id, string name)
    {
        this.id = id;
        this.name = name;
    }
    public int Id { get { return id; } }
    public override string ToString() { return name; }
    public static bool operator !=(EFxTag lhs, EFxTag rhs) { return lhs.id != rhs.id; }
    public static bool operator ==(EFxTag lhs, EFxTag rhs) { return lhs.id == rhs.id; }

    public override bool Equals(object obj)
    {
        EFxTag rhs = (EFxTag)obj;
        if (rhs == null) return false;
        return id == rhs.id;
    }
    public override int GetHashCode() { return base.GetHashCode(); }
}

public class EASfxManager : EAGenericSingleton<EASfxManager>
{
    Dictionary<EAEffectID, EA_CEffectModule> m_effects = new Dictionary<EAEffectID, EA_CEffectModule>();

    //--------------------------------------------------------------------------
    //	effect index generation
    EAIDGenerator m_IDGenerator = null;
    //--------------------------------------------------------------------------

    public EASfxManager()
    {
        m_IDGenerator = new EAIDGenerator(50000);
    }

    public void Destroy()
    {
        var it = m_effects.GetEnumerator();
        while(it.MoveNext())
        {
            it.Current.Value.ResetInfo(eEffectState.ES_UnLoad);
        }
        m_effects.Clear();
        m_IDGenerator.ReGenerate();
    }

    private EASfx LoadSfx(EACEffectInfo info, float lifeTime)
    {
        info.m_EffectId = m_IDGenerator.GenerateID();
        if (m_effects.TryGetValue(info.m_EffectId, out EA_CEffectModule module))
        {
            module.SetObjInfo(info);
            if (lifeTime > 0) module.AutoDelete();
            return module.GetSfx();
        }

        module = new EA_CEffectModule();
        m_effects.Add(info.m_EffectId, module);
        module.SetObjInfo(info);
        if (lifeTime > 0) module.AutoDelete();
        return module.GetSfx();
    }

    public EASfx LoadFxWorld(EFxTag fxtag, Vector3 emitPos, Vector3 emitAngle, float lifeTime = 0f)
    {
        EACEffectInfo info = new EACEffectInfo();
        info.m_eEffectState = eEffectState.ES_Load;
        info.m_eAttachType = eEffectAttachType.eWorld;
        info.m_EffectTableIndex = fxtag.ToString();
        info.m_lifeTime = lifeTime;
        info.m_EmitPos = emitPos;
        info.m_EmitAngle = emitAngle;
        info.isSpawn = true;
        EASfx sfx = LoadSfx(info, lifeTime);
        return sfx;
    }

    public EASfx LoadFxOffset(EFxTag fxtag, EAObject obj, string attachBoneName, Vector3 emitPos, Vector3 emitAngle, float lifeTime = 0f)
    {
        EACEffectInfo info = new EACEffectInfo();
        info.m_eEffectState = eEffectState.ES_Load;
        info.m_eAttachType = eEffectAttachType.eLinkOffset;
        info.m_EffectTableIndex = fxtag.ToString();
        info.m_AttachObjectId = obj.GetObjId();
        info.m_AttachBoneName = attachBoneName;
        info.m_lifeTime = lifeTime;
        info.m_EmitPos = emitPos;
        info.m_EmitAngle = emitAngle;
        info.isSpawn = true;

        EASfx sfx = LoadSfx(info, lifeTime);
        return sfx;
    }
    public  EASfx LoadFxLinkBone(EFxTag fxtag, EAObject obj, string attachBoneName, float lifeTime = 0f)
    {
        EACEffectInfo info = new EACEffectInfo();
        info.m_eEffectState = eEffectState.ES_Load;
        info.m_eAttachType = eEffectAttachType.eLinkBone;
        info.m_EffectTableIndex = fxtag.ToString();
        info.m_AttachObjectId = obj.GetObjId();
        info.m_AttachBoneName = attachBoneName;
        info.m_lifeTime = lifeTime;
        info.isSpawn = true;

        EASfx sfx = LoadSfx(info, lifeTime);
        return sfx;
    }

    public EASfx LoadFx(EFxTag fxtag , float lifeTime = 0f)
    {
        EACEffectInfo info = new EACEffectInfo();
        info.m_eEffectState = eEffectState.ES_Load;
        info.m_eAttachType = eEffectAttachType.eNone;
        info.m_EffectTableIndex = fxtag.ToString();
        info.m_lifeTime = lifeTime;
        info.isSpawn = true;

        EASfx sfx = LoadSfx(info, lifeTime);
        return sfx;
    }

    public EASfx StartFxWorld(EFxTag fxtag, Vector3 emitPos, Vector3 emitAngle, float lifeTime = 0f)
    {
        EASfx sfx = LoadFxWorld(fxtag, emitPos, emitAngle, lifeTime);
        StartFx(sfx.effectId);
        return sfx;
    }

    public EASfx StartFxOffset(EFxTag fxtag,EAObject obj,string attachBoneName , Vector3 emitPos, Vector3 emitAngle, float lifeTime = 0f)
    {
        EASfx sfx = LoadFxOffset(fxtag, obj, attachBoneName, emitPos, emitAngle, lifeTime);
        StartFx(sfx.effectId);
        return sfx;
    }

    public EASfx StartFxLinkBone(EFxTag fxtag, EAObject obj, string attachBoneName,float lifeTime = 0f)
    {
        EASfx sfx = LoadFxLinkBone(fxtag, obj, attachBoneName, lifeTime);
        StartFx(sfx.effectId);
        return sfx;
    }

    public void StartFx(EAEffectID id)
    {
        if (!m_effects.TryGetValue(id, out EA_CEffectModule module)) return;
        module.ResetInfo(eEffectState.ES_Start);
    }

    public void DeleteFx(EAEffectID id)
    {
        if (!m_effects.TryGetValue(id, out EA_CEffectModule module)) return;
        module.ResetInfo(eEffectState.ES_UnLoad);
        m_effects.Remove(id);
        m_IDGenerator.FreeID(id);
    }

    // puos 20141019 Delete an effect associated with a specific actor
    public void DeleteRelatedFxActor(uint ActorId , bool bOnlyNotLoop = false)
    {
        List<uint> fxIdList = new List<uint>();

        var it = m_effects.GetEnumerator();

        while(it.MoveNext())
        {
            uint id = it.Current.Value.GetEffectInfo().m_EffectId;

            // Get the effect id corresponding to the actor
            if (it.Current.Value.GetEffectInfo().m_AttachObjectId != ActorId) continue;
            if(bOnlyNotLoop) 
            {
                // loop passes.
                if (it.Current.Value.GetEffectInfo().m_lifeTime > 0) fxIdList.Add(id);
            }
            if(!bOnlyNotLoop)
            {
                fxIdList.Add(id);
            }
        }

        // Delete the effect list.
        for (int i = 0; i < fxIdList.Count; ++i) DeleteFx(fxIdList[i]);
    }
}
