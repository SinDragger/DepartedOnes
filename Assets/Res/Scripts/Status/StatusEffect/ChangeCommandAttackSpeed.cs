using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCommandAttackSpeed : StatusEffectTerm
{
    public float speed;
    public override void Execution(AggregationEntity target)
    {
        CommandUnit command = target as CommandUnit;
        command.nowWeaponSpeedDelta += speed;

    }
    public override void ReverseExecution(AggregationEntity target)
    {
        CommandUnit command = target as CommandUnit;
        command.nowWeaponSpeedDelta -= speed;
    }
}
