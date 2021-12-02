using System.Collections;
using System.Collections.Generic;
using System;

public class data_compareDataHolder : EADataTable 
{
   Dictionary<string,data_compareInfo> dictionaryData = new Dictionary<string,data_compareInfo>();
   public data_compareInfo[] arrayData = null;

   public override EADataInfo FindByKey(string key)
   {
	   dictionaryData.TryGetValue(key,out data_compareInfo def);
	   return def;
   }

   public Dictionary<string,data_compareInfo> GetDatas()
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
		   if(!dictionaryData.TryGetValue(arrayData[i].atk_type , out data_compareInfo v))
		   {
			  dictionaryData.Add(arrayData[i].atk_type , arrayData[i]);
		   }
		}
    }
	
	public override IEnumerator LoadAsync()
	{
	    dictionaryData.Clear();
		
		for (int i = 0; i < arrayData.Length; ++i)
		{
		   if(!dictionaryData.TryGetValue(arrayData[i].atk_type , out data_compareInfo v))
		   {
			   dictionaryData.Add(arrayData[i].atk_type , arrayData[i]);
		   }
			
		   if(i % 1000 == 0) yield return null;
		}
		
		yield return null;
	}
	
	public override void ParseCSV(Type classtype, string path)
    {
        arrayData = (data_compareInfo[])ParseCSVToArray(classtype,path);
    }
}