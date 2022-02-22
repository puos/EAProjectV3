using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITween_BezierPoint : MonoBehaviour {

    public enum PointType
    {
        None,
        Broken,
        Smooth
    }

    [SerializeField] Vector3 m_handles01 = new Vector3(-0.1f,0,0);
    [SerializeField] Vector3 m_handles02 = new Vector3(0.1f,0,0);
    [SerializeField] PointType m_type = PointType.Smooth;

    [HideInInspector] [SerializeField] UITween_BezierCurve m_curve;
    public UITween_BezierCurve curve
    {
        get
        {
            if (m_curve == null)
                m_curve = GetComponentInParent<UITween_BezierCurve>();
            return m_curve;
        }
    }
	public PointType type { get { return m_type; } set {if(m_type != value) { m_type = value; curve.IsChange(); } }}

    public Vector3 worldHandles01 
    { 
        get { return m_type == PointType.None? transform.position : transform.TransformPoint(m_handles01); } 
        set { m_handles01 = transform.InverseTransformPoint(value); curve.IsChange(); }
    }
    public Vector3 worldHandles02 
    { 
        get { return m_type == PointType.None ? transform.position : transform.TransformPoint(m_handles02); } 
        set { m_handles02 = transform.InverseTransformPoint(value); curve.IsChange(); }
    }

	private void OnValidate()
	{
        if (type == PointType.Smooth && -m_handles01 != m_handles02)
            m_handles02 = -m_handles01;
	}
}
