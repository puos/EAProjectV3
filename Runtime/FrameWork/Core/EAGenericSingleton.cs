using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EAGenericSingleton<T> where T : new()
{
    static protected T _instance;
    static public T instance 
    {
        get 
        {
            if (_instance == null) _instance = new T();
            return _instance;
        }
    }
    // Prevent generation from outside
    protected EAGenericSingleton()
    {
    }
}
