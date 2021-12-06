using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// Manage Option Values
public class OptionManager : Singleton<OptionManager>
{
    [System.Serializable]
    public class OptionValue
    {
        private string value;
        public string key { get; private set; }
        public Action<object> onChange;
        public OptionValue(string key,string value)
        {
            this.key = key;
            this.value = value;
        }
        public OptionValue(string key,int value)
        {
            this.key = key;
            this.value = value.ToString();
        }
        public void Set(string value)
        {
            bool notifyChange = this.value != value;
            this.value = value;
            if (notifyChange == false) return;
            if (onChange != null) onChange(value);
        }
        public void Set(int value)
        {
            Set(value.ToString());
        }
        public int intValue { get { int.TryParse(value, out int result); return result; } } 
        public string stringValue { get { return value; } }
    }

    Dictionary<int, OptionValue> optionValues = new Dictionary<int, OptionValue>();
    
    public override GameObject GetSingletonParent()
    {
        return EAMainFrame.instance.gameObject;
    }

    protected override void Initialize()
    {
        base.Initialize();

        EAMainFrame.instance.OnMainFrameFacilityCreated(MainFrameAddFlags.OptionMgr);
    }

    protected override void Close()
    {
        base.Close();

        SaveToDisc();
    }

    // player pref save
    public void SaveToDisc() 
    {
        var it = optionValues.GetEnumerator();
        while (it.MoveNext())
        {
            PlayerPrefs.SetString(it.Current.Value.key, it.Current.Value.stringValue);
        }

        PlayerPrefs.Save();

        Debug.Log("OptionManager - SaveToDisc");
    }
    public void SetOptionValue(string option,int value)
    {
        var optionConverted = CRC32.GetHashForAnsi(option);
        if(optionValues.TryGetValue(optionConverted,out OptionValue Value))
        {
            Value.Set(value);
        }
        else
        {
            optionValues.Add(optionConverted, new OptionValue(option, value));
        }
    }
    public void AddListener(string option,Action<int> listener)
    {
        var optionConverted = CRC32.GetHashForAnsi(option);
        if (optionValues.TryGetValue(optionConverted, out OptionValue Value))
        {
            Action<object> internalDelegate = (e) => listener((int)e);
            Value.onChange += internalDelegate;
        }
    }
    public void RemoveListener(string option,Action<int> listener)
    {
        var optionConverted = CRC32.GetHashForAnsi(option);
        if(optionValues.TryGetValue(optionConverted,out OptionValue Value))
        {
            Action<object> internalDelegate = (e) => listener((int)e);
            optionValues[optionConverted].onChange -= internalDelegate;
        }
    }
    public void AddListener(string option, Action<string> listener)
    {
        var optionConverted = CRC32.GetHashForAnsi(option);
        if (optionValues.TryGetValue(optionConverted, out OptionValue Value))
        {
            Action<object> internalDelegate = (e) => listener((string)e);
            Value.onChange += internalDelegate;
        }
    }
    public void RemoveListener(string option, Action<string> listener)
    {
        var optionConverted = CRC32.GetHashForAnsi(option);
        if (optionValues.TryGetValue(optionConverted, out OptionValue Value))
        {
            Action<object> internalDelegate = (e) => listener((string)e);
            optionValues[optionConverted].onChange -= internalDelegate;
        }
    }
    public int Get(string option)
    {
        int optionConverted = CRC32.GetHashForAnsi(option);
        if(!optionValues.TryGetValue(optionConverted,out OptionValue Value))
        {
            return int.MinValue;
        }
        return Value.intValue;
    }
    public string GetString(string option)
    {
        int optionConverted = CRC32.GetHashForAnsi(option);
        if (!optionValues.TryGetValue(optionConverted, out OptionValue Value))
        {
            return string.Empty;
        }
        return Value.stringValue;
    }
    public float GetValueInRatio(string option,float min,float max)
    {
        int originValue = Get(option);
        originValue = Mathf.Max(originValue, 0);
        float gap = Mathf.Abs(max - min);
        return (originValue - min) / gap;
    }
}
