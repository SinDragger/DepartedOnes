using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAttackSpeed : StatusEffectTerm
{
    public float speed;

    public override void Execution(AggregationEntity target)
    {
        SoldierStatus soldier = target as SoldierStatus;
        soldier.nowWeaponSpeed += speed;

    }
    public override void ReverseExecution(AggregationEntity target)
    {
        SoldierStatus soldier = target as SoldierStatus;

        soldier.nowWeaponSpeed -= speed;

    }

}
