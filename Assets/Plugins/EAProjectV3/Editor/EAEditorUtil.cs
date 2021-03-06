using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Unity.EditorCoroutines.Editor;
using System.Collections;

[InitializeOnLoad]
public class EAEditorUtil
{
    class Events
    {
        public float execTime;
        public Action  cb;
    }


    [MenuItem("Tools/Save Assets %&#s")]
    public static void SaveAssets()
    {
        EditorUtility.DisplayProgressBar("Notice", "Assets are being saved to disk ", 0.5f);

        AssetDatabase.SaveAssets();

        EditorUtility.DisplayProgressBar("Notice", "Assets are being saved to disk ", 1f);

        Delay(0.5f, () =>
        {
            EditorUtility.ClearProgressBar();
        });
    }

    [MenuItem("Assets/ResourceCheckTool/FindAllAssetsDependsOn")]
    public static void FindAllAssetsUsingResource()
    {
        string jobTitle = "FindAllAssetsDependsOn";
        string[] folders = AssetDatabase.GetSubFolders("Assets");

        if (folders.Length < 1)
        {
            List<string> insert = new List<string>(folders);
            insert.Insert(0, "Assets");
            folders = insert.ToArray();
        }
        FindReferencingAssets(jobTitle, folders);
    }

    static void FindReferencingAssets(string jobTitle, string[] targetFolders)
    {
        StringBuilder resultString = new StringBuilder();

        UnityEngine.Object assetObj = Selection.objects[0];
        string target = AssetDatabase.GetAssetPath(assetObj.GetInstanceID());
        Debug.Log("Find assets that depend on a resource(" + target + ")..");
        string progressTitle = jobTitle + ":" + target;

        string[] guids = AssetDatabase.FindAssets(null, targetFolders);

        List<string> uniques = new List<string>();

        foreach (string item in guids)
        {
            if (!uniques.Contains(item)) uniques.Add(item);
        }

        guids = uniques.ToArray();

        string[] paths = new string[guids.Length];

        for (int i = 0; i < guids.Length; ++i) paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);

        int cntFound = 0;

        for (int i = 0; i < paths.Length; ++i)
        {
            string path = paths[i];

            if (!path.EndsWith("unity", System.StringComparison.OrdinalIgnoreCase)
                && !path.EndsWith("prefab", System.StringComparison.OrdinalIgnoreCase))
                continue;

            if (target == path)
                continue;

            string foundNotice = (cntFound == 0) ? string.Empty : string.Format("({0} find)", cntFound);
            EditorUtility.DisplayProgressBar(progressTitle, string.Format("{0}Search({1}/{2}): {3}",
                foundNotice, i, paths.Length, path), (float)i / paths.Length);

            bool justFound = false;
            string[] ss = AssetDatabase.GetDependencies(new string[] { path });
            foreach (var s in ss)
            {
                if (s == target)
                {
                    cntFound++;
                    justFound = true;
                    resultString.Append(path);
                    resultString.Append("\n");
                    break;
                }
            }
            if (justFound && cntFound == 1)
            {
                EditorUtility.ClearProgressBar();
                if (!ConfirmMsgBox("Found 1 asset. Do you want to continue searching?"))
                    break;
            }
        }

        string foundMsg = "total " + cntFound + " Found assets.";
        Debug.Log(foundMsg + "\n\n" + resultString);

        EditorUtility.ClearProgressBar();

