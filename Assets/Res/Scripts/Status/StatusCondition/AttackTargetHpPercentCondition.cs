using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTargetHpPercentCondition : StatusCondition
{
    public float percent;//需要满足触发
   

    /// <summary>
    /// 目标单位的生命值小于
    /// </summary>
    /// <param name="paramValue"></param>
    /// <returns></returns>
    public override bool CheckCondition(EntityStatus entity)
    {
        var target = (entity.dataModel as SoldierStatus).focusTarget;
        if (target == null) return false;
        if (((float)target.nowHp / (float)target.EntityData.maxLife) > percent)
        {
            return true;
        }
        return false;
    }
}