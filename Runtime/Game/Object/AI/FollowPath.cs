using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FollowPath : Steering
{
    private float stopRadius    = 0.005f;
    private float pathOffset    = 0.71f;
    private float pathDirection = 1f;
    private EAAIPath aiPath = new EAAIPath();

    public override void Initialize(EASteeringBehaviour steering)
    {
        fWeight = 0.5f;
        base.Initialize(steering);
    }

    public override Vector3 GetSteering()
    {
        if (aiPath.IsReverse() && IsAtEndOfPath(aiPath)) aiPath.ReversePath();
        Vector3 accel = GetVelocity(aiPath, out Vector3 targetPosition);
        return accel;
    }
    
    public void SetAIPath(EAAIPath aiPath)
    {
        this.aiPath = aiPath.Clone();
    }

    public void LoopOn() => this.aiPath.LoopOn();
    public void LoopOff() => this.aiPath.LoopOff();
    public bool IsLoop() => this.aiPath.IsLoop();

    public Vector3 GetVelocity(EAAIPath path,out Vector3 targetPosition)
    {
        if(path.Length == 1)
        {
            targetPosition = path[0];
            return steering.Arrive(targetPosition);
        }

        float param = path.GetParam(steering.tr.position, steering.agent);

        if(!path.IsLoop())
        {
            if(IsAtEndOfPath(path,param,out Vector3 finalDestination))
            {
                targetPosition = finalDestination;
                return Vector3.zero;
            }
        }

        param += pathDirection * pathOffset;
        targetPosition = path.GetPosition(param);

        return steering.Arrive(targetPosition);
    }
    
    public bool IsAtEndOfPath(EAAIPath path)
    {
        if(path.Length == 1)
        {
            Vector3 endPos = path[0];
            return Vector3.Distance(steering.tr.position, endPos) < stopRadius;
        }
        float param = path.GetParam(steering.tr.position, steering.agent);
        return IsAtEndOfPath(path, param, out Vector3 finalDestination);
    }

    bool IsAtEndOfPath(EAAIPath path,float param,out Vector3 finalDestination)
    {
        bool result;
        finalDestination = (pathDirection > 0) ? path[path.Length - 1] : path[0];
        if(param >= path.distances[path.Length -2])
        {
            result = Vector3.Distance(steering.tr.position, finalDestination) < stopRadius;
        }
        else
        {
            result = false;
        }
        return result;
    }

    public override void DrawGizmo()
    {
        aiPath.DrawGizmo(Color.black);

        int idx = aiPath.GetClosetSegment(steering.agent.GetPos());
        DebugExtension.DrawCircle(aiPath[idx], Color.black, 2f);
    }
}