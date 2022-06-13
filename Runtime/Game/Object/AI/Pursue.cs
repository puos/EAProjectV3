using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursue : Steering
{
    public float maxPrediction = 1f;
    private EAAIAgent tAgent = null;

    public void SetTAgent(EAAIAgent tAgent)
    {
        this.tAgent = tAgent;
    }

    private Vector3 FindExplicitTarget()
    {
        Vector3 displacement = tAgent.GetPos() - steering.agent.GetPos();
        float distance = displacement.magnitude;

        float speed = steering.agent.GetVelocity().magnitude;
        float prediction = distance / speed;
        if (speed <= distance / maxPrediction) prediction = maxPrediction;
        Vector3 explicitTarget = tAgent.GetPos() + tAgent.GetVelocity() * prediction;
        return explicitTarget;
    }

    public override Vector3 GetSteering()
    {
        if (tAgent == null) return Vector3.zero;
        Vector3 explicitTarget = FindExplicitTarget();
        return steering.Seek(explicitTarget);
    }

    public override void DrawGizmo()
    {
        if (tAgent == null) return;
        Vector3 explicitTarget = FindExplicitTarget();
        Vector3 acc = steering.Seek(explicitTarget);

        if (acc.magnitude > 0f)
        {
            DebugExtension.DrawLineArrow(steering.agent.GetPos(), steering.agent.GetPos() + acc.normalized, Color.green);
        }
    }
}
