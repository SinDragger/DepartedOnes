using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountCondition : StatusCondition
{
    public int count;//需要满足触发的次数

    /// <summary>
    /// 1
    /// </summary>
    /// <param name="paramValue"></param>
    /// <returns></returns>
    public override bool CheckCondition(EntityStatus entity)
    {
        if (entity.heapNum >= count)
        {
            return true;
        }
        return false;
    }
}
