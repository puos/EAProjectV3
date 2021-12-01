using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IObject
{
    uint GetIdx();
    void SetIdx(uint idx);
}

public interface IObjectFactory<T> where T : IObject
{
    T CreateObject(uint idx);
}

public class GeneralPool<T> where T : IObject
{
    Dictionary<uint, T> m_list = new Dictionary<uint, T>();
    Queue<T> m_Queue = new Queue<T>();
    protected uint m_nIDMaxCount = 0;
    IObjectFactory<T> m_factory = null;

    public GeneralPool(IObjectFactory<T> factory,uint nIDMaxCount = 50)
    {
        m_factory = factory;
        m_nIDMaxCount = nIDMaxCount;
        ReGenerate();
    }

    public void ReGenerate()
    {
        m_Queue.Clear();
        for(uint i = 1; i <= m_nIDMaxCount; ++i)
        {
           m_Queue.Enqueue(m_factory.CreateObject(i));
        }
        m_list.Clear();
    }

    public T Create()
    {
        if(m_Queue.Count <= 0)
        {
            uint nextMaxCount = m_nIDMaxCount * 2;
            for(uint i = m_nIDMaxCount + 1; i <= nextMaxCount; ++i)
            {
                m_Queue.Enqueue(m_factory.CreateObject(i));
            }
            m_nIDMaxCount = nextMaxCount;
        }
        T obj = m_Queue.Dequeue();
        m_list.Add(obj.GetIdx(), obj);
        return obj;
    }

    public void Free(T obj)
    {
        if(m_list.TryGetValue(obj.GetIdx(),out T temp))
        {
            m_list.Remove(obj.GetIdx());
            m_Queue.Enqueue(obj);
        }
    }
}