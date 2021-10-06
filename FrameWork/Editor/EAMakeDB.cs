﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;

public class EAMakeDB : Editor
{
    const string dataAssetPath = "Assets/Resources/DataAssets";
    const string codeTargetPath = "Assets/Script/DataStruct/";
    const string tmplDataFile = "[TableName]Info";
    const string tmplDataHolderFile = "[TableName]DataHolder";
    const string dataAssetLoc = "Assets/Developer/Datatables/";
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
        string path = EditorUtility.OpenFilePanel("Please select a CSV file. The file name becomes the data table","","csv");

        if (string.IsNullOrEmpty(path)) return;

        path = path.Replace("\\", "/");
        string tableName = MakeLines(path);
        tableName = tableName.Split('.')[0];
        GenerateDataTableTemplate(tableName);
        Type T = GenerateDataHolderTemplate(tableName, fieldNames[0], fieldTypes[0]);

        MethodInfo method = typeof(EAMakeDB).GetMethod("CreateAsset", BindingFlags.Static | BindingFlags.Public);
        MethodInfo generic = method.MakeGenericMethod(T);
        generic.Invoke(null, new object[] { tableName, fieldNames[0] });
        EditorUtility.DisplayDialog("dataTable creation complete", "Conversion complete", "OK");
    }

    [MenuItem("Assets/MakeCSV")]
    public static void MakeCSV() 
    {
        if(Selection.objects.Length == 0)
        {
            Debug.Log("Not Object");
            return;
        }
        for(int i = 0; i < Selection.objects.Length; ++i)
        {
            string path = AssetDatabase.GetAssetPath(Selection.objects[i]);
            ScriptableObject sObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

            if (sObject == null) continue;

            ExportAsCSV(sObject);
        }
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

    private static Type GenerateDataHolderTemplate(string tableName , string primaryKey , string keyType)
    {
        // Copy the template file to the data type location.
        string targetPath = codeTargetPath + tmplDataHolderFile + ".cs";
        string codeTemplate = File.ReadAllText(GetRootPath() + tmplDataHolderFile);
        string targetClass = tmplDataHolderFile;

        targetPath = targetPath.Replace("[TableName]", tableName);
        targetClass = targetClass.Replace("[TableName]", tableName);
        codeTemplate = codeTemplate.Replace("[TableName]", tableName);
        codeTemplate = codeTemplate.Replace("[KeyField]", primaryKey);
        codeTemplate = codeTemplate.Replace("[KeyType]", keyType);

        File.WriteAllText(targetPath, codeTemplate);

        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        AssetDatabase.SaveAssets();

        return Type.GetType(targetClass + ",Assembly-CSharp");
    }

    public static void CreateAsset<T>(string tableName,string primaryKey) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();
        FieldInfo arrayData = asset.GetType().GetField("arrayData");
        Type dataType = arrayData.FieldType;
        
        Array dataElements = Array.CreateInstance(dataType.GetElementType(), lines.Length - dataStartPos);


    }
    private static void ExportAsCSV(ScriptableObject sObject)
    {
        if (!Directory.Exists(dataAssetLoc)) Directory.CreateDirectory(dataAssetLoc);

        string str = sObject.GetType().ToString();
        str = str.Replace("DataHolder", "");
        FieldInfo arrayData = sObject.GetType().GetField("arrayData");
        Array dataElements = arrayData.GetValue(sObject) as Array;

        FieldInfo[] fields = dataElements.GetValue(0).GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

        string[] strArray  = new string[fields.Length];
        string[] strArray2 = new string[fields.Length];

        for(int i = 0; i < fields.Length; ++i)
        {
            strArray[i] = fields[i].Name;
            strArray2[i] = GetType(fields[i].FieldType.ToString());
        }

        StringBuilder builder = new StringBuilder();
        builder.AppendLine(string.Join(",", strArray));
        builder.AppendLine(string.Join(",", strArray2));

        for(int i = 0; i < dataElements.Length; ++i)
        {
            string[] strArray3 = new string[fields.Length];
            var data = dataElements.GetValue(i);
            for (int j = 0; j < fields.Length; ++j)
            { strArray3[j] = fields[j].GetValue(data).ToString(); }
            builder.AppendLine(string.Join(",", strArray3));
        }

        File.WriteAllText(dataAssetLoc + str + ".csv", builder.ToString());
        AssetDatabase.Refresh();
    }
    private static string GetType(string type)
    {
        string result = "string";
        switch(type)
        {
            case "System.String": result = "string"; break;
            case "System.Single": result = "float";  break;
            case "System.Int32":  result = "int"; break;
        }
        return result;
    }
}
