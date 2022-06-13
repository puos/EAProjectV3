using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EAAIAgent : EAAIObject
{
    public uint objectId { get; set; }
    public Vector3 GetVelocity();
    public void SetVelocity(Vector3 vel);
    public Vector3 GetOrientationToVector();
    public Vector3 TransformDirection(Vector3 v);
    public float GetSpeed();
    public EAAIGroup GetAIGroup();
    public void SetAIGroup(EAAIGroup aiGroup);
    public void AddAgent(string aiGroup);
    public EAGameAIPhysicWorld World();
    public void Stop();
}
