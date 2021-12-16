﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EAActorMover 
{
    public EAAIAgent aiAgent { get; private set; }

    private EASteeringBehaviour steeringBehaviour = null;
    protected System.Action onMoveComplete = null;

    public bool AIOn { get; set; }
    public bool LookOn { get; set; }
    public bool LookAtTarget { get; set; }
    public float smoothRatio { get; set; }

    public EASteeringBehaviour Steering { get { return steeringBehaviour; } }

    public void Initialize(EAAIAgent aiAgent)
    {
        this.aiAgent = aiAgent;
        steeringBehaviour = new EASteeringBehaviour(aiAgent);
        AIOn = true;
        LookOn = false;
        LookAtTarget = false;
        smoothRatio = 0.25f;
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
        Vector3 force = steeringBehaviour.Calculate();

        aiAgent.AIUpdate(force, Time.deltaTime);
        
        //moveState
        if(steeringBehaviour.IsSteering())
        {
            float epsillon = aiAgent.GetEpsilon();
            Vector3 vel = aiAgent.GetVelocity();

            if (EAMathUtil.Equal(vel, Vector3.zero , epsillon) &&
                (oldVelocity.magnitude - vel.magnitude) >= 0f)
            {
                if (LookOn) aiAgent.SetHeading(vel);
                if (LookAtTarget) LookAtDirection(aiAgent.VTarget() - aiAgent.GetPos());
                if (onMoveComplete != null) onMoveComplete();
                return;
            }

            if (LookOn) aiAgent.SetHeading(vel, smoothRatio);
            if (LookAtTarget) LookAtDirection(aiAgent.VTarget() - aiAgent.GetPos(), smoothRatio);
        }  
    }

    protected void LookAtDirection(Vector3 direction,float smoothRatio = 1f)
    {
        direction.Normalize();
        aiAgent.SetHeading(direction, smoothRatio);
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

    public void MoveToPath(List<Vector3> paths,bool isLoop = false , System.Action onMoveComplete = null)
    {
        steeringBehaviour.SetPath(paths, isLoop);
        steeringBehaviour.FollowPathOn();
        this.onMoveComplete = onMoveComplete;
    }
}