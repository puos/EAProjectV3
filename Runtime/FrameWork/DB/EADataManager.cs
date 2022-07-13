using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EADataInfo
{
}

public class EADataTable : ScriptableObject
{
    public virtual void Load()
    {
    }
    public virtual IEnumerator LoadAsync() 
    {
        yield return null;
    }
    public virtual EADataInfo FindByKey(string key)
    {
        return default(EADataInfo);
    }
    public virtual EADataInfo FindByKey(int key)
    {
        return default(EADataInfo);
    } 
    public virtual EADataInfo FindByKey(float key)
    {
        return default(EADataInfo);
    }
    public virtual EADataInfo[] GetArrayData() 
    {
        return null;
    }
    public virtual void ParseCSV(Type classtype,string path)
    {
    }
    public Array ParseCSVToArray(Type classtype,string path)
    {
        return default(Array);
    }
}

public interface IEADataManager
{
    string TranslateKeyArgs(LANGUAGE_TYPE langType, UI_TEXT_TYPE uiType, string uiID, params object[] parms);
}

public class EADataManager<classT> : EAGenericSingleton<classT> , IEADataManager where classT : new()
{
    private Dictionary<int, EADataTable> dicDataTables = new Dictionary<int, EADataTable>();

    protected override void SingletonToInit()
    {
       if(EAMainFrame.iDataManager == null) EAMainFrame.iDataManager = this;
    }

    public void InitializeTableData(Dictionary<string,string> DataInfoList)
    {
        var it = DataInfoList.GetEnumerator();
        while(it.MoveNext())
        {
            int key = CRC32.GetHashForAnsi(it.Current.Key);
            if (!dicDataTables.TryGetValue(key, out EADataTable outDatas))
            {
                EADataTable so = GameResourceManager.instance.Load<EADataTable>(it.Current.Value);
                if (so == null) continue;
                so.Load();
                dicDataTables.Add(key, so);
            }
        }
    }
    public IEnumerator CoInitializeTableData(Dictionary<string, string> DataInfoList)
    {
        var it = DataInfoList.GetEnumerator();
        while (it.MoveNext())
        {
            EADataTable so = GameResourceManager.instance.Load<EADataTable>(it.Current.Value);
            if (so != null)
            {
                int key = CRC32.GetHashForAnsi(it.Current.Key);
                if (!dicDataTables.TryGetValue(key, out EADataTable outDatas))
                {
                    so.Load();
                    dicDataTables.Add(key, so);
                }
                yield return null;
            }
        }
        yield return null;
    }
    public IEnumerator InitializeTableDataAsync(Dictionary<string, string> DataInfoList)
    {
        var it = DataInfoList.GetEnumerator();
        while (it.MoveNext())
        {
            EADataTable so = null;
            bool bLoad = false;
            GameResourceManager.instance.LoadAsync<EADataTable>(it.Current.Value,(EADataTable table) => 
            {
                so = table;
                bLoad = true;
            });

            if (bLoad == false) yield return null;

            int key = CRC32.GetHashForAnsi(it.Current.Key);
            if (!dicDataTables.TryGetValue(key, out EADataTable outDatas))
            {
                yield return so.LoadAsync();
                dicDataTables.Add(key, so);
            }
        }
    }
    public EADataTable GetTable<T>() where T : EADataInfo
    {
        string tableName = typeof(T).Name;
        int key = CRC32.GetHashForAnsi(tableName);
        dicDataTables.TryGetValue(key, out EADataTable table);
        if (table != null) return table;
        Debug.LogError("not find data type " + typeof(T));
        return default(EADataTable);
    }
    public List<T> GetData<T>(Predicate<T> match) where T : EADataInfo
    {
        List<T> list = new List<T>();
        string tableName = typeof(T).Name;
        int key = CRC32.GetHashForAnsi(tableName);
        dicDataTables.TryGetValue(key, out EADataTable table);
        if (table == null) return list;
        T[] array = table.GetArrayData() as T[];
        for(int i = 0; i < array.Length;++i)
        {
            if (match(array[i])) list.Add(array[i]);
        }
        return list;
    }
    public T GetDataFirst<T>(Predicate<T> match) where T : EADataInfo
    {
        string tableName = typeof(T).Name;
        int key = CRC32.GetHashForAnsi(tableName);
        dicDataTables.TryGetValue(key, out EADataTable table);
        if (table == null) return default(T);
        T[] array = table.GetArrayData() as T[];
        for(int i = 0; i < array.Length; ++i)
        {
            if (match(array[i])) return array[i];
        }
        return default(T);
    }
    /// Get array data
    public T[] GetArrayData<T>() 
    {
        T[] array = null;
        string tableName = typeof(T).Name;
        int key = CRC32.GetHashForAnsi(tableName);
        dicDataTables.TryGetValue(key, out EADataTable table);
        if (table == null) return null;
        array = table.GetArrayData() as T[];
        return array;
    }
    /// Find the name of a data asset by type using the following rules
    /// const string tmplDataFile = "[TableName]Info";
    /// const string tmplDataHolderFile = "[TableName]DataHolder";
    public static string Type2AssetName(Type type)
    {
        const string TABLE_NAME_POSTFIX = "Info";
        const string ASSET_NAME_POSTFIX = "DataHolderAsset";
        string className = type.Name;
        return className.Replace(TABLE_NAME_POSTFIX, ASSET_NAME_POSTFIX);
    }

    public static string NotSetString = "<color=red><NotFound></color>";

    /// <summary>
    /// Translate with key
    /// </summary>
    /// <param name="langType"> Currently applied language type </param>
    /// <param name="uiType"> Currently applied ui type </param>
    /// <param name="uiID"></param>
    /// <param name="parms"></param>
    /// <returns></returns>
    public virtual string TranslateKeyArgs(LANGUAGE_TYPE langType,UI_TEXT_TYPE uiType,string uiID,params object[] parms)
    {
        return uiID;
    }
}