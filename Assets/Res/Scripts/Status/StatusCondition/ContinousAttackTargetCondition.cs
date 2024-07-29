using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 科技解锁条件
/// </summary>
public class ContinousAttackTargetCondition : StatusCondition
{
    public float needTime;
    public float attackSpeedChange;
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
                float continueTime = c.GetFloatValue("ContinousAttackTime");
                if (c.attackTarget == lastAttackUnit)
                {
                    continueTime += Time.deltaTime;
                    if (continueTime > needTime)
                    {
                        return true;
                    }
                }
                else
                {
                    continueTime = 0;
                }
                c.SetFloatValue("ContinousAttackTime", continueTime);
                c.SetObjectValue("LastAttackTarget", c.attackTarget);
            }
            else
            {
                c.SetFloatValue("ContinousAttackTime", 0f);
            }
        }
        return false;
    }
}
