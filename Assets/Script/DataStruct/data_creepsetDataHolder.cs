using System.Collections;
using System.Collections.Generic;
using System;

public class data_creepsetDataHolder : EADataTable 
{
   Dictionary<string,data_creepsetInfo> dictionaryData = new Dictionary<string,data_creepsetInfo>();
   public data_creepsetInfo[] arrayData = null;

   public override EADataInfo FindByKey(string key)
   {
	   dictionaryData.TryGetValue(key,out data_creepsetInfo def);
	   return def;
   }

   public Dictionary<string,data_creepsetInfo> GetDatas()
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
		   if(!dictionaryData.TryGetValue(arrayData[i].creep_set_id , out data_creepsetInfo v))
		   {
			  dictionaryData.Add(arrayData[i].creep_set_id , arrayData[i]);
		   }
		}
    }
	
	public override IEnumerator LoadAsync()
	{
	    dictionaryData.Clear();
		
		for (int i = 0; i < arrayData.Length; ++i)
		{
		   if(!dictionaryData.TryGetValue(arrayData[i].creep_set_id , out data_creepsetInfo v))
		   {
			   dictionaryData.Add(arrayData[i].creep_set_id , arrayData[i]);
		   }
			
		   if(i % 1000 == 0) yield return null;
		}
		
		yield return null;
	}
	
	public override void ParseCSV(Type classtype, string path)
    {
        arrayData = (data_creepsetInfo[])ParseCSVToArray(classtype,path);
    }
}