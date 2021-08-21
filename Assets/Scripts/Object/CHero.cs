using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHero : EAActor
{
    private Transform muzzle = null;
    private Transform turret = null;

    private Quaternion turret_rotation = Quaternion.identity;

    public override void Initialize()
    {
        base.Initialize();

        turret = GetTransform("TankTurret");

        actorMover.isReachedSetPos = false;

        Input.multiTouchEnabled = true;

        rb.useGravity = true;
        actorMover.SetSpeed(2f);
        if (turret != null) turret_rotation = turret.rotation;
        collisionEvent = (Collision c, EAObject myObj) => 
        {
            EAActor unit = c.gameObject.GetComponent<EAActor>();
            if (unit == null) return;
            Stop();
        };
    }

    protected override void UpdatePerFrame()
    {
        base.UpdatePerFrame();

        Vector3 velocity = rb.velocity;

        velocity.y = 0f;
        velocity.Normalize();

        if (velocity.magnitude > 0)
            SetRotation(Quaternion.LookRotation(velocity, Vector3.up), true, Time.deltaTime * 2.0f);

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
        //if (activeFire == false) return;

        //CBullet bullet = magazine.Get();

        //bullet.ownerId = Id;
        //bullet.SetActive(true);
        //bullet.SetRotation(Quaternion.LookRotation(muzzle.forward, Vector3.up));
        //bullet.Move(muzzle.forward, 5f, muzzle, 0, 23.0f);
    }

    public override void Release()
    {
        base.Release();
    }

    public void SetRotationSub(Quaternion rot)
    {
        turret_rotation = rot;
    }

    public void SetRotationSubReset()
    {
        turret.localRotation = Quaternion.identity;
    }

    public void Stop() { actorMover.Stop(); }

    public void SetSpeed(float speed) { actorMover.SetSpeed(speed); }

    public void MoveTo(Vector3 targetPosition, System.Action onMoveComplete = null) 
    { 
        actorMover.MoveTo(targetPosition, onMoveComplete); 
    }

    public static CHero MainPlayerCreate() 
    {
        EA_CCharUser mainPlayer = EACObjManager.instance.GetMainPlayer();
        ObjectInfo obj = mainPlayer.GetObjInfo();
        obj.m_ModelTypeIndex = "Player";
        obj.m_objClassType = typeof(CHero);
        mainPlayer.ResetInfo(eObjectState.CS_SETENTITY);
        return mainPlayer.GetLinkIActor() as CHero;
    }
}
