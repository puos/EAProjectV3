using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EA_CCharMob : EA_CCharBPlayer
{
    MobInfo m_MobInfo = new MobInfo();
    public EA_CCharMob() 
    {
    }

    public bool SetMobInfo(MobInfo mobInfo)
    {
        m_MobInfo = mobInfo;
        return true;
    }

    MobInfo GetMobInfo() { return m_MobInfo; }
}
