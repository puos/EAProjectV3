using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EAActorMover 
{
    private EASteeringBehaviour steering = null;
    private Arrive arrive = null;
    private FollowPathV2 followPath = null;
    protected System.Action onMoveComplete = null;
    private float epsillon = 0.71f;

    public bool AIOn { get; set; }
    public bool LookOn { get; set; }
    public bool LookAtTarget { get; set; }
    public float smoothRatio { get; set; }

    public void Initialize(EASteeringBehaviour steering)
    {
        this.steering = steering;
        AIOn = true;
        LookOn = false;
        LookAtTarget = false;
        smoothRatio = 0.25f;
        epsillon = 0.71f;
        arrive = steering.Get<Arrive>(Steering.behaviour_type.arrive);
        followPath = steering.Get<FollowPathV2>(Steering.behaviour_type.follow_path_v2);
    }

    public void AIUpdate()
    {
        if (AIOn == false) return;

        //keep a record of its old position so we can update its cell later
        //in this method
        Vector3 oldVelocity = steering.agent.GetVelocity();

        if (LookAtTarget)
        {
            Vector3 vTarget = Vector3.zero;
            if (steering.IsOn(Steering.behaviour_type.arrive)) vTarget = arrive.GetVTarget();
            if (steering.IsOn(Steering.behaviour_type.follow_path_v2)) vTarget = followPath.GetCurVTarget();
            LookAtDirection(vTarget - steering.agent.GetPos(), smoothRatio);
        }
        else if (LookOn) steering.LookWhereYourGoing();

        //moveState
        if (steering.IsOn(Steering.behaviour_type.arrive))
        {
            Vector3 endPos = arrive.GetVTarget();
            bool isReached = Vector3.Distance(steering.agent.GetPos(), endPos) < epsillon;
            if (isReached)
            {
                steering.ArriveOff();
                if (onMoveComplete != null) onMoveComplete();
                return;
            }
        }
        if(steering.IsOn(Steering.behaviour_type.follow_path_v2))
        {
            if(followPath.IsAtEndOfPath())
            {
                steering.Follow2Off();
                if (onMoveComplete != null) onMoveComplete();
                return;
            }
        }
    }

    public void SetVTarget(Vector3 vTarget)
    {
        if (arrive == null) return;
        arrive.SetVTarget(vTarget);
    }

    protected void LookAtDirection(Vector3 direction,float smoothRatio = 1f)
    {
        if (!steering.isCanFly) direction.y = 0;
        steering.agent.SetRotation(Quaternion.LookRotation(direction.normalized, Vector3.up), smoothRatio);
    } 

    public void MoveTo(Vector3 targetPosition, System.Action onMoveComplete = null)
    {
        steering.ArriveOn();
        arrive.SetVTarget(targetPosition);
        this.onMoveComplete = onMoveComplete;
    }

    public void SetSpeed(float speed,float epsillon = 0.71f)
    {
        steering.SetMaxSpeed(speed);
        this.epsillon = epsillon;
    }

    public void MoveToPath(EAAIPath aiPath, bool isLoop = false , System.Action onMoveComplete = null)
    {
        followPath.SetAIPath(aiPath);
        followPath.SetWaypointSeekDist(epsillon);

        if (isLoop) followPath.LoopOn();
        if (!isLoop) followPath.LoopOff();

        steering.Follow2On();
        steering.ResetInfo();

        this.onMoveComplete = onMoveComplete;
    }
}