using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AIPathManager : EAGenericSingleton<AIPathManager>
{
    Dictionary<string, EAAIPath> aiPaths = new Dictionary<string, EAAIPath>();

    public void Clear()
    {
        aiPaths.Clear();
    }

    public void AddPath(string key,EAAIPath aiPath)
    {
        if(!aiPaths.TryGetValue(key,out EAAIPath value))
        {
            aiPaths.Add(key, aiPath);
            return;
        }
        aiPaths[key] = aiPath;
    }

    public void AddPath(string key, Vector3[] nodes)
    {
        EAAIPath aiPath = new EAAIPath(nodes);
        AddPath(key, aiPath);
    }

    public EAAIPath GetPath(string key)
    {
        aiPaths.TryGetValue(key, out EAAIPath aiPath);
        return aiPath;
    }
}