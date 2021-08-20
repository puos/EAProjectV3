using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EADataAdaptor : MonoBehaviour
{
    [Serializable]
    public class EADataSlot
    {
        public string name;
        public GameObject dataObj;
    }

    [SerializeField] private EADataSlot[] datas;

    public GameObject GetData(string name)
    {
        for(int i = 0; i < datas.Length; ++i)
        {
            if (datas[i].name.Equals(name, StringComparison.Ordinal)) return datas[i].dataObj;
        }
        return null;
    }
}
