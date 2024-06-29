using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEditor.VersionControl.Asset;
using UnityEngine.PlayerLoop;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

partial class CEnemy
{
    public enum EFSMState { None = 0, Move, Chasing, Attack, Dead }

    private float updateTime= 0;

    public EFSMState state = EFSMState.None;

    public float UpdateTime
    {
        get { return updateTime; } 
        set {  updateTime = value; }
    }
    
    protected override void InitializeFSM()
    {
        base.InitializeFSM();
        
        updateTime = Time.time;
        state = EFSMState.None;

        fsmMaker.AddState(EFSMState.Chasing, Chasing , ChasingUpdate);
        fsmMaker.AddState(EFSMState.Attack, Attack , AttackUpdate);
    }

    protected override void UpdateFSM()
    {
        base.UpdateFSM();
        state = (EFSMState)fsmMaker.CurrentState;
    }

    public void AddState(EFSMState state,Action stateAction,Action updateAction)
    {
        fsmMaker.AddState(state,stateAction,updateAction);
    }

    public void ChangeFSMState(EFSMState newState)
    {
        updateTime = Time.time;
        fsmMaker.ChangeState(newState);
    }

    private void Chasing()
    {
        if (target == null)
        {
            ChangeFSMState(EFSMState.Move);
            return;
        }
        Debug.Log("enemy chase :" + Id + " -> " + target.Id);

        Vector3 targetPos = target.GetPos();
        Stop();

        Debug.Log("before enemy pos : " + GetPos() + " target pos : " + targetPos);

        MoveTo(targetPos);
    }

    private void ChasingUpdate() 
    {
        if (Time.time - updateTime <= 0) return;

        updateTime = Time.time + 2f;

        DebugExtension.DebugCircle(GetCenterPos(), Vector3.up, Color.black, 10f, 2f);

        if (Vector3.Distance(target.GetPos(), GetPos()) > 12.0f)
        {
            ChangeFSMState(CEnemy.EFSMState.Move);
            return;
        }

        if (Vector3.Distance(target.GetPos(), GetPos()) > 10.0f)
        {
            return;
        }

        ChangeFSMState(CEnemy.EFSMState.Attack);
    }

    private void Attack()
    {
        Vector3 dir = target.GetPos() - GetPos();
        dir.Normalize();

        Stop();
        SetRotationMuzzle(Quaternion.LookRotation(dir, Vector3.up));
        StartFire();
    }

    private void AttackUpdate()
    {
        if (Time.time - updateTime <= 0) return;
        updateTime = Time.time + 3f;

        if (Vector3.Distance(target.GetPos(), GetPos()) >= 10.0f)
        {
            ChangeFSMState(EFSMState.Chasing);
            return;
        }

        Attack();
    }

}
