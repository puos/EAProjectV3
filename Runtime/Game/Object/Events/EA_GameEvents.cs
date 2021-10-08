using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EA_GameEvents
{
    public static bool showLog;
    public delegate void OnAttackMsg(EAActor attacker, EAActor victim, EAItemAttackWeaponInfo weaponInfo, EAItem item);
    public static OnAttackMsg onAttackMsg = (EAActor attacker, EAActor victim , EAItemAttackWeaponInfo weaponInfo, EAItem item) =>
    {
        if (showLog) Debug.Log("LogicEvent - onAttackMsg attacker : " + attacker.Id + " victim : " + victim.Id +
             " weapon Type : " + weaponInfo.attachType + " item id : " + item.Id);
    };
}