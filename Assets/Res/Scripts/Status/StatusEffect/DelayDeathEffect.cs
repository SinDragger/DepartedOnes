using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayDeathEffect : StatusEffectTerm
{
    public override void Execution(AggregationEntity target)
    {
        target.SetBoolValue(Constant_AttributeString.STATUS_DELAY_DEATH, true);
    }
}