using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UITween_BezierPoint))]
public class Jun_BezierPointEditor : Editor 
{
	public override void OnInspectorGUI()
	{
        base.OnInspectorGUI();
	}

	private void OnSceneGUI()
	{
        UITween_BezierPoint tar = (UITween_BezierPoint)target;
        UITween_BezierCurveEditor.DrawCurve(tar.curve);
        if (tar.transform.parent != tar.curve)
            tar.transform.parent = tar.curve.transform;
    }
}
