using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Debug输出端。用于对于目标内容的屏蔽
/// </summary>
public class DebugManager : Singleton<DebugManager>
{
    public static void LogError(object result)
    {
        Debug.LogError(result.ToString());
    }


}
