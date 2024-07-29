using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPoolManager : MonoSingleton_AutoCreate<GameObjectPoolManager>
{
    public Dictionary<string, Stack<GameObject>> pools = new Dictionary<string, Stack<GameObject>>();

    protected override void Init()
    {
        base.Init();
        gameObject.SetActive(false);
    }
    public void Recycle(GameObject target, string routeName)
    {
        if (!pools.ContainsKey(routeName))
        {
            pools[routeName] = new Stack<GameObject>();
        }
        if (pools[routeName].Contains(target)) return;
        pools[routeName].Push(target);
        target.transform.SetParent(transform);
    }

    public GameObject Spawn(string routeName)
    {
        if (pools.ContainsKey(routeName))
        {
            if (pools[routeName].Count > 0)
            {
                return pools[routeName].Pop();
            }
        }
        return Instantiate(DataBaseManager.Instance.ResourceLoad<GameObject>(routeName));
    }

    public GameObject Spawn(string routeName, Transform parent)
    {
        if (pools.ContainsKey(routeName))
        {
            if (pools[routeName].Count > 0)
            {
                var result = pools[routeName].Pop();
                result.transform.parent = parent;
                return result;
            }
        }
        return Instantiate(DataBaseManager.Instance.ResourceLoad<GameObject>(routeName), parent);
    }

    public GameObject Spawn(string routeName, Vector3 position, Transform parent = null)
    {
        if (pools.ContainsKey(routeName))
        {
            if (pools[routeName].Count > 0)
            {
                var result = pools[routeName].Pop();
                result.transform.parent = parent;
                result.transform.position = position;
                return result;
            }
        }
        return Instantiate(DataBaseManager.Instance.ResourceLoad<GameObject>(routeName), position, Quaternion.identity, parent);
    }
}
