﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EAWeapon : EAItem
{
    EAItemAttackWeaponInfo weaponInfo = new EAItemAttackWeaponInfo();

    bool bLock = false;
    float coolTime = 0;

    protected Transform muzzleTransform = null;

    public override void Initialize()
    {
        base.Initialize();
        muzzleTransform = cachedTransform;
    }

    public virtual void StartFire()
    {
        coolTime = 0f;
    }

    public virtual void StopFire()
    {

    }

    public void FireShoot()
    {
        EAActor actor = GetOwner();

        if (actor == null) return;
        if (muzzleTransform == null) return;

        ObjectInfo objInfo = new ObjectInfo();

        objInfo.spawnPos = muzzleTransform.position;
        objInfo.spawnAngle = muzzleTransform.eulerAngles;

        objInfo.m_ModelTypeIndex = weaponInfo.uProjectileModelType;
        objInfo.m_objClassType = weaponInfo.m_objProjectileClassType;
        objInfo.SetObjName("projectile");

        ItemObjInfo itemInfo = new ItemObjInfo();
        itemInfo.m_HavenUser = actor.Id;
        itemInfo.m_eItemType = eItemObjType.IK_Projectile;
        EA_CItem item = EACObjManager.instance.CreateItem(objInfo, itemInfo);
        EAProjectile projectile = item.GetLinkItem() as EAProjectile;
        projectile.SetWeaponInfo(weaponInfo);
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

        StopFire();
    }
}
