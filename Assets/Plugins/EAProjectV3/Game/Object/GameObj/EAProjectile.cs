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

    private Vector3 vStartPos = Vector3.zero;
    private Vector3 vStartDir = Vector3.zero;
    private Transform target = null;

    private float lifeTime = 0f;
    private float killDistance = 0f;

    public override void Initialize()
    {
        base.Initialize();

        if (cachedCollider == null) cachedCollider = gameObject.AddComponent<SphereCollider>();

        cachedCollider.isTrigger = true;
        target = null;

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

             EA_GameEvents.onAttackMsg(owner , actor , WeaponInfo , this );
        };
    }

    public void Move(Vector3 targetDir, float speed = 1f, Transform offset = null, float lifeTime = 0f, float killDistance = 0f)
    {
        vStartPos = (offset == null) ? tr.position : offset.position;
        vStartDir = targetDir;

        this.lifeTime = lifeTime;
        this.killDistance = killDistance;

        rb.velocity = vStartDir * speed;
        SetPos(vStartPos);
    }

    protected override void UpdatePerFrame()
    {
        base.UpdatePerFrame();

        if (killDistance == 0) return;

        if (Vector3.Distance(tr.position, vStartPos) > killDistance)
        {
            Release();
        }
    }

    public void SetWeaponInfo(EAItemAttackWeaponInfo WeaponInfo)
    {
        this.WeaponInfo = new EAItemAttackWeaponInfo(WeaponInfo);
    }

    private void OnDrawGizmos()
    {
        DebugExtension.DrawCircle(tr.position, Vector3.up, Color.magenta, GetBRadius());
    }
}
