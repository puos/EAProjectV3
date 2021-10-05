using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using System.IO;

public class EAMakeDB : Editor
{
    const string dataAssetPath = "Assets/Resources/DataAssets";
    const string codeTargetPath = "Assets/Script/DataStruct/";
    const string tmplDataFile = "[TableName]Info";
    const string tmplDataHolderFile = "[TableName]DataHolder";
    const string tmplExt = ".cs";
    static string[] fieldNames = null;
    static string[] fieldTypes = null;
    static string[] comments = null;
    static string[] lines = null;
    static string codeTmplLoc = string.Empty;

    static int dataStartPos = 2;

    public static string GetRootPath()
    {
        if (!string.IsNullOrEmpty(codeTmplLoc)) return codeTmplLoc;

        var assets = AssetDatabase.FindAssets(tmplDataFile);
        foreach(var asset in assets)
        {
            var path = AssetDatabase.GUIDToAssetPath(asset);

            if(path.Contains("Editor/" + tmplDataFile))
            {
                codeTmplLoc = Path.GetDirectoryName(path);
                break;
            }
        }  
        return codeTmplLoc;
    }


    [MenuItem("Tools/MakeDB")]
    public static void MakeDB()
    {
        string path = EditorUtility.OpenFilePanel("Please select a CSV file. The file name becomes the data table","", "csv");

        if (string.IsNullOrEmpty(path)) return;

        path = path.Replace("\\", "/");
        string tableName = MakeLines(path);
        tableName = tableName.Split('.')[0];
        GenerateDataTableTemplate(tableName);
    }

    private static string MakeLines(string path)
    {
        dataStartPos = 2;
        lines = File.ReadAllLines(path, Encoding.Default);
        string[] url = path.Split('/');
        string filename = url[url.Length - 1];

        // Make sure the required directory exists
        CheckDirectory();

        lines[0] = lines[0].Replace("//", "");
        lines[1] = lines[1].Replace("//", "");
        fieldNames = lines[0].Split(',');
        fieldTypes = lines[1].Split(',');

        comments = new string[0];

        // If there is a comment, parse the comment
        if(lines[2].StartsWith("//"))
        {
            lines[2] = lines[2].Replace("//", "");
            comments = lines[2].Split(',');
            dataStartPos++;
        } 

        return filename;
    }

    private static void CheckDirectory()
    {
        if (!Directory.Exists(dataAssetPath)) Directory.CreateDirectory(dataAssetPath);
        if (!Directory.Exists(codeTargetPath)) Directory.CreateDirectory(codeTargetPath);
    }

    // Create a data type by replacing [@TableName] and [@Field] in the template file.
    private static void GenerateDataTableTemplate(string tableName)
    {
        string tmplFullPath = codeTargetPath + tmplDataFile + tmplExt;
        string publicMembers = string.Empty;

        tmplFullPath = tmplFullPath.Replace("[TableName]", tableName);
        string codeTemplate = File.ReadAllText(GetRootPath() + tmplDataFile);
        for(int i = 0; i < fieldNames.Length; ++i)
        {
            publicMembers += "\tpublic " + fieldTypes[i].ToLower() + "\t" + fieldNames[i] + ";";
            if(comments == null)
            {
                publicMembers += "\n";
                continue;
            }
            publicMembers += "\t//" + comments[i];
            publicMembers += "\n";
        }
        File.WriteAllText(tmplFullPath, codeTemplate);
    }

    private static void GenerateDataHolderTemplate(string tableName , string primaryKey , string keyType , string pathName)
    {
        // Copy the template file to the data type location.
        string targetPath = codeTargetPath + tmplDataHolderFile + ".cs";
        string codeTemplate = File.ReadAllText(GetRootPath() + tmplDataHolderFile);
        string targetClass = tmplDataHolderFile;

        targetPath = targetPath.Replace("[TableName]", tableName);
        targetClass = targetClass.Replace("[TableName]", tableName);

    }

    private static void CreateLock(string pathName,string className,string keyName)
    {
        using (StreamWriter sw = File.CreateText(".Lock.txt")) 
        {
            sw.WriteLine("pathName="+pathName);
            sw.WriteLine("className=" + className);
            sw.WriteLine("keyName=" + keyName);
            sw.Close();
        }
    }

    private static bool SecureLock(out string pathName,out string className,out string keyName)
    {
        pathName = string.Empty;
        className = string.Empty;
        keyName = string.Empty;

        if (!File.Exists(".Lock.txt")) return false;

        string[] lines = File.ReadAllLines(".Lock.txt");
        foreach(string s in lines)
        {
            string[] oneLine = s.Split('=');
            switch(oneLine[0])
            {
                case "pathName":
                    pathName = oneLine[1];
                    break;
                case "className":
                    className = oneLine[1];
                    break;
                case "keyName":
                    keyName = oneLine[1];
                    break;
                default:
                    Debug.LogWarning("There is an unknown identifier");
                    break;
            }
        }
        File.Delete(".Lock.txt");
        return true;
    }
}
