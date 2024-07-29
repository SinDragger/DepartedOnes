using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMoveSpeed : StatusEffectTerm
{
    public float changePercent;
    //float soldiernowSpeedPercent;
    public override void Execution(AggregationEntity target)
    {
        SoldierStatus soldier = target as SoldierStatus;
        //soldiernowSpeedPercent = soldier.nowSpeedPercent;
        soldier.ChangeNowSpeed(changePercent);
    }

    public override void ReverseExecution(AggregationEntity target)
    {
        SoldierStatus soldier = target as SoldierStatus;
        soldier.ChangeNowSpeed(-changePercent);
        //soldier.nowSpeedPercent = soldiernowSpeedPercent;
    }
}
