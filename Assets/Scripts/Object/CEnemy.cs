using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEnemy : EAActor
{
    public enum EFSMState { None = 0, Move, Chasing, Attack, Dead }

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

    protected override void UpdatePerFrame()
    {
        base.UpdatePerFrame();

        FSMUpdate();
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
    public void ChangeFSMState(EFSMState newState)
    {
        base.ChangeFSMState((int)newState);
    }

    public void StateAdd(EFSMState state,System.Action action)
    {
        if (states.TryGetValue((int)state, out System.Action value)) return;

        states.Add((int)state, action);
    }    

    public void UpdateAdd(EFSMState state, System.Action action)
    {
        if (updates.TryGetValue((int)state, out System.Action value)) return;

        updates.Add((int)state, action);
    }

    public void CmdState(EFSMState state)
    {
        if (!states.TryGetValue((int)state, out System.Action value)) return;

        if(value != null) value();
    }

    private static void Chasing(CEnemy enemy)
    {
        enemy.StateAdd(CEnemy.EFSMState.Chasing, () =>
        {
            if (enemy.target == null)
            {
                enemy.ChangeFSMState((int)CEnemy.EFSMState.Move);
                return;
            }

            Debug.Log("enemy chase :" + enemy + " -> " + enemy.target.Id);

            Vector3 targetPos = enemy.target.GetPos();
            enemy.Stop();

            Debug.Log("before enemy pos : " + enemy.GetPos() + " target pos : " + targetPos);

            enemy.MoveTo(targetPos);
        });

        float updateTime = Time.time;

        enemy.UpdateAdd(CEnemy.EFSMState.Chasing, () =>
        {
            if (Time.time - updateTime <= 0) return;

            updateTime = Time.time + 2f;

            DebugExtension.DebugCircle(enemy.GetCenterPos(), Vector3.up, Color.black, 10f, 2f);

            if (Vector3.Distance(enemy.target.GetPos(), enemy.GetPos()) > 12.0f)
            {
                enemy.ChangeFSMState(CEnemy.EFSMState.Move);
                return;
            }

            if (Vector3.Distance(enemy.target.GetPos(), enemy.GetPos()) > 10.0f)
            {
                return;
            }

            enemy.ChangeFSMState(CEnemy.EFSMState.Attack);
        });
    }

    private static void Attack(CEnemy enemy)
    {
        float updateTime = Time.time;

        enemy.StateAdd(CEnemy.EFSMState.Attack, () =>
        {
            Vector3 dir = enemy.target.GetPos() - enemy.GetPos();
            dir.Normalize();

            enemy.Stop();
            enemy.SetRotationMuzzle(Quaternion.LookRotation(dir, Vector3.up));
            enemy.StartFire();
        });

        enemy.UpdateAdd(CEnemy.EFSMState.Attack, () =>
        {
            if (Time.time - updateTime <= 0) return;
            updateTime = Time.time + 3f;

            if (Vector3.Distance(enemy.target.GetPos(), enemy.GetPos()) >= 10.0f)
            {
                enemy.ChangeFSMState((int)CEnemy.EFSMState.Chasing);
                return;
            }

            enemy.CmdState(CEnemy.EFSMState.Attack);

        });
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

        Chasing(enemy);
        Attack(enemy);
        
        return enemy;
    }
}
