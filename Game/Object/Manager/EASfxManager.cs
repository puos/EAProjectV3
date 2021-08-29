using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EAEffectID = System.UInt32;

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

    public EASfx CreateSfx(string effectName,float lifeTime = 0f)
    {
        EACEffectInfo info = new EACEffectInfo();
        info.m_eEffectState = eEffectState.ES_Load;
        info.m_EffectTableIndex = effectName;
        info.m_lifeTime = lifeTime;
        
        if (info.m_EffectId == CObjGlobal.InvalidEffectID) info.m_EffectId = m_IDGenerator.GenerateID();
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

    public void DeleteSfx(EAEffectID id)
    {
        if (!m_effects.TryGetValue(id, out EA_CEffectModule module)) return;

        module.ResetInfo(eEffectState.ES_UnLoad);
        m_effects.Remove(id);
        m_IDGenerator.FreeID(id);
    }
}
