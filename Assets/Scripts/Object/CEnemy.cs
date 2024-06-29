using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class CEnemy : EAActor
{
    [SerializeField] private Transform muzzle = null;
    [SerializeField] private Transform turret = null;

    private EAActorMover actorMover = null;

    public EAActor target { get; private set; }

    public override void Initialize()
    {
        base.Initialize();
        
        turret = GetTransform("TankTurret");

        rb.useGravity = true;

        target = null;
              
        collisionEvent = (Collision c, EAObject myObj) =>
        {
            EAActor unit = c.gameObject.GetComponent<EAActor>();
            if (unit != null) Stop();

            CSubCube map = c.gameObject.GetComponent<CSubCube>();
            if (map != null) Stop();
        };
    }

    public override void InitializeAI()
    {
        base.InitializeAI();

        if (actorMover == null) actorMover = new EAActorMover();
        actorMover.Initialize(steering);
        actorMover.SetSpeed(3.5f);
        actorMover.LookOn = true;
        AddAgent("enemy");
    }

    public override void UpdateAI()
    {
        base.UpdateAI();
        if (actorMover != null) actorMover.AIUpdate();
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
        if (currWeapon != null) currWeapon.StartFire();
    }

    public void StartFire()
    {
        EASfxManager.instance.StartFxOffset(TankEFxTag.FireFx, this, string.Empty, Vector3.zero, Vector3.zero, 1f);
        if (currWeapon != null) currWeapon.FireShoot();
    }

    public void SetRotationMuzzle(Quaternion rot)
    {
        if (turret != null) turret.rotation = rot;
    }

    public void SetTarget(EAActor target)
    {
        this.target = target;
    }

    public void MoveTo(Vector3 targetPosition, System.Action onMoveComplete = null)
    {
        actorMover.MoveTo(targetPosition, onMoveComplete);
    }
    
    public static CEnemy Clone() 
    {
        ObjectInfo obj = new ObjectInfo();

        obj.m_ModelTypeIndex = "Enemy";
        obj.m_objClassType = typeof(CEnemy);

        MobInfo mobInfo = new MobInfo();
        EA_CCharMob mob = EACObjManager.instance.CreateMob(obj, mobInfo);

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
        EA_ItemManager.instance.InsertEquip(mob.GetObjID(), 0, unit);
        EA_ItemManager.instance.EquipmentItem(mob.GetObjID(), 0);

        CEnemy enemy = mob.GetLinkIActor() as CEnemy;
        
        return enemy;
    }
}
