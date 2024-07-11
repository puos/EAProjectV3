using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EAScene))]
public class EASceneInspector : Editor
{
    EAScene t;

    public override void OnInspectorGUI()
    {
        t = target as EAScene;

        EditorGUI.BeginChangeCheck();

        t.screenX = EditorGUILayout.FloatField("screenX ", t.screenX);
        t.screenY = EditorGUILayout.FloatField("screenY ", t.screenY);

        if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(t);

        base.serializedObject.ApplyModifiedProperties();
    }

    
}
