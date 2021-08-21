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

    private void Chasing(CEnemy enemy)
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
            enemy.SetRotation(Quaternion.LookRotation((targetPos - enemy.GetPos()).normalized, Vector3.up));

            Debug.Log("before enemy pos : " + enemy.GetPos() + " target pos : " + targetPos);

            enemy.MoveTo(targetPos);
        });

        float updateTime = Time.time;

        enemy.StateAdd(CEnemy.EFSMState.Chasing, () =>
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

    private void Attack(CEnemy enemy)
    {
        float updateTime = Time.time;

        enemy.StateAdd(CEnemy.EFSMState.Attack, () =>
        {
            Vector3 dir = enemy.target.GetPos() - enemy.GetPos();
            dir.Normalize();

            enemy.Stop();
            enemy.SetRotationSub(Quaternion.LookRotation(dir, Vector3.up));
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
}
