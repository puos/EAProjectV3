﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UITween_BezierCurve))]
public class UITween_BezierCurveEditor : Editor 
{

	public override void OnInspectorGUI()
	{
        UITween_BezierCurve tar = (UITween_BezierCurve)target;
        GUILayout.BeginVertical();

		tar.isAuto = EditorGUILayout.Toggle("Auto", tar.isAuto);
        tar.isClose = EditorGUILayout.Toggle("Close:",tar.isClose);
        if(GUILayout.Button("AddPoint",EditorStyles.miniButton))
        {
            tar.AddPoint();
        }

        for (int i = 0; i < tar.pointCount; i++)
        {
            UITween_BezierPoint thisPoint = tar.GetPoint(i);
            if(thisPoint == null)
            {
                break;
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("Point:" + i, GUILayout.Width(40));
            thisPoint.transform.position = EditorGUILayout.Vector3Field("",thisPoint.transform.position);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

	private void OnSceneGUI()
	{
        UITween_BezierCurve tar = (UITween_BezierCurve)target;
        DrawCurve(tar);
	}

    public static void DrawCurve (UITween_BezierCurve tar)
    {
        for (int i = 0; i < tar.pointCount; i++)
        {
            UITween_BezierPoint thisPoint = tar.GetPoint(i);
            if (thisPoint == null)
            {
                tar.RemovePointAt(i);
                break;
            }

			float handleSize = HandleUtility.GetHandleSize(thisPoint.transform.position) * 0.1f;

            var fmh_56_85_638553095609947776 = Quaternion.identity; Vector3 pointPos = Handles.FreeMoveHandle(thisPoint.transform.position, handleSize, Vector3.zero, Handles.SphereHandleCap);
            if (thisPoint.transform.position != pointPos)
            {
                thisPoint.transform.position = pointPos;
                tar.IsChange();
            }

			if(!tar.isAuto)
			{            
				if (thisPoint.type != UITween_BezierPoint.PointType.None)
				{
					var fmh_67_77_638553095609988621 = Quaternion.identity; Vector3 handle01Pos = Handles.FreeMoveHandle(thisPoint.worldHandles01, handleSize, Vector3.zero, Handles.CubeHandleCap);

					if (thisPoint.worldHandles01 != handle01Pos)
					{
						thisPoint.worldHandles01 = handle01Pos;
						if (thisPoint.type == UITween_BezierPoint.PointType.Smooth) thisPoint.worldHandles02 = -(handle01Pos - pointPos) + pointPos;
					}

					var fmh_75_77_638553095609992550 = Quaternion.identity; Vector3 handle02Pos = Handles.FreeMoveHandle(thisPoint.worldHandles02, handleSize, Vector3.zero, Handles.CubeHandleCap);
					if (thisPoint.worldHandles02 != handle02Pos)
					{
						thisPoint.worldHandles02 = handle02Pos;
						if (thisPoint.type == UITween_BezierPoint.PointType.Smooth) thisPoint.worldHandles01 = -(handle02Pos - pointPos) + pointPos;
					}

					Handles.DrawLine(pointPos, handle01Pos);
					Handles.DrawLine(pointPos, handle02Pos);
				}
			}
			else
			{
				UITween_BezierPoint currentPoint = tar.GetPoint(i);
				UITween_BezierPoint prePoint = tar.GetPrePoint(i);
				UITween_BezierPoint nextPoint = tar.GetNextPoint(i);

				if(prePoint == null || nextPoint == null || currentPoint == null)
				{
					currentPoint.type = UITween_BezierPoint.PointType.None;
				}
				else
				{
					//Vector3 pos01 = prePoint.transform.position - currentPoint.transform.position;
					//Vector3 pos02 = nextPoint.transform.position - currentPoint.transform.position;

					//Vector3 nPos = Vector3.Cross(pos01, pos02);

					//float angle = (180 - Vector3.Angle(pos01, pos02))*0.5f * Mathf.Deg2Rad;
					//Vector3 pos;
					//Vector3 a = prePoint.transform.position - pos;
					//Vector3 b = pos - currentPoint.transform.position;

					//Vector3 curPPos = currentPoint.transform.position;

					//(pos.x - curPPos.x)*(pos.x - curPPos.x)

					//Mathf.Cos(angle) * pos01.magnitude;
				}
			}
        }
    }
}
