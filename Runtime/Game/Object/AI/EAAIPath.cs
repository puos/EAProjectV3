using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EAAIPath
{
    private List<Vector3> m_wayPoints = new List<Vector3>();
    int currentIdx = 0;
    bool m_bLooped;

    public EAAIPath()
    {
        m_bLooped = false;
    }

    public EAAIPath(List<Vector3> path,bool looped)
    {
        Set(path);
        m_bLooped = looped;
    }

    public void LoopOn() => m_bLooped = true;
    public void LoopOff() => m_bLooped = false;

    public void Set(List<Vector3> new_path)
    {
        m_wayPoints.Clear();
        for (int i = 0; i < new_path.Count; ++i) m_wayPoints.Add(new_path[i]);
        currentIdx = 0;
    }

    public List<Vector3> GetPath() 
    {
        return m_wayPoints;
    }
   
    public Vector3 CurrentWayPoint()
    {
        if (m_wayPoints.Count <= 0) return Vector3.zero;
        if (m_wayPoints.Count <= currentIdx) return m_wayPoints[m_wayPoints.Count - 1];

        return m_wayPoints[currentIdx];
    }

    public bool Finished()
    {
        if ((currentIdx >= (m_wayPoints.Count - 1) && !m_bLooped)) return true;

        return false;
    }

    // Change the next waypoint in the path.
    public void SetNextWayPoint() 
    {
        Debug.Assert(m_wayPoints.Count > 0);

        int limit = (m_wayPoints.Count == 0) ? 1 : m_wayPoints.Count;
        if (m_bLooped) currentIdx = (currentIdx + 1) % limit;
        if (!m_bLooped) currentIdx = Math.Min(currentIdx + 1, limit - 1);
    }
    //creates a random path which is bound by rectangle described by
    //the min/max values
    public List<Vector3> CreateRandomPath(int numWayPoints,float minX,float minY,float maxX,float maxY,float fHeight)
    {
        m_wayPoints.Clear();
        float midX = (maxX + minX) * 0.5f;
        float midY = (maxY + minY) * 0.5f;
        float smaller = Math.Min(midX, midY);
        float spacing = EAMathUtil.TWO_PI / (float)numWayPoints;
        for(int i = 0 ; i < numWayPoints; ++i)
        {
            float radialDist = EAMathUtil.RandInRange(smaller * 0.2f, smaller);
            Vector2 temp = new Vector2(radialDist, 0f);
            temp = Vec2RotateAroundOrigin(temp, i * spacing);
            Vector3 temp2 = Vector3.zero;

            temp2.x = temp.x + midX;
            temp2.y = fHeight;
            temp2.z = temp.y + midY;

            m_wayPoints.Add(temp2);
        }
        currentIdx = 0;
        return m_wayPoints;
    }
    public Vector2 Vec2RotateAroundOrigin(Vector2 v, float ang)
    {
        Quaternion q = Quaternion.Euler(0, 0, ang);

        // now transform the object's vertices
        Vector2 t = q * v;
        return t;
    }

    // renders the path in orange
    public void OnDebugRender() 
    {
        if (m_wayPoints.Count <= 0) return;

        int idx = 0;

        Vector3 wp = m_wayPoints[idx];

        Gizmos.color = Color.red;

        while(idx < m_wayPoints.Count - 1)
        {
            idx += 1;
            Vector3 n = m_wayPoints[idx];
            DebugExtension.DrawLineArrow2(wp, n, Color.black);
            wp = n;
        }

        if (m_bLooped) DebugExtension.DrawLineArrow2(wp, m_wayPoints[0], Color.black);

        Gizmos.DrawSphere(wp, 0.1f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(m_wayPoints[currentIdx], 0.1f);
        Gizmos.color = Color.white;
    }
}
