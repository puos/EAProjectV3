using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class EAActor : EAAIAgent
{
    private float maxSpeed = 1f;
    private Vector3 m_vCrosshair;
    private EAAIGroup m_aiGroup = new EAAIGroup();
    public uint objectId { get; set; }
    public bool Tag { get; set; }
    public float GetMaxSpeed() => maxSpeed; 
    public void SetMaxSpeed(float newSpeed) => maxSpeed = newSpeed; 
    public Vector3 GetVelocity() => rb.velocity; 
    public float GetSpeed() => rb.velocity.magnitude; 
    public Vector3 VTarget() => m_vCrosshair;
    public void SetVTarget(Vector3 vCrosshair) => m_vCrosshair = vCrosshair; 
    public Vector3 GetSide() => tr.right;
    public EAAIGroup GetAIGroup() => m_aiGroup;
    public void SetAIGroup(EAAIGroup aiGroup) => m_aiGroup = aiGroup;
    public float GetTimeElapsed()
    {
        return Time.deltaTime;
    }

    public EAGamePhysicWorld World()
    {
        return EAGamePhysicWorld.instance;
    }

    public void UnTagging()
    {
        Tag = false;
    }

    public void Tagging()
    {
        Tag = true;
    }

    public Vector3 GetHeading()
    {
        return tr.forward;
    }

    public void SetHeading(Vector3 newHeading)
    {
        tr.forward = newHeading;
    }

    public void AIUpdate(Vector3 velocity, float fElapsedTime)
    {
        float gravitySpeed = velocity.y;

        //make sure vehicle does not exceed maximum velocity
        velocity = EAMathUtil.Truncate(velocity, maxSpeed);
        velocity.Set(velocity.x, gravitySpeed, velocity.z);
        rb.velocity = velocity;
    }

    public void Stop()
    {
       rb.isKinematic = true;
       rb.velocity = Vector3.zero;
       rb.angularVelocity = Vector3.zero;
       rb.isKinematic = false;
    }
}
