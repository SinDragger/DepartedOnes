using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 改变破甲攻击力
/// </summary>
public class ChangeArmorBreakingAttackPower : StatusEffectTerm
{
    public int value;
    public override void Execution(AggregationEntity target)
    {
        CommandUnit command = target as CommandUnit;
        command.breakAttackBonus += value;

    }
    public override void ReverseExecution(AggregationEntity target)
    {
        CommandUnit command = target as CommandUnit;
        command.breakAttackBonus -= value;

    }
}
//TODO change weapon base attack power 改变武器基础攻击力
//TODO change weapon armor breaking attack power 改变武器破甲攻击力
