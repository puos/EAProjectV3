using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class EAEditorUtil
{
    class Events
    {
        public float execTime;
        public Action cb;
    }
    
    static List<Events> eventsJob = new List<Events>();

    static EAEditorUtil()
    {
        EditorApplication.update += OnEditorUpdate;
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
        Debug.Log(foundMsg);

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

    static void OnEditorUpdate() 
    {
        if (eventsJob.Count == 0) return;

        for(int i = eventsJob.Count - 1; i >= 0; --i)
        {
            Events e = eventsJob[i];
            if(e.execTime >= EditorApplication.timeSinceStartup)
            {
                if(e.cb != null)e.cb();
                eventsJob.RemoveAt(i);
            } 
        }
    }

    public static void Delay(float timeout,Action cb)
    {
        Events e = new Events();
        e.execTime = (float)EditorApplication.timeSinceStartup + timeout;
        e.cb = cb;
        eventsJob.Add(e);
    }
}
