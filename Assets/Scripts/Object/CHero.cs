using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHero : EAActor
{
    private Transform muzzle = null;
    private Transform turret = null;
    private Quaternion turret_rotation = Quaternion.identity;
    private EAActorMover actorMover = null;
    
    public override void Initialize()
    {
        base.Initialize();

        turret = GetTransform("TankTurret");

        rb.useGravity = true;
    
        if (turret != null) turret_rotation = turret.rotation;
        collisionEvent = (Collision c, EAObject myObj) => 
        {
            EAActor unit = c.gameObject.GetComponent<EAActor>();
            if (unit == null) return;
            Stop();
        };
    }

    public override void InitializeAI()
    {
        base.InitializeAI();

        if (actorMover == null) actorMover = new EAActorMover();
        actorMover.Initialize(steering);
        actorMover.SetSpeed(2f);
        AddAgent("hero");
    }

    public override void UpdateAI()
    {
        base.UpdateAI();
        if (actorMover != null) actorMover.AIUpdate();
    }

    protected override void UpdatePerFrame()
    {
        base.UpdatePerFrame();

        Vector3 velocity = GetVelocity();

        velocity.y = 0f;
        velocity.Normalize();

        if (velocity.magnitude > 0)
            SetRotation(Quaternion.LookRotation(velocity, Vector3.up), Time.deltaTime * 2.0f);

        if (turret != null)
        {
            if (!Quaternion.Equals(turret.rotation, turret_rotation))
            {
                turret.rotation = Quaternion.Lerp(turret.rotation, turret_rotation, Time.deltaTime * 4.0f);
            }
        }
    }

    public void StartFire()
    {
        EASfxManager.instance.StartFxOffset(TankEFxTag.FireFx, this , string.Empty , Vector3.zero , Vector3.zero , 1f);

        if (currWeapon != null) currWeapon.FireShoot();
    }

    public override bool SetItemAttachment(eAttachType attachType, EAObject gameObject)
    {
        if (attachType == eAttachType.eWT_Turret) EAFrameUtil.SetParent(gameObject.tr, turret);
        return true;
    }

    public override void DoAttachItem(eAttachType attachType, eItemType itemType)
    {
        base.DoAttachItem(attachType, itemType);
        if (itemType != eItemType.eIT_Weapon) return; 
        if(currWeapon != null) currWeapon.StartFire();
    }

    public override void Release()
    {
        base.Release();
    }

    public void SetRotationMuzzle(Quaternion rot)
    {
        turret_rotation = rot;
    }

    public void SetRotationSubReset()
    {
        turret.localRotation = Quaternion.identity;
    }

    public override void Stop()
    {
        steering.ResetInfo();
        base.Stop();
    }

    public void SetSpeed(float speed) { actorMover.SetSpeed(speed); }

    public void MoveTo(Vector3 targetPosition, System.Action onMoveComplete = null) 
    { 
        actorMover.MoveTo(targetPosition, onMoveComplete);
        SetRotationSubReset();
    }

    public static CHero MainPlayerCreate() 
    {
        EA_CCharUser mainPlayer = EACObjManager.instance.GetMainPlayer();
        ObjectInfo obj = mainPlayer.GetObjInfo();
        obj.m_ModelTypeIndex = "Player";
        obj.m_objClassType = typeof(CHero);
        mainPlayer.ResetInfo(eObjectState.CS_SETENTITY);

        CHero hero = mainPlayer.GetLinkIActor() as CHero;
        hero.Initialize();

        EAItemInfo info = new EAItemInfo();
        info.m_eItemType = eItemType.eIT_Weapon;
        info.m_objClassType = typeof(EAWeapon);
        info.m_ModelTypeIndex = "Barrel";

        EAItemAttackWeaponInfo weaponinfo = new EAItemAttackWeaponInfo();

        weaponinfo.id = "Barrel";
        weaponinfo.attachType = eAttachType.eWT_Turret;
        weaponinfo.bAutoMode = false;
        weaponinfo.uProjectileModelType = "Bullet";
        weaponinfo.m_objProjectileClassType = typeof(CBullet);
        weaponinfo.fProjectileSpeed = 5f;
        weaponinfo.fKillDistance = 23f;

        EA_CItemUnit unit = EA_ItemManager.instance.CreateItemUnit(info);
        unit.SetAttackWeaponInfo(weaponinfo);
        EA_ItemManager.instance.InsertEquip(mainPlayer.GetObjID(), 0, unit);
        EA_ItemManager.instance.EquipmentItem(mainPlayer.GetObjID(), 0);

        return hero;
    }
}
