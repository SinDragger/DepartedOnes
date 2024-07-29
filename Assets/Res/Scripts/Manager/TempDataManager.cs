using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempDataManager : Singleton<TempDataManager>
{
    Dictionary<string, object> tempDic = new Dictionary<string, object>();

    public void SetData<T>(string key,T value)
    {
        tempDic[key] = value;
    }

    public T GetData<T>(string key,T defaultValue=default)
    {
        if (tempDic.ContainsKey(key))
        {
            return (T)tempDic[key];
        }
        return defaultValue;
    }
}
