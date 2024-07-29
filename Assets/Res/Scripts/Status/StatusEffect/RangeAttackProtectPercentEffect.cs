using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttackProtectPercentEffect : StatusEffectTerm
{
    public float percent;

   
    public override void Execution(AggregationEntity target)
    {
        target.SetFloatValue("RangeAttackProtectPercent", percent);
    }
}

