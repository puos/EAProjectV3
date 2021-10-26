using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EAActorMover : MonoBehaviour
{
    private EAAIAgent aiAgent = null;

    private EASteeringBehaviour steeringBehaviour = null;
    protected OnMoveComplete onMoveComplete = null;
    public enum MoveCompleteState { Reached , Landed }

    public delegate void OnMoveComplete(EAAIAgent aiAgent, EASteeringBehaviour sbehaviour, MoveCompleteState state);
    
    public void Initialize()
    {
        aiAgent = GetComponent<EAActor>();
        steeringBehaviour = new EASteeringBehaviour(aiAgent);
    }

    public void Release()
    {
        EAGamePhysicWorld.instance.RemoveAgent(aiAgent); 
    }

    public void AIUpdate()
    {
        //keep a record of its old position so we can update its cell later
        //in this method
        Vector3 oldVelocity = aiAgent.GetVelocity();

        //calculate the combined force from each steering behavior in the 
        //vehicle's list
        Vector3 changedVelocity = steeringBehaviour.Calculate();
        
        aiAgent.AIUpdate(changedVelocity , Time.deltaTime);
        
        
        
        bool reached = false;

        if ((curDir.magnitude > 0.01f) && (Vector3.Dot(curDir, changedVelocity) < 0)) reached = true;
        if (changedVelocity.magnitude <= 0.01f && Vector3.Distance(actor.GetPos(), targetPosition) <= arriveEpsilon) reached = true;

        if (reached)
        {
             if (onMoveComplete != null) onMoveComplete();
        }
    }

    public void MoveTo(Vector3 targetPosition, OnMoveComplete onMoveComplete = null)
    {
        this.targetPosition = targetPosition;
        this.onMoveComplete = onMoveComplete;
         
    }

    public void SetSpeed(float speed , float epsilon = 0.01f)
    {
        aiAgent.SetMaxSpeed(speed);
        arriveEpsilon = epsilon;
    }

    public void Stop()
    {
        this.targetPosition = Vector3.zero;
        this.onMoveComplete = null;
        isMove = false;

        actor.rb.isKinematic = true;
        actor.rb.velocity = Vector3.zero;
        actor.rb.angularVelocity = Vector3.zero;
        actor.rb.isKinematic = false;
    }
}