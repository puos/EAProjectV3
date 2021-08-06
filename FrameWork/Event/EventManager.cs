using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBase
{
    public int nErrCode = 0;
}

public class EventManager : Singleton<EventManager>
{
    public  delegate void EventDelegate<T>(T cEvent) where T : EventBase;
    private delegate void EventDelegate(EventBase cEvent);

    private Dictionary<Type, EventDelegate> m_dicDelegate = new Dictionary<Type, EventDelegate>();
    private Dictionary<Delegate, EventDelegate> m_dicDelegateLookup = new Dictionary<Delegate, EventDelegate>();

    public void Clear() 
    {
        m_dicDelegate.Clear();
        m_dicDelegateLookup.Clear();
    }

    public void AddListener<T>(EventDelegate<T> Callback) where T : EventBase
    {
        if (m_dicDelegateLookup.ContainsKey(Callback))
            return;

        EventDelegate internalDelegate = (e) => Callback((T)e);
        m_dicDelegateLookup[Callback] = internalDelegate;

        Type type = typeof(T);
        if(!m_dicDelegate.TryGetValue(type,out EventDelegate tempDelegate))
        {
            m_dicDelegate[type] = tempDelegate += internalDelegate;
        }
        else
        {
            m_dicDelegate[type] = internalDelegate;
        }
    }

    public void DelListener<T>(EventDelegate<T> Callback) where T : EventBase
    {
        if(m_dicDelegateLookup.TryGetValue(Callback, out EventDelegate internalDelegate))
        {
            Type type = typeof(T);
            if(m_dicDelegate.TryGetValue(type, out EventDelegate tempDelegate))
            {
                tempDelegate -= internalDelegate;
                if(tempDelegate == null)
                {
                    m_dicDelegate.Remove(type);
                }
                else 
                {
                    m_dicDelegate[type] = tempDelegate;
                }
            }
        }
    }

    public void ExcuteEvent(EventBase cEvent)
    {
        if(m_dicDelegate.TryGetValue(cEvent.GetType() , out EventDelegate Callback))
        {
            Callback.Invoke(cEvent);
        }
    }
}
