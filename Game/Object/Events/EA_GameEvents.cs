using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EA_GameEvents
{
    public static bool showLog;
    public delegate void OnAttackMsg(EA_CCharBPlayer attacker, EA_CCharBPlayer victim, EAItemAttackWeaponInfo weaponInfo, uint projectileId);
    public static OnAttackMsg onAttackMsg = (EA_CCharBPlayer attacker, EA_CCharBPlayer victim, EAItemAttackWeaponInfo weaponInfo, uint projectileId) =>
    {
        uint attackerId = (attacker != null) ? attacker.GetObjID() : CObjGlobal.InvalidObjID;
        uint victimId = (victim != null) ? victim.GetObjID() : CObjGlobal.InvalidObjID;

        if (showLog) Debug.Log("LogicEvent - onAttackMsg attacker : " + attackerId + " victim : " + victimId +
             " weapon Type : " + weaponInfo.attachType + " projectile id : " + projectileId);
    };
}