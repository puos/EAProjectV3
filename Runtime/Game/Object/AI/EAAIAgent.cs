using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EAAIAgent : EAAIObject
{
    public uint objectId { get; set; }
    public Vector3 VTarget();
    public void SetVTarget(Vector3 vCrosshair);
    public Vector3 GetVelocity();
    public float GetMaxSpeed();
    public void SetMaxSpeed(float newSpeed);
    public float GetSpeed();
    public Vector3 GetSide();
    public EAAIGroup GetAIGroup();
    public void SetAIGroup(EAAIGroup aiGroup);
    /**
    * @return time elapsed from last update
    */
    public float GetTimeElapsed();

    public EAGamePhysicWorld World();

    public void AIUpdate(Vector3 velocity, float fElapsedTime);
}
