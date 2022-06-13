using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class EAAIPath
{
    public Vector3[] nodes;

    [NonSerialized]
    public float maxDist;

    [NonSerialized]
    public float[] distances;

    public Vector3 this[int i]
    {
        get { return nodes[i]; }
        set { nodes[i] = value; }
    } 

    public int Length { get { return nodes.Length; } } 

    public Vector3 EndNode { get { return nodes[nodes.Length - 1]; } }

    private bool m_bLooped;
    private bool m_bReversePath;

    public void LoopOn() => m_bLooped = true;
    public void LoopOff() => m_bLooped = false;
    public bool IsLoop() => m_bLooped;

    public void ReverseOn() => m_bReversePath = true;
    public void ReverseOff() => m_bReversePath = false;
    public bool IsReverse() => m_bReversePath;

    public EAAIPath()
    {
        this.nodes = new Vector3[] { Vector3.zero };
        CalcDistances();
    }

    public EAAIPath(Vector3[] nodes)
    {
        this.nodes = nodes;
        CalcDistances();
    }

    public EAAIPath Clone()
    {
        return new EAAIPath((Vector3[])this.nodes.Clone());
    }

    public void CalcDistances()
    {
        distances = new float[nodes.Length];
        distances[0] = 0;

        for(var i = 0; i < nodes.Length - 1; ++i)
        {
            distances[i + 1] = distances[i] + Vector3.Distance(nodes[i], nodes[i + 1]);
        }
        maxDist = distances[distances.Length - 1];
    }

    public float GetParam(Vector3 position, EAAIAgent agent)
    {
        int closestSegment = GetClosetSegment(position);
        float param = this.distances[closestSegment] + GetParamForSegment(position, nodes[closestSegment], nodes[closestSegment + 1], agent);
        return param;
    }

    public int GetClosetSegment(Vector3 position)
    {
        float closestDist = DistToSegment(position, nodes[0], nodes[1]);
        int closetSegment = 0;
        for(int i = 1; i < nodes.Length - 1; ++i)
        {
            float dist = DistToSegment(position, nodes[i], nodes[i + 1]);
            if(dist <= closestDist)
            {
                closestDist   = dist;
                closetSegment =    i;
            }
        }
        return closetSegment;
    }

    float DistToSegment(Vector3 p,Vector3 v,Vector3 w)
    {
        Vector3 vw = w - v;
        float l2 = Vector3.Dot(vw, vw);
        if (l2 == 0) return Vector3.Distance(p, v);
        float t = Vector3.Distance(p - v, vw) / l2;
        if (t < 0) return Vector3.Distance(p, v);
        if (t > 1) return Vector3.Distance(p, w);
        Vector3 closetPoint = Vector3.Lerp(v, w, t);
        return Vector3.Distance(p, closetPoint);
    }

    float GetParamForSegment(Vector3 p,Vector3 v,Vector3 w,EAAIAgent agent)
    {
        Vector3 vw = w - v;
        float l2 = Vector3.Dot(vw, vw);
        if (l2 == 0) return 0;
        float t = Vector3.Dot(p - v, w) / l2;
        if (t < 0) t = 0;
        else if (t > 1) t = 1;
        return t * (v - w).magnitude;
    }

    public Vector3 GetPosition(float param)
    {
        if (param < 0) param = (m_bLooped) ? param + maxDist : 0;
        else if (param > maxDist) param = (m_bLooped) ? param - maxDist : maxDist;

        int i = 0;
        for(;i<distances.Length;++i)
        {
            if (distances[i] > param) break;
        }

        if (i > distances.Length - 2) i = distances.Length - 2;
        else i -= 1;

        float t = (param - distances[i]) / Vector3.Distance(nodes[i], nodes[i + 1]);
        return Vector3.Lerp(nodes[i], nodes[i + 1], t);
    }
    public void RemoveNode(int i)
    {
        Vector3[] newNodes = new Vector3[nodes.Length - 1];

        int newNodeIndex = 0;
        for(int j = 0; j < newNodes.Length; ++i)
        {
            if (j == i) continue;
            
            newNodes[newNodeIndex] = nodes[j];
            newNodeIndex++;
        }

        this.nodes = newNodes;
        CalcDistances();
    }
    public void ReversePath()
    {
        Array.Reverse(nodes);
        CalcDistances();
    }

    public void DrawGizmo(Color color)
    {
        for(int i = 0; i < nodes.Length - 1; ++i)
        {
            DebugExtension.DrawLineArrow2(nodes[i], nodes[i + 1], color);
        }

        if (IsLoop()) DebugExtension.DrawLineArrow2(nodes[nodes.Length - 1], nodes[0] , color);
    }
}
