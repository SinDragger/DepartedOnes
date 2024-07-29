using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 改变基础攻击值
/// </summary>
public class ChangeBasicAttackPower : StatusEffectTerm
{
    public int value;
    public override void Execution(AggregationEntity target)
    {
        CommandUnit command = target as CommandUnit;
        command.basicAttackBonus += value;

    }
    public override void ReverseExecution(AggregationEntity target)
    {
        CommandUnit command = target as CommandUnit;
        command.basicAttackBonus -= value;

    }
}
