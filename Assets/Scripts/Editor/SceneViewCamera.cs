using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraFollow))]
public class SceneViewCamera : Editor
{
    private CameraFollow cam = null;

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        if (cam == null) cam = (CameraFollow)target;

        if(GUILayout.Button("SceneViewCamera"))
        {
            Camera svcam = SceneView.lastActiveSceneView.camera;
            cam.transform.position = svcam.transform.position;
            cam.transform.rotation = svcam.transform.rotation;
        }

        if(GUILayout.Button("MainCamera"))
        {
            SceneView sView = SceneView.lastActiveSceneView;
            sView.pivot = cam.transform.position;
            sView.rotation = cam.transform.rotation;
            sView.Repaint();
        }

        if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(cam);
    }

}
