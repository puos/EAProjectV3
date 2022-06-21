using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class EAActor : EAAIAgent
{
    protected EASteeringBehaviour steering = null;

    private EAAIGroup m_aiGroup = new EAAIGroup();
    public uint objectId { get; set; }
    public bool Tag { get; set; }

    public virtual void InitializeAI()
    {
        if (steering == null) steering = GetComponent<EASteeringBehaviour>();
        if (steering == null) steering = AddComponent<EASteeringBehaviour>();
        if (steering != null) steering.Initialize();
    }
    public virtual void UpdateAI()
    {
        if (steering == null) return;
        steering.Steer();
    }
    public virtual void ReleaseAI()
    {
        EAGameAIPhysicWorld.instance.RemoveAgent(this);
    }
    public Vector3 GetVelocity() => rb.velocity;
    public void SetVelocity(Vector3 v) => rb.velocity = v;
    public float GetSpeed() => rb.velocity.magnitude;
    public EAAIGroup GetAIGroup() => m_aiGroup;
    public void SetAIGroup(EAAIGroup aiGroup) => m_aiGroup = aiGroup;
    public void AddAgent(string teamId)
    {
        World().AddAgent(teamId, this);
    }
    public EAGameAIPhysicWorld World()
    {
        return EAGameAIPhysicWorld.instance;
    }
    public Vector3 GetSide() => tr.right;
    public void UnTagging() => Tag = false;
    public void Tagging() => Tag = true;
    public virtual void Stop()
    {
       rb.isKinematic = true;
       rb.velocity = Vector3.zero;
       rb.angularVelocity = Vector3.zero;
       rb.isKinematic = false;

       steering.ZeroFlag();
       steering.ResetInfo();
    }
}
