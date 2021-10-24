using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Wall
{
    protected Vector3 m_vA;
    protected Vector3 m_vB;
    protected Vector3 m_vN;

    protected void CalculateNormal()
    {
        Vector3 normal = (m_vB - m_vA).normalized;
        m_vN = Vector3.Cross(Vector3.up, normal);
    }

    public Wall()
    {
    }

    public Wall(Vector3 A,Vector3 B)
    {
        m_vA = A;
        m_vB = B;
        CalculateNormal();
    }
    public Wall(Vector3 A,Vector3 B,Vector3 N)
    {
        m_vA = A;
        m_vB = B;
        m_vN = N;
    }
    public void OnDebugRenderer()
    {
        Render(false);
    }

    public void Render(bool renderNormals)
    {
        Gizmos.DrawLine(m_vA, m_vB);

        //render the normals if rqd
        if(renderNormals)
        {
            Vector3 v = Center();
            DebugExtension.DrawLineArrow(v, v + m_vN * 0.5f);
        }
    }

    public Vector3 From() => m_vA;
    public Vector3 To() => m_vB;
    public Vector3 Normal() => m_vN;
    public Vector3 Center() => (m_vA + m_vB) * 0.5f;
}
