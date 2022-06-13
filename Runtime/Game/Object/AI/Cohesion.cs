using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Cohesion : Steering
{
    private float facingCosine = 180f;
    private float facingCosineVal;
    private NearSensor sensor;
    private TacticsType cohesionType = TacticsType.ALL;

    public override void Initialize(EASteeringBehaviour steering)
    {
        base.Initialize(steering);
        facingCosineVal = Mathf.Cos(facingCosine * Mathf.Deg2Rad);
        sensor = steering.nearSensor;
    }

    public void SetTactics(TacticsType t) 
    {
        cohesionType = t;
    }

    public void SetFacingCosine(float facingCosine)
    {
        this.facingCosine = facingCosine;
    }

    public override Vector3 GetSteering()
    {
        if(cohesionType == TacticsType.TACTICS_ALLY)
        {
            EAAIGroup group = steering.agent.GetAIGroup();
            return GetSteering(group.Agents());
        } 
        return GetSteering(sensor.targets);
    }
  
    public Vector3 GetSteering(ICollection<EAAIAgent> targets)
    {
        Vector3 centerOfMass = Vector3.zero;
        int count = 0;

        var it = targets.GetEnumerator();
        while (it.MoveNext())
        {
            if (Equals(it.Current, steering.agent)) continue;

            if (steering.IsFacing(it.Current.GetPos(), facingCosineVal))
            {
                centerOfMass += it.Current.GetPos();
                count++;
            }
        }

        if (count == 0) return Vector3.zero;

        centerOfMass = centerOfMass / count;
        return steering.Arrive(centerOfMass);
    }
}
