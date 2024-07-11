using System.Collections;
using System.Collections.Generic;
using System;

public class ActionSelectionDataHolder : EADataTable 
{
   Dictionary<int,ActionSelectionInfo> dictionaryData = new Dictionary<int,ActionSelectionInfo>();
   public ActionSelectionInfo[] arrayData = null;

   public override EADataInfo FindByKey(int key)
   {
	   dictionaryData.TryGetValue(key,out ActionSelectionInfo def);
	   return def;
   }

   public Dictionary<int,ActionSelectionInfo> GetDatas()
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
		   if(!dictionaryData.TryGetValue(arrayData[i].PrimaryKey , out ActionSelectionInfo v))
		   {
			  dictionaryData.Add(arrayData[i].PrimaryKey , arrayData[i]);
		   }
		}
    }
	
	public override IEnumerator LoadAsync()
	{
	    dictionaryData.Clear();
		
		for (int i = 0; i < arrayData.Length; ++i)
		{
		   if(!dictionaryData.TryGetValue(arrayData[i].PrimaryKey , out ActionSelectionInfo v))
		   {
			   dictionaryData.Add(arrayData[i].PrimaryKey , arrayData[i]);
		   }
			
		   if(i % 1000 == 0) yield return null;
		}
		
		yield return null;
	}
	
	public override void ParseCSV(Type classtype, string path)
    {
        arrayData = (ActionSelectionInfo[])ParseCSVToArray(classtype,path);
    }
}