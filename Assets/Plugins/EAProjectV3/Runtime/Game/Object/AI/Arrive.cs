using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : Steering
{
    private Vector3 vTarget = Vector3.zero;

    public void SetVTarget(Vector3 vTarget)
    {
        this.vTarget = vTarget;
    }

    public Vector3 GetVTarget() => this.vTarget;

    public override Vector3 GetSteering()
    {
       return steering.Arrive(vTarget);
    }
}
