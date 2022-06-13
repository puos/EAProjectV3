using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PursueOffset : Steering
{
    public float maxPrediction = 1f;
    private EAAIAgent tAgent = null;
    private Vector3 offset = Vector3.zero;

    public void SetTAgent(EAAIAgent tAgent)
    {
        this.tAgent = tAgent;
    }

    public void SetOffset(Vector3 offset) 
    {
        this.offset = offset;
    }

    public override Vector3 GetSteering()
    {
        Vector3 worldOffsetPos = tAgent.GetPos() + tAgent.TransformDirection(offset);
        Vector3 displacement = worldOffsetPos - steering.agent.GetPos();
        float distance = displacement.magnitude;
        float speed = steering.agent.GetVelocity().magnitude;
        float prediction = distance / speed;
        if (speed <= (distance / maxPrediction)) prediction = maxPrediction;
        Vector3 targetPos = worldOffsetPos + tAgent.GetVelocity() * prediction;
        return steering.Arrive(targetPos);
    }
}