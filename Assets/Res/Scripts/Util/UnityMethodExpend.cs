using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class UnityMethodExpend
{

    public static void Reset(this RectTransform rect)
    { 
        rect.localScale = Vector3.one;
        rect.localPosition = Vector3.zero;
        rect.localRotation = Quaternion.identity;
    }

    public static void SetBtnEvent(this Button button, UnityAction onclick)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onclick);
    }
    public static bool Contains(this string[] array,string key)
    {
        if (array == null || array.Length == 0) return false;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == key) return true;
        }
        return true;
    }

    public static void DictionaryAppend<TKey>(this Dictionary<TKey, int> dicA, Dictionary<TKey, int> dicB)
    {
        foreach (var newDataPair in dicB)
        {
            if (dicA.ContainsKey(newDataPair.Key))
            {
                dicA[newDataPair.Key] += newDataPair.Value;
            }
            else
            {
                dicA[newDataPair.Key] = newDataPair.Value;
            }
        }
    }

    public static TValue DictionaryFirst<TKey, TValue>(this Dictionary<TKey, TValue> dic)
    {
        foreach (var newDataPair in dic)
        {
            return newDataPair.Value;
        }
        return default;
    }
    public static List<TKey> GetKeysByValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TValue value)
    {
        List<TKey> keys = new List<TKey>();

        foreach (KeyValuePair<TKey, TValue> pair in dictionary)
        {
            if (EqualityComparer<TValue>.Default.Equals(pair.Value, value))
            {
                keys.Add(pair.Key);
            }
        }

        return keys;
    }
    public static void DictionaryRemove<TKey>(this Dictionary<TKey, int> dicA, Dictionary<TKey, int> dicB)
    {
        foreach (var newDataPair in dicB)
        {
            if (dicA.ContainsKey(newDataPair.Key))
            {
                dicA[newDataPair.Key] -= newDataPair.Value;
                if (dicA[newDataPair.Key] <= 0)
                {
                    dicA.Remove(newDataPair.Key);
                }
            }
        }
    }
    public static List<(TKey,TValue)> GetDictionaryPairList<TKey,TValue>(this Dictionary<TKey, TValue> dic)
    {
        List<(TKey, TValue)> result = new List<(TKey, TValue)>();
        foreach(var pair in dic)
        {
            result.Add((pair.Key, pair.Value));
        }
        result.Sort();
        return result;
    }

    public static Dictionary<TKey, TValue> Clone<TKey,TValue>(this Dictionary<TKey, TValue> dicA)
    {
        Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();
        foreach(var data in dicA)
        {
            result.Add(data.Key, data.Value);
        }
        return result;
    }

    public static TValue Pop<TKey,TValue>(this SortedList<TKey, TValue> sortedList)
    {
        TValue result = default;
        foreach(var data in sortedList)
        {
            result = data.Value;
            break;
        }
        sortedList.RemoveAt(sortedList.IndexOfValue(result));
        return result;
    }
}
