using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FollowPathV2 : Steering
{
    private float pathOffset = 0.71f;
    private int   currentIdx = 0;
    private EAAIPath aiPath = new EAAIPath();

    public override Vector3 GetSteering()
    {
        if (aiPath.IsReverse() && IsAtEndOfPath()) aiPath.ReversePath();

        Vector3 endPos = aiPath[currentIdx];
        if(Vector3.Distance(steering.tr.position, endPos) < pathOffset)
        {
            if (this.aiPath.IsLoop()) currentIdx = (currentIdx + 1) % aiPath.Length;
            if (!this.aiPath.IsLoop()) currentIdx = Mathf.Min(currentIdx + 1, aiPath.Length - 1);
        }
        endPos = aiPath[currentIdx];
        return steering.Arrive(endPos);
    }

    public void SetAIPath(EAAIPath aiPath)
    {
        this.aiPath = aiPath.Clone();
    }

    public void LoopOn() => this.aiPath.LoopOn();
    public void LoopOff() => this.aiPath.LoopOff();
    public bool IsLoop() => this.aiPath.IsLoop();

    public bool IsAtEndOfPath()
    {
        if (aiPath.Length == 1)
        {
            Vector3 endPos = aiPath[0];
            return Vector3.Distance(steering.tr.position, endPos) < pathOffset;
        }
        float param = aiPath.GetParam(steering.tr.position, steering.agent);
        return IsAtEndOfPath(aiPath, param, out Vector3 finalDestination);
    }
    private bool IsAtEndOfPath(EAAIPath path, float param, out Vector3 finalDestination)
    {
        bool result;
        finalDestination = path[path.Length - 1];
        if (param >= path.distances[path.Length - 2])
        {
            result = Vector3.Distance(steering.tr.position, finalDestination) < pathOffset;
        }
        else
        {
            result = false;
        }
        return result;
    }

    public void SetWaypointSeekDist(float pathOffset) => this.pathOffset = pathOffset;
    
    public Vector3 GetCurVTarget()
    {
        if (currentIdx < 0 || currentIdx >= aiPath.Length) return aiPath[aiPath.Length - 1];
        return aiPath[currentIdx];
    } 
    public override void DrawGizmo()
    {
        aiPath.DrawGizmo(Color.black);

        int idx = aiPath.GetClosetSegment(steering.agent.GetPos());
        DebugExtension.DrawCircle(aiPath[idx], Color.black, 2f);
    }
}
