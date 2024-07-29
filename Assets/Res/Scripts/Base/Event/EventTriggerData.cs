using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTriggerData
{
    public Dictionary<string, object> context = new Dictionary<string, object>();
    /// <summary>
    /// 处理事件
    /// </summary>
    /// <returns>是否消耗了</returns>
    public virtual bool Process()
    {
        return true;
    }
}
