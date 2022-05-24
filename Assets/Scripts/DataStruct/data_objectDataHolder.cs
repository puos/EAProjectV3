using System.Collections;
using System.Collections.Generic;
using System;

public class data_objectDataHolder : EADataTable 
{
   Dictionary<string,data_objectInfo> dictionaryData = new Dictionary<string,data_objectInfo>();
   public data_objectInfo[] arrayData = null;

   public override EADataInfo FindByKey(string key)
   {
	   dictionaryData.TryGetValue(key,out data_objectInfo def);
	   return def;
   }

   public Dictionary<string,data_objectInfo> GetDatas()
   {
         return dictionaryData;
   }

   public override EADataInfo[] GetArrayData()
   {
         return arrayData;
   }
   
   public override void Load()
   {
        dictionaryData.Clear();
		
		for (int i = 0; i < arrayData.Length; ++i)
		{
		   if(!dictionaryData.TryGetValue(arrayData[i].obj_id , out data_objectInfo v))
		   {
			  dictionaryData.Add(arrayData[i].obj_id , arrayData[i]);
		   }
		}
    }
	
	public override IEnumerator LoadAsync()
	{
	    dictionaryData.Clear();
		
		for (int i = 0; i < arrayData.Length; ++i)
		{
		   if(!dictionaryData.TryGetValue(arrayData[i].obj_id , out data_objectInfo v))
		   {
			   dictionaryData.Add(arrayData[i].obj_id , arrayData[i]);
		   }
			
		   if(i % 1000 == 0) yield return null;
		}
		
		yield return null;
	}
	
	public override void ParseCSV(Type classtype, string path)
    {
        arrayData = (data_objectInfo[])ParseCSVToArray(classtype,path);
    }
}