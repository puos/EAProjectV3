using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using EAObjID = System.UInt32;

public class EAProjectile : EAItem
{
    protected EAItemAttackWeaponInfo WeaponInfo = new EAItemAttackWeaponInfo();

    public override void Initialize()
    {
        base.Initialize();

        if (cachedCollider == null) cachedCollider = gameObject.AddComponent<SphereCollider>();
    }

    public void SetWeaponInfo(EAItemAttackWeaponInfo WeaponInfo)
    {
        this.WeaponInfo = new EAItemAttackWeaponInfo(WeaponInfo);
    }
}
