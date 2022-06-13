using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CollisionAvoidance : Steering
{
    private float maxAcceleration = 15f;
    private float distanceBetween = 0f;
    
    private NearSensor sensor;

    public override void Initialize(EASteeringBehaviour steering)
    {
        base.Initialize(steering);
        sensor = steering.nearSensor;
    }

    public void SetBeweenDist(float fValue)
    {
        this.distanceBetween = fValue;
    }

    public override void Reset()
    {
        base.Reset();
        this.distanceBetween = 0f;
    }

    public override Vector3 GetSteering()
    {
        Vector3 acceleration = Vector3.zero;

        float shortestTime = float.PositiveInfinity;
        EAAIObject firstTarget = null;
        float firstMinSeparation = 0, firstDistance = 0, firstRadius = 0;
        Vector3 firstRelativePos = Vector3.zero, firstRelativeVel = Vector3.zero;

        var it = sensor.targets.GetEnumerator();
        while(it.MoveNext())
        {
            EAAIAgent agent = (EAAIAgent)it.Current;
            Vector3 relativePos = steering.agent.GetColliderPos() - agent.GetColliderPos();
            Vector3 relativeVel = steering.agent.GetVelocity() - agent.GetVelocity();

            float distance = relativePos.magnitude;
            float relativeSpeed = relativeVel.magnitude;

            if (relativeSpeed == 0) continue;
            float timeToCollision = -1 * Vector3.Dot(relativePos, relativeVel) / (relativeSpeed * relativeSpeed);

            Vector3 separation = relativePos + relativeVel * timeToCollision;
            float minSeparation = separation.magnitude;

            if(timeToCollision > 0 && timeToCollision < shortestTime)
            {
                shortestTime = timeToCollision;
                firstTarget = agent;
                firstMinSeparation = minSeparation;
                firstDistance = distance;
                firstRelativePos = relativePos;
                firstRelativeVel = relativeVel;
                firstRadius = agent.GetBRadius();
            }
        }

        if (firstTarget == null) return acceleration;

        if(firstMinSeparation <= 0 || firstDistance < (steering.agent.GetBRadius() + firstRadius + distanceBetween))
        {
            acceleration = steering.agent.GetColliderPos() - firstTarget.GetColliderPos();
        }
        else
        {
            acceleration = firstRelativePos + firstRelativeVel * shortestTime;
        }

        if (!steering.isCanFly) acceleration.y = 0f;
        acceleration.Normalize();
        acceleration *= maxAcceleration;

        return acceleration;
    }

    public override void DrawGizmo()
    {
        Vector3 dir = GetSteering();
        dir.Normalize();
        DebugExtension.DrawArrow(steering.agent.GetColliderPos(), dir , Color.magenta);
    }
}