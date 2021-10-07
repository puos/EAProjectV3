using System.Collections;
using System.Collections.Generic;
using System;

public class missionstringDataHolder : EADataTable 
{
   Dictionary<int,missionstringInfo> dictionaryData = new Dictionary<int,missionstringInfo>();
   public missionstringInfo[] arrayData = null;

   public override EADataInfo FindByKey(int key)
   {
	   dictionaryData.TryGetValue(key,out missionstringInfo def);
	   return def;
   }

   public Dictionary<int,missionstringInfo> GetDatas()
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
		   if(!dictionaryData.TryGetValue(arrayData[i].idKey , out missionstringInfo v))
		   {
			  dictionaryData.Add(arrayData[i].idKey , arrayData[i]);
		   }
		}
    }
	
	public override IEnumerator LoadAsync()
	{
	    dictionaryData.Clear();
		
		for (int i = 0; i < arrayData.Length; ++i)
		{
		   if(!dictionaryData.TryGetValue(arrayData[i].idKey , out missionstringInfo v))
		   {
			   dictionaryData.Add(arrayData[i].idKey , arrayData[i]);
		   }
			
		   if(i % 1000 == 0) yield return null;
		}
		
		yield return null;
	}
	
	public override void ParseCSV(Type classtype, string path)
    {
        arrayData = (missionstringInfo[])ParseCSVToArray(classtype,path);
    }
}