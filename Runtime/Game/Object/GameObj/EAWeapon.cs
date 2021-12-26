﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EAWeapon : EAItem
{
    EAItemAttackWeaponInfo weaponInfo = new EAItemAttackWeaponInfo();

    bool bLock = true;
    float coolTime = 0;
    float updateCheckTime = 0;

    protected Transform muzzleTransform = null;

    public override void Initialize()
    {
        base.Initialize();
        muzzleTransform = tr;
        StopFire();
    }

    public override void Release()
    {
        base.Release();
        StopFire();
    }

    public virtual void StartFire()
    {
        bLock = false;
    }

    public virtual void StopFire()
    {
        bLock = true;
        updateCheckTime = 0;
    }

    protected virtual void FireEvent() 
    {
        FireShoot();
    }

    public void FireShoot()
    {
        EAActor actor = GetOwner();

        if (actor == null) return;
        if (muzzleTransform == null) return;
       
        ObjectInfo objInfo = new ObjectInfo();

        objInfo.m_ModelTypeIndex = weaponInfo.uProjectileModelType;
        objInfo.m_objClassType = weaponInfo.m_objProjectileClassType;
        objInfo.SetObjName("projectile");

        ItemObjInfo itemInfo = new ItemObjInfo();
        itemInfo.m_HavenUser = actor.Id;
        itemInfo.m_eItemType = eItemObjType.IK_Projectile;
        EA_CItem item = EACObjManager.instance.CreateItem(objInfo, itemInfo);
        EAProjectile projectile = item.GetLinkItem() as EAProjectile;

        projectile.SetPos(muzzleTransform.position);
        projectile.SetRotation(Quaternion.Euler(muzzleTransform.eulerAngles));
        projectile.SetWeaponInfo(weaponInfo);
        projectile.SetRotation(Quaternion.LookRotation(muzzleTransform.forward, Vector3.up));
        projectile.Move(muzzleTransform.forward, weaponInfo.fProjectileSpeed, muzzleTransform, 0, weaponInfo.fKillDistance);
        projectile.FireEvent();
    }

    public EAItemAttackWeaponInfo GetWeaponInfo() { return weaponInfo; }

    // Carry a weapon
    public void RaiseWeapon() 
    {
        EA_CItem itemBase = GetItemBase();

        if (itemBase == null) return;
        if (itemBase.GetItemInfo().m_iItemIndex == CObjGlobal.InvalidItemID) return;

        EA_CItemUnit pItemUnit = EA_ItemManager.instance.GetItemUnit(itemBase.GetItemInfo().m_iItemIndex);
        if (pItemUnit != null) weaponInfo = new EAItemAttackWeaponInfo(pItemUnit.GetAttackWeaponinfo());

        coolTime = weaponInfo.fFiringTime;

        StopFire();
    }

    protected override void UpdatePerFrame()
    {
        base.UpdatePerFrame();

        if (weaponInfo.bAutoMode == false) return;
        if (bLock == true) return;

        if (updateCheckTime >= Time.time) return;
        if (updateCheckTime < Time.time) updateCheckTime = Time.time;
        updateCheckTime += coolTime;

        FireEvent();
    }
}
