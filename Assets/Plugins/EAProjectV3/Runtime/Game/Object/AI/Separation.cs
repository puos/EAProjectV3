using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Separation : Steering
{
    private float sepMaxAcceleration = 25;
    private float maxSepDist = 1f;
       
    private NearSensor sensor;
    private TacticsType separationType = TacticsType.ALL;

    public override void Initialize(EASteeringBehaviour steering)
    {
        base.Initialize(steering);
        sensor = steering.nearSensor;
    }

    public void SetTactics(TacticsType t)
    {
        separationType = t;
    }

    public void SepMaxAccel(float sepMaxAcceleration)
    {
        this.sepMaxAcceleration = sepMaxAcceleration;
    }
    public void SepMaxDist(float maxSepDist)
    {
        this.maxSepDist = maxSepDist;
    }
   
    public override Vector3 GetSteering()
    {
        if(separationType == TacticsType.TACTICS_ALLY)
        {
            EAAIGroup group = steering.agent.GetAIGroup();
            return GetSteering(group.Agents());
        }
        return GetSteering(sensor.targets);
    }

    public Vector3 GetSteering(ICollection<EAAIAgent> targets)
    {
        Vector3 acceleration = Vector3.zero;
        var it = targets.GetEnumerator();
        while (it.MoveNext())
        {
            if (Equals(it.Current, steering.agent)) continue;

            Vector3 direction = steering.agent.GetPos() - it.Current.GetPos();
            float dist = direction.magnitude;
            if (dist < maxSepDist)
            {
                var strength = sepMaxAcceleration * (maxSepDist - dist) / (maxSepDist - it.Current.GetBRadius() - steering.agent.GetBRadius());
                if (!steering.isCanFly) direction.y = 0f;
                direction.Normalize();
                acceleration += direction * strength;
            }
        }
        return acceleration;
    }
}