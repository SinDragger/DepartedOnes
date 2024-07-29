using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BelongCondition : StatusCondition
{
    /// <summary>
    /// 0:相同触发 1:不同触发
    /// </summary>
    public int effectType;

    public override void Init(AggregationEntity target)
    {
        base.Init(target);
    }
    /// <summary>
    /// 1
    /// </summary>
    /// <param name="paramValue"></param>
    /// <returns></returns>
    public override bool CheckCondition(EntityStatus entity)
    {
        int selfBelong = entity.belong;
        int targetBelong = (entity.dataModel as IBelongable).Belong;
        if (effectType == 0 && selfBelong == targetBelong)
        {
            return true;
        }
        else if (effectType == 1 && selfBelong != targetBelong)
        {
            return true;
        }
        if (effectType == 2)
        {
            return true;
        }
        return false;
    }
}
