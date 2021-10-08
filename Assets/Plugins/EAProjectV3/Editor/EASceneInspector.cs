using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EAScene))]
public class EASceneInspector : Editor
{
    EAScene t;
    
    static readonly GUIContent controllerClass = new GUIContent("controllerClassType");
    public override void OnInspectorGUI()
    {
        t = target as EAScene;

        SerializedProperty scriptProperty = base.serializedObject.FindProperty("script");

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(scriptProperty, controllerClass);

        if (t.script != null) t.controllerClassType = t.script.name;

        t.screenX = EditorGUILayout.FloatField("screenX ", t.screenX);
        t.screenY = EditorGUILayout.FloatField("screenY ", t.screenY);

        if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(t);

        base.serializedObject.ApplyModifiedProperties();
    }
}
