using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EAActorMover 
{
    private EAAIAgent aiAgent = null;

    private EASteeringBehaviour steeringBehaviour = null;
    protected System.Action onMoveComplete = null;

    public bool AIOn { get; set; }

    public void Initialize(EAAIAgent aiAgent)
    {
        this.aiAgent = aiAgent;
        steeringBehaviour = new EASteeringBehaviour(aiAgent);
        AIOn = true;
    }

    public void Release()
    {
        EAGamePhysicWorld.instance.RemoveAgent(aiAgent); 
    }

    public void AIUpdate()
    {
        if (AIOn == false) return;

        //keep a record of its old position so we can update its cell later
        //in this method
        Vector3 oldVelocity = aiAgent.GetVelocity();

        //calculate the combined force from each steering behavior in the 
        //vehicle's list
        Vector3 changedVelocity = steeringBehaviour.Calculate();
        
        aiAgent.AIUpdate(changedVelocity , Time.deltaTime);
        
        //moveState
        if(steeringBehaviour.IsSteering())
        {
            float epsillon = aiAgent.GetEpsilon();
            Vector3 vel = aiAgent.GetVelocity();

            if (EAMathUtil.Equal(vel, Vector3.zero , epsillon) &&
                oldVelocity.magnitude >= epsillon)
            {
                if (onMoveComplete != null) onMoveComplete();
            }
        }  
    }

    public void MoveTo(Vector3 targetPosition, System.Action onMoveComplete = null)
    {
        aiAgent.SetVTarget(targetPosition);
        steeringBehaviour.ArriveOn();
        this.onMoveComplete = onMoveComplete;
    }

    public void SetSpeed(float speed,float epsilon = 0.01f)
    {
        aiAgent.SetMaxSpeed(speed);
        aiAgent.SetEpsilon(epsilon);
    }

    public void Stop()
    {
        steeringBehaviour.DefaultOn();
        aiAgent.StopMove();
    }
}