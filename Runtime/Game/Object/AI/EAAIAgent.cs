using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EAAIAgent : EAAIObject
{
    public uint objectId { get; set; }
    Vector3 VTarget();
    void SetVTarget(Vector3 vCrosshair);
    Vector3 GetVelocity();
    float GetMaxSpeed();
    void SetMaxSpeed(float newSpeed);
    float GetSpeed();
    Vector3 GetSide();
    EAAIGroup GetAIGroup();
    void SetAIGroup(EAAIGroup aiGroup);
    /**
    * @return time elapsed from last update
    */
    float GetTimeElapsed();

    EAGamePhysicWorld World();
}
