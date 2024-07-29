using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierDeathResourceGainEffect : StatusEffectTerm
{
    public override void Execution(AggregationEntity target)
    {
        (target as SoldierStatus).onReverse += OnUnitDie;
    }

    void OnUnitDie()
    {
        GameManager.instance.playerData.soulPoint += 1;
    }
    public override void ReverseExecution(AggregationEntity target)
    {
        (target as SoldierStatus).onReverse -= OnUnitDie;
    }
}

