using System.Collections;
using System.Collections.Generic;
using System;

public class data_gearItemDataHolder : EADataTable 
{
   Dictionary<string,data_gearItemInfo> dictionaryData = new Dictionary<string,data_gearItemInfo>();
   public data_gearItemInfo[] arrayData = null;

   public override EADataInfo FindByKey(string key)
   {
	   dictionaryData.TryGetValue(key,out data_gearItemInfo def);
	   return def;
   }

   public Dictionary<string,data_gearItemInfo> GetDatas()
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
		   if(!dictionaryData.TryGetValue(arrayData[i].id , out data_gearItemInfo v))
		   {
			  dictionaryData.Add(arrayData[i].id , arrayData[i]);
		   }
		}
    }
	
	public override IEnumerator LoadAsync()
	{
	    dictionaryData.Clear();
		
		for (int i = 0; i < arrayData.Length; ++i)
		{
		   if(!dictionaryData.TryGetValue(arrayData[i].id , out data_gearItemInfo v))
		   {
			   dictionaryData.Add(arrayData[i].id , arrayData[i]);
		   }
			
		   if(i % 1000 == 0) yield return null;
		}
		
		yield return null;
	}
	
	public override void ParseCSV(Type classtype, string path)
    {
        arrayData = (data_gearItemInfo[])ParseCSVToArray(classtype,path);
    }
}