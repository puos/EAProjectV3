using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EADataAdaptor : MonoBehaviour
{
    public enum DataType { OBJ , INT , FLOAT , STRING };
    [Serializable]
    public class EADataSlot
    {
        public string name;
        public GameObject dataObj;
    }

    [Serializable]
    public class EAValueSlot
    {
        public string name;
        public string value;
    }

    [SerializeField] private EADataSlot[] datas;
    [SerializeField] private EAValueSlot[] values;

    public GameObject GetData(string name)
    {
        for(int i = 0; i < datas.Length; ++i)
        {
            if (datas[i].name.Equals(name, StringComparison.Ordinal)) return datas[i].dataObj;
        }
        Debug.Assert(false,$"DataAdaptor {name} is invalid");
        return null;
    }
    public float GetFloat(string name)
    {
        float value = 0f;

        for (int i = 0; i < values.Length; ++i)
        {
            if (values[i].name.Equals(name, StringComparison.Ordinal))
            {
                if (float.TryParse(values[i].value, out value)) return value;
            }
        }
        Debug.Assert(false, $"DataAdaptor {name} is invalid");
        return value;
    }
    public int GetInt(string name)
    {
        int value = 0;

        for (int i = 0; i < values.Length; ++i)
        {
            if (values[i].name.Equals(name, StringComparison.Ordinal))
            {
                if (int.TryParse(values[i].value, out value)) return value;
            }
        }
        Debug.Assert(false, $"DataAdaptor {name} is invalid");
        return value;
    }
    public string GetString(string name)
    {
        for (int i = 0; i < values.Length; ++i)
        {
            if (values[i].name.Equals(name, StringComparison.Ordinal))
            {
                return values[i].value;
            }
        }
        Debug.Assert(false, $"DataAdaptor {name} is invalid");
        return string.Empty;
    }

}
