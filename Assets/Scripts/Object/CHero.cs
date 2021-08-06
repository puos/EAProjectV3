using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHero : EAActor
{
    protected float fShootCoolTime = 0;
    protected float updateCheckTime = 0;

    [SerializeField] private Transform muzzle = null;
    [SerializeField] private Transform turret = null;

    private Quaternion turret_rotation = Quaternion.identity;

    public override void Initialize()
    {
        base.Initialize();

        actorMover.isReachedSetPos = false;

        Input.multiTouchEnabled = true;

        rb.useGravity = true;
       
        actorMover.SetSpeed(400);
        fShootCoolTime = 2f;
        if (turret != null) turret_rotation = turret.rotation;
    }

    protected override void UpdatePerFrame()
    {
        base.UpdatePerFrame();

        Vector3 velocity = rb.velocity;

        velocity.y = 0f;
        velocity.Normalize();

        if (velocity.magnitude > 0)
            SetRotation(Quaternion.LookRotation(velocity, Vector3.up), true, Time.deltaTime * 2.0f);


        if(!Quaternion.Equals(turret.rotation, turret_rotation))
        {
            if (turret != null) turret.rotation = Quaternion.Lerp(turret.rotation, turret_rotation, Time.deltaTime * 4.0f);
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

        //if(magazine != null) magazine.Release();
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
}
