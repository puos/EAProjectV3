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

        this.triggerEvent = (Collider c,EAObject obj) => 
        {
             EAActor actor = c.gameObject.GetComponent<EAActor>();
             EAActor owner = GetOwner();
             // [4/11/2018 puos] attacker is not dead
             if (actor == null) return;
             
             if(owner != null)
             {
                //  [12/2/2019 puos] If attacker and attacked are the same, passing
                if (owner.GetCharBase().GetObjID() == actor.GetCharBase().GetObjID()) return;
             }
                         
            EA_GameEvents.onAttackMsg((owner != null) ? owner.GetCharBase() : null,
                actor.GetCharBase(), WeaponInfo , GetItemBase().GetObjID());
        };
    }

    public void SetWeaponInfo(EAItemAttackWeaponInfo WeaponInfo)
    {
        this.WeaponInfo = new EAItemAttackWeaponInfo(WeaponInfo);
    }

    private void OnDrawGizmos()
    {
        DebugExtension.DrawCircle(cachedTransform.position, Vector3.up, Color.magenta, GetBRadius());
    }
}
