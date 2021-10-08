using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

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
    static public void SaveAssets() 
    {
        EditorUtility.DisplayProgressBar("Notice", "Assets are being saved to disk ", 0.5f);

        AssetDatabase.SaveAssets();

        EditorUtility.DisplayProgressBar("Notice", "Assets are being saved to disk ", 1f);

        Delay(0.5f, () => 
        {
            EditorUtility.ClearProgressBar();
        });
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
