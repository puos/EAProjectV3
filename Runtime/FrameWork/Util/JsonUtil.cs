using JsonFx.Json;
using System;
using System.Text;
using System.Collections.Generic;

public class JsonUtil
{
    public static string ObjToJson(object obj, bool prettyPrint = false)
    {
        if (prettyPrint == false) return JsonWriter.Serialize(obj);

        JsonWriterSettings jwsetting = new JsonWriterSettings();
        jwsetting.PrettyPrint = true;
        StringBuilder sb = new StringBuilder();
        JsonWriter writer = new JsonWriter(sb, jwsetting);
        writer.Write(obj);

        return sb.ToString();
    }
    public static T JsonToObj<T>(string json)
    {
        return JsonReader.Deserialize<T>(json);
    }
    public static Dictionary<string,object> JsonToDic(string json)
    {
        return JsonReader.Deserialize<Dictionary<string,object>>(json);
    }
    public static object JsonToObj(string json,System.Type type)
    {
        return JsonReader.Deserialize(json, type);
    }
}
