using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Steering
{
    [Flags]
    public enum behaviour_type
    {
        none = 1 << 0,
        seek = 1 << 1,
        flee = 1 << 2,
        arrive = 1 << 3,
        wander1 = 1 << 4,
        wander2 = 1 << 5,
        cohesion = 1 << 6,
        seperation = 1 << 7,
        alignment = 1 << 8,
        agent_avoidance = 1 << 9,
        obstacle_avoidance = 1 << 10,
        follow_path = 1 << 11,
        pursuit = 1 << 12,
        pursuitOffset = 1 << 13,
        evade = 1 << 14,
        follow_path_v2 = 1 << 15,
        hide = 1 << 16,
    }

    protected EASteeringBehaviour steering = null;
    public behaviour_type bType { get; private set; }

    public enum TacticsType { ALL, TACTICS_ALLY }

    protected float fWeight = 1f;

    public virtual void Initialize(EASteeringBehaviour steering)
    {
        this.steering = steering;
    }

    public virtual Vector3 GetSteering() => Vector3.zero;

    public virtual void DrawGizmo()
    {
    }

    public Vector3 GetWSteering()
    {
        return GetSteering() * fWeight;
    }

    public void SetWeight(float fWeight) => this.fWeight = fWeight;

    public virtual void Reset() => this.fWeight = 1f;
    
    public static Steering Clone(behaviour_type clonetype)
    {
        switch (clonetype)
        {
            case behaviour_type.wander1: return new Wander1();
            case behaviour_type.wander2: return new Wander2();
            case behaviour_type.cohesion: return new Cohesion();
            case behaviour_type.seperation: return new Separation();
            case behaviour_type.arrive: return new Arrive();
            case behaviour_type.agent_avoidance: return new CollisionAvoidance();
            case behaviour_type.obstacle_avoidance: return new ObstacleAvoidance();
            case behaviour_type.flee: return new Flee();
            case behaviour_type.evade: return new Evade();
            case behaviour_type.follow_path: return new FollowPath();
            case behaviour_type.follow_path_v2: return new FollowPathV2();
            case behaviour_type.pursuit: return new Pursue();
            case behaviour_type.pursuitOffset: return new PursueOffset();
            case behaviour_type.hide: return new Hide();
        }
        return new Steering();
    }
}