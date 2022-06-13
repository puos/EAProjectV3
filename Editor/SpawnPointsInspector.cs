using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CanEditMultipleObjects]
[CustomEditor(typeof(SpawnPoint))]
public class SpawnPointsInspector : Editor
{
    SpawnPoint p;

    private void OnEnable()
    {
        p = target as SpawnPoint;
        p.Initialize();
    }

    public override void OnInspectorGUI()
    {

        int width = 70;

        GUILayout.BeginVertical(GUILayout.Width(width));

        p.randomRadius = EditorGUILayout.FloatField("random radius : ", p.randomRadius);

        System.Type type = typeof(Transform);

        var it = p.spawnGroup.GetEnumerator();
        
        while(it.MoveNext())
        {
            List<Transform> trs = it.Current.Value;
            EditorGUILayout.PrefixLabel($"{trs[0].name}");
            
            for(int i = 0; i < trs.Count; ++i)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(trs[i] as Object, type, true, GUILayout.Width(160));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
           
            EditorGUILayout.Space();
        }

        bool refresh = GUILayout.Button("Refresh", GUILayout.Width(60));

        if(refresh)
        {
            p.Generate();
            p.Initialize();
        }

        GUILayout.EndVertical();

        if(GUI.changed)
        {
            EditorUtility.SetDirty(p);

            Scene currScene = EditorSceneManager.GetActiveScene();

            if (!Application.isPlaying)
                EditorSceneManager.MarkSceneDirty(currScene);
        }
    }
}
