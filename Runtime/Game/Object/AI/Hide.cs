using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Hide : Steering
{
    public float distanceFromBoundary = 0.6f;
    public float maxPrediction = 1f;
    private EAAIAgent tAgent = null;
    private TacticsType hideType = TacticsType.ALL;
    private NearSensor sensor;

    public override void Initialize(EASteeringBehaviour steering)
    {
        base.Initialize(steering);
        sensor = steering.nearSensor;
    }

    public void SetTAgent(EAAIAgent tAgent)
    {
        this.tAgent = tAgent;
    }

    public void SetTactics(TacticsType t)
    {
        hideType = t;
    }

    public override Vector3 GetSteering()
    {
        if(hideType == TacticsType.TACTICS_ALLY)
        {
            EAAIGroup group = steering.agent.GetAIGroup();
            return GetSteering(group.Agents());
        }  
        return GetSteering(sensor.targets);
    }

    public Vector3 GetSteering(ICollection<EAAIAgent> obstacles)
    {
        float distToCloset = Mathf.Infinity;
        Vector3 bestHidingSpot = Vector3.zero;

        var it = obstacles.GetEnumerator();
        while(it.MoveNext())
        {
            if (Equals(it.Current, steering.agent)) continue;
            if (Equals(it.Current, tAgent)) continue;

            Vector3 hidingSpot = GetHidingPosition(it.Current, tAgent);
            float dist = Vector3.Distance(hidingSpot, steering.tr.position);
            if(dist < distToCloset)
            {
                distToCloset   = dist;
                bestHidingSpot = hidingSpot;
            }
        }

        if (distToCloset == Mathf.Infinity) return Evade();
        return steering.Arrive(bestHidingSpot);
    }

    private Vector3 GetHidingPosition(EAAIAgent obstacle,EAAIAgent target)
    {
        float distAway = obstacle.GetBRadius() + distanceFromBoundary;
        Vector3 dir = obstacle.GetPos() - target.GetPos();
        dir.Normalize();
        return obstacle.GetPos() + dir * distAway;
    }

    private Vector3 FindExplicitTarget()
    {
        Vector3 displacement = tAgent.GetPos() - steering.agent.GetPos();
        float distance = displacement.magnitude;

        float speed = tAgent.GetVelocity().magnitude;
        float prediction;
        if (speed <= distance / maxPrediction) prediction = maxPrediction;
        else
        {
            prediction = distance / speed;
            prediction *= 0.9f;
        }
        Vector3 explicitTarget = tAgent.GetPos() + tAgent.GetVelocity() * prediction;
        return explicitTarget;
    }

    public Vector3 Evade()
    {
        if (tAgent == null) return Vector3.zero;
        Vector3 explicitTarget = FindExplicitTarget();
        return steering.Flee(explicitTarget);
    }
}