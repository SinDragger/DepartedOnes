using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 数据聚合实体:场景中交互的底层数据单元。散列存储自身数据、具备数据延展性
/// 不同势力存在不互通的资源类型。同比单位与建筑。（例如有复数生产序列、高速生产序列的建筑。乃至支持生产序列上传、改动、优化）
/// 效应器跟数据聚合实体进行通讯，请求数据
/// </summary>
public class AggregationEntity
{
    public string idName;//标识名称

    Dictionary<string, object> aggregationDataDic;
    /// <summary>
    /// 整型数据
    /// </summary>
    Dictionary<string, int> intDataDic;
    /// <summary>
    /// 浮点型数据
    /// </summary>
    Dictionary<string, float> floatDataDic;
    /// <summary>
    /// 字符串数据
    /// </summary>
    Dictionary<string, string> stringDataDic;

    public void SetObjectValue(string key, object value)
    {
        if (aggregationDataDic == null) aggregationDataDic = new Dictionary<string, object>();
        aggregationDataDic[key] = value;
    }

    public object GetObjectValue(string key)
    {
        if (aggregationDataDic == null) aggregationDataDic = new Dictionary<string, object>();
        if (aggregationDataDic.ContainsKey(key)) return aggregationDataDic[key];
        return null;
    }
    

    public T GetObjectValue<T>(string key,T defaultValue = default)
    {
        if (aggregationDataDic == null) aggregationDataDic = new Dictionary<string, object>();
        if (aggregationDataDic.ContainsKey(key)) return (T)aggregationDataDic[key];
        return defaultValue;
    }

    public bool ChangeIntValue(string key, int value)
    {
        if (intDataDic == null) intDataDic = new Dictionary<string, int>();
        if (intDataDic.ContainsKey(key))
        {
            intDataDic[key] += value;
            return true;
        }
        return false;
    }
    public void SetBoolValue(string key, bool value)
    {
        if (intDataDic == null) intDataDic = new Dictionary<string, int>();
        intDataDic[key] = value ? 1 : 0;
    }
    
    public bool GetBoolValue(string key, bool defaultValue = false)
    {
        if (intDataDic == null) intDataDic = new Dictionary<string, int>();
        if (intDataDic.ContainsKey(key))
        {
            return intDataDic[key] > 0;
        }
        return defaultValue;
    }

    public void SetIntValue(string key, int value)
    {
        if (intDataDic == null) intDataDic = new Dictionary<string, int>();
        intDataDic[key] = value;
    }

    public int GetIntValue(string key, int defaultValue = 0)
    {
        if (intDataDic == null) intDataDic = new Dictionary<string, int>();
        if (intDataDic.ContainsKey(key))
        {
            return intDataDic[key];
        }
        return defaultValue;
    }

    public bool ChangeFloatValue(string key, float value)
    {
        if (floatDataDic == null) floatDataDic = new Dictionary<string, float>();
        if (floatDataDic.ContainsKey(key))
        {
            floatDataDic[key] += value;
            return true;
        }
        return false;
    }

    public void SetFloatValue(string key, float value)
    {
        if (floatDataDic == null) floatDataDic = new Dictionary<string, float>();
        floatDataDic[key] = value;
    }

    public bool CheckPercentFloatValue(string key)
    {
        float value = GetFloatValue(key);
        if (value == 0) return false;
        return UnityEngine.Random.Range(0, 100f) <= value;
    }

    public float GetFloatValue(string key, float defaultValue = 0)
    {
        if (floatDataDic == null) floatDataDic = new Dictionary<string, float>();
        if (floatDataDic.ContainsKey(key))
        {
            return floatDataDic[key];
        }
        return defaultValue;
    }

    public bool StringValueContain(string key, string value, bool isComplete = true)
    {
        if (stringDataDic == null) stringDataDic = new Dictionary<string, string>();
        if (stringDataDic.ContainsKey(key))
        {
            foreach (var subValue in stringDataDic[key].Split(Constant_AttributeString.SEPERATE_CHAR))
            {
                if (isComplete)
                {
                    if (subValue.Equals(value)) return true;
                }
                else
                {
                    if (subValue.Contains(value)) return true;
                }
            }
        }
        return false;
    }

    public bool RemoveStringValue(string key, string value)
    {
        if (stringDataDic == null) stringDataDic = new Dictionary<string, string>();
        if (stringDataDic.ContainsKey(key))
        {
            string newValue = "";
            bool hasAddSep = false;
            bool hasRemove = false;
            foreach (var subValue in stringDataDic[key].Split(Constant_AttributeString.SEPERATE_CHAR))
            {
                if (subValue.Equals(value))
                {
                    hasRemove = true;
                    continue;
                }
                if (!hasAddSep) hasAddSep = true;
                else newValue += Constant_AttributeString.SEPERATE_CHAR;
                newValue += subValue;
            }
            return hasRemove;
        }
        return false;
    }


    public void AddStringValue(string key, string value)
    {
        if (stringDataDic == null) stringDataDic = new Dictionary<string, string>();
        if (stringDataDic.ContainsKey(key))
        {
            if (string.IsNullOrEmpty(stringDataDic[key]))
            {
                stringDataDic[key] = value;
            }
            else
            {
                stringDataDic[key] += Constant_AttributeString.SEPERATE_CHAR + value;
            }
        }
        else
        {
            stringDataDic[key] = value;
        }

    }

    public void SetStringValue(string key, string value)
    {
        if (stringDataDic == null) stringDataDic = new Dictionary<string, string>();
        stringDataDic[key] = value;
    }

    public string GetStringValue(string key, string defaultValue = "")
    {
        if (stringDataDic == null) stringDataDic = new Dictionary<string, string>();
        if (stringDataDic.ContainsKey(key))
            return stringDataDic[key];
        return defaultValue;
    }

}

//数据聚合注入接口
public interface IAggregationDataInject
{
    bool InjectData(AggregationEntity entity);
}