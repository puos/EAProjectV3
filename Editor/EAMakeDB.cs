using System;
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
                codeTmplLoc = codeTmplLoc.TrimEnd('/');
                codeTmplLoc += @"/";
                codeTmplLoc = codeTmplLoc.Replace("\\", "/");
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
        GenerateDataHolderTemplate(tableName, fieldNames[0], fieldTypes[0]);

        EAEditorUtil.Delay(.5f, () => 
        {
            string targetClass = tmplDataHolderFile;
            targetClass = targetClass.Replace("[TableName]", tableName);
            Type T = Type.GetType(targetClass + ",Assembly-CSharp");

            if (T == null) return false;

            MethodInfo method = typeof(EAMakeDB).GetMethod("CreateAsset", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo generic = method.MakeGenericMethod(T);
            generic.Invoke(null, new object[] { tableName, fieldNames[0] });
            EditorUtility.DisplayDialog("dataTable creation complete", "Conversion complete", "OK");
            return true;
        });
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
            fieldNames[i] = fieldNames[i].Replace("(", "");
            fieldNames[i] = fieldNames[i].Replace(")", "");
            fieldNames[i] = fieldNames[i].Replace(" ", "_");

            publicMembers += "\tpublic " + fieldTypes[i].ToLower() + "\t" + fieldNames[i] + ";";
           
            if(comments.Length == 0)
            {
                publicMembers += "\n";
                continue;
            }
            if(string.IsNullOrEmpty(comments[i]))
            {
                publicMembers += "\n";
                continue;
            }

            publicMembers += "\t//" + comments[i];
            publicMembers += "\n";
        }
        
        codeTemplate = codeTemplate.Replace("[@TableName]", tableName);
        codeTemplate = codeTemplate.Replace("[@Field]", publicMembers);

        File.WriteAllText(tmplFullPath, codeTemplate);

        AssetDatabase.SaveAssets();
        AssetDatabase.ImportAsset(tmplFullPath, ImportAssetOptions.ForceUpdate);
    }
    private static void GenerateDataHolderTemplate(string tableName , string primaryKey , string keyType)
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

        AssetDatabase.SaveAssets();
        AssetDatabase.ImportAsset(targetPath, ImportAssetOptions.ForceUpdate);
    }
    private static void CreateAsset<T>(string tableName,string primaryKey) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();
        FieldInfo arrayData = asset.GetType().GetField("arrayData");
        Type dataType = arrayData.FieldType;

        dataType = dataType.GetElementType();
        Array dataElements = Array.CreateInstance(dataType, lines.Length - dataStartPos);

        MethodInfo method = typeof(EAMakeDB).GetMethod("GetRecord", BindingFlags.Static | BindingFlags.NonPublic);
        MethodInfo generic = method.MakeGenericMethod(dataType);

        for(int i = dataStartPos; i < lines.Length; ++i)
        {
            object result = (object)generic.Invoke(null, new object[] { fieldNames , lines[i].Split(',') });
            dataElements.SetValue(result, i - dataStartPos);
        }

        arrayData.SetValue(asset, dataElements);

        string assetPath = dataAssetPath + "/" + typeof(T).ToString() + "Asset.asset";
        // If the file exists before creating it, delete it and create a new one.
        if (File.Exists(assetPath)) File.Delete(assetPath);

        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.ImportAsset(assetPath,ImportAssetOptions.ForceUpdate);
    }
    private static T GetRecord<T>(string[] fieldName,string[] columns) where T : new()
    {
        T record = new T();

        string[] newColumns = new string[fieldName.Length];
        int curIdx = 0;
        for (int i = 0; i < columns.Length; ++i)
        {
            if (columns[i].StartsWith("\""))
            {
                newColumns[curIdx] = columns[i];
                do
                {
                    i = i + 1;
                    if (i >= columns.Length) break;
                    newColumns[curIdx] += " " + columns[i];
                }
                while (!columns[i].EndsWith("\""));
                newColumns[curIdx] = newColumns[curIdx].Replace("\"", "").Trim();
            }
            else
            {
                newColumns[curIdx] = columns[i];
            }
            curIdx++;
        }
        FieldInfo[] fields = record.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo f in fields)
        {
            for (int i = 0; i < fieldName.Length; ++i)
            {
                if (f.Name.Equals(fieldName[i]))
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(newColumns[i]))
                        {
                            f.SetValue(record, Convert.ChangeType(newColumns[i], f.FieldType));
                        }
                    }
                    catch
                    {
                        Debug.LogError("line:[" + i + "]" + f.Name + " : " + newColumns[i] + "->" + f.FieldType);
                    }
                }
            }
        }

        return record;
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