        MsgBox(foundMsg + "\n\n" + resultString);
    }

    public static void MsgBox(string msg)
    {
        EditorUtility.DisplayDialog("Notice", msg, "OK");
    }

    public static bool ConfirmMsgBox(string msg)
    {
        return EditorUtility.DisplayDialog("Notice", msg, "OK", "Cancel");
    }

    static IEnumerator DelayFunc(Events e)
    {
        yield return new WaitForSeconds(e.execTime);

        e.cb();
    }

    public static void Delay(float timeout,Action cb)
    {
        Events e = new Events();
        e.execTime = timeout;
        e.cb = cb;

        object obj = new object();
        EditorCoroutineUtility.StartCoroutine(DelayFunc(e), obj);
    }

    public static void SetDirty(UnityEngine.Object target)
    {
        SetDirty(target, true);
    }

    public static void SetDirty(UnityEngine.Object target, bool setSceneDirty)
    {
        EditorUtility.SetDirty(target);
        Scene currScene = EditorSceneManager.GetActiveScene();
        if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(currScene);
    }

    public static bool GetBool(string name, bool defaultValue) { return EditorPrefs.GetBool(name, defaultValue); }
    public static void SetBool(string name, bool val) { EditorPrefs.SetBool(name, val); }

    public static bool minimalisticLook
    {
        get { return GetBool("Minimalistic", false); }
        set { SetBool("Minimalistic", value); }
    }

    static bool mEndHorizontal = false;

    public static string textArea = "TextArea";

    public static void BeginContents() { BeginContents(EAEditorUtil.minimalisticLook); }

    public static void BeginContents(bool minimalistic)
    {
        if (!minimalistic)
        {
            mEndHorizontal = true;
            GUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(textArea, GUILayout.MinHeight(10f));
        }
        else
        {
            mEndHorizontal = false;
            EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(10f));
            GUILayout.Space(10f);
        }
        GUILayout.BeginVertical();
        GUILayout.Space(2f);
    }

    // End drawing the content area.
    public static void EndContents()
    {
        GUILayout.Space(3f);
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        if (mEndHorizontal)
        {
            GUILayout.Space(3f);
            GUILayout.EndHorizontal();
        }
        GUILayout.Space(3f);
    }

    public static bool DrawHeader(string text, bool detailed) { return DrawHeader(text, text, detailed, !detailed); }

    public static bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
    {
        bool state = GetBool(key, true);
        if (!minimalistic) GUILayout.Space(3f);
        if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        GUILayout.BeginHorizontal();
        GUI.changed = false;

        if (minimalistic)
        {
            if (state) text = "\u25BC" + (char)0x200a + text;
            else text = "\u25BA" + (char)0x200a + text;

            GUILayout.BeginHorizontal();
            GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
            if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();
        }
        else
        {
            text = "<b><size=11>" + text + "</size></b>";
            if (state) text = "\u25BC " + text;
            else text = "\u25BA " + text;
            if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
        }

        if (GUI.changed) SetBool(key, state);

        if (!minimalistic) GUILayout.Space(2f);
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        if (!forceOn && !state) GUILayout.Space(3f);
        return state;
    }

    private static List<string> results = new List<string>();

    [MenuItem("Tools/Log Missing Prefabs And Components")]
    public static void Search()
    {
        results.Clear();
        GameObject[] gos = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject go in gos) Traverse(go.transform);
        Debug.Log("> Total Results: " + results.Count);
        foreach (string result in results) Debug.Log("> " + result);
    }
    private static void Traverse(Transform transform,string path = "")
    {
        string thisPath = path + "/" + transform.name;
        Component[] components = transform.GetComponents<Component>();
        for(int i = 0; i < components.Length; ++i)
        {
            if (components[i] == null) AppendComponentResult(thisPath, i);
        }
        for(int c = 0; c < transform.childCount; ++c)
        {
            Transform t = transform.GetChild(c);
            PrefabAssetType pt = PrefabUtility.GetPrefabAssetType(t.gameObject);
            if(pt == PrefabAssetType.MissingAsset)
            {
                AppendTransformResult(path + "/" + transform.name, t.name);
            }
            else
            {
                Traverse(t, thisPath);
            } 
        }
    }
    private static void AppendComponentResult(string childPath,int index)
    {
        results.Add("Missing Component " + index + " of" + childPath);
    }
    private static void AppendTransformResult(string childPath,string name)
    {
        results.Add("Missing Prefab for \"" + name + "\" of " + childPath);
    }
}
