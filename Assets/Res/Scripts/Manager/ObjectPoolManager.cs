using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    public Dictionary<Type, Stack<object>> pools = new Dictionary<Type, Stack<object>>();

    Type point;
    public void Recycle<T>(object target)
    {
        if (target == null) return;
        point = typeof(T);
        if (!pools.ContainsKey(point))
        {
            pools.Add(point, new Stack<object>());
        }
        pools[point].Push(target);
    }
    public void Recycle<T>(T target)
    {
        if (target == null) return;
        point = typeof(T);
        if (!pools.ContainsKey(point))
        {
            pools.Add(point, new Stack<object>());
        }
        pools[point].Push(target);
    }

    public T Spawn<T>() where T : new()
    {
        if (pools.TryGetValue(typeof(T), out Stack<object> pool))
        {
            if (pool.Count > 0)
            {
                object result = pool.Pop();
                return (T)result;
            }
        }
        return new T();
    }
}
