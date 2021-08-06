using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEnemy : EAActor
{
    public enum EFSMState { None = 0, Move, Chasing, Attack, Dead }

    [SerializeField] private Transform muzzle = null;
    [SerializeField] private Transform turret = null;

    public EAActor target { get; private set; }

    public override void Initialize()
    {
        base.Initialize();
        rb.useGravity = true;

        target = null;
        actorMover.isReachedSetPos = false;
    }

    public override void Release()
    {
        base.Release();
    }

    protected override void UpdatePerFrame()
    {
        base.UpdatePerFrame();

        FSMUpdate();
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

    public void SetRotationSub(Quaternion rot)
    {
        if (turret != null) turret.rotation = rot;
    }

    public void SetTarget(EAActor target)
    {
        this.target = target;
    }

    public void Stop() { actorMover.Stop(); }

    public void SetSpeed(float speed) { actorMover.SetSpeed(speed); }

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
}
