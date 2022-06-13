using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

[CanEditMultipleObjects]
[CustomEditor(typeof(EAPathMaker))]
public class EAPathMakerInspector : Editor
{
    EAPathMaker p;

    private void OnEnable()
    {
        p = target as EAPathMaker;
        p.Initialize();
    }

    public override void OnInspectorGUI()
    {
        int width = 70;

        GUILayout.BeginVertical(GUILayout.Width(width));

        p.key = EditorGUILayout.TextField("key : ", p.key);
        p.color = EditorGUILayout.ColorField("color : ", p.color);

        System.Type type = typeof(Transform);

        for(int i = 0; i < p.objects.Length; ++i)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(p.objects[i] as Object, type, true, GUILayout.Width(160));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

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
            Scene curScene = EditorSceneManager.GetActiveScene();

            if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(curScene);
        }
    }
}
