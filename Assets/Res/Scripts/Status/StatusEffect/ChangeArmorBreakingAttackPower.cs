using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �ı��Ƽ׹�����
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
//TODO change weapon base attack power �ı���������������
//TODO change weapon armor breaking attack power �ı������Ƽ׹�����
