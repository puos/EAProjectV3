using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class EAMeshTool : Editor
{
    private static void MakeCharacterInfo(GameObject partsObject)
    {
        EACharacterInfo skeleton = partsObject.GetComponent<EACharacterInfo>();

    }
}
