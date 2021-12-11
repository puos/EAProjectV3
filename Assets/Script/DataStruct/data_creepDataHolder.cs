using System.Collections;
using System.Collections.Generic;
using System;

public class data_creepDataHolder : EADataTable 
{
   Dictionary<int,data_creepInfo> dictionaryData = new Dictionary<int,data_creepInfo>();
   public data_creepInfo[] arrayData = null;

   public override EADataInfo FindByKey(int key)
   {
	   dictionaryData.TryGetValue(key,out data_creepInfo def);
	   return def;
   }

   public Dictionary<int,data_creepInfo> GetDatas()
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
		   if(!dictionaryData.TryGetValue(arrayData[i].id , out data_creepInfo v))
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
		   if(!dictionaryData.TryGetValue(arrayData[i].id , out data_creepInfo v))
		   {
			   dictionaryData.Add(arrayData[i].id , arrayData[i]);
		   }
			
		   if(i % 1000 == 0) yield return null;
		}
		
		yield return null;
	}
	
	public override void ParseCSV(Type classtype, string path)
    {
        arrayData = (data_creepInfo[])ParseCSVToArray(classtype,path);
    }
}