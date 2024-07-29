using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 科技解锁条件
/// </summary>
public class StopContinousShootCondition : StatusCondition
{
    /// <summary>
    /// </summary>
    /// <param name="paramValue"></param>
    /// <returns></returns>
    public override bool CheckCondition(EntityStatus entity)
    {
        if (entity.dataModel is CommandUnit)
        {
            var c = entity.dataModel as CommandUnit;
            if (c.TroopState == TroopState.SHOOTING && c.attackTarget != null)
            {
                CommandUnit lastAttackUnit = c.GetObjectValue("LastAttackTarget") as CommandUnit;
                if (c.attackTarget == lastAttackUnit)
                {
                    return false;
                }
                c.SetObjectValue("LastAttackTarget", c.attackTarget);
            }
            c.SetFloatValue("ContinousAttackTime", 0f);
        }
        return true;
    }
}
