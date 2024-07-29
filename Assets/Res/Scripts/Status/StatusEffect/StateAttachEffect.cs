using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAttachEffect : StatusEffectTerm
{
    public string targetIdName;
    public override void Execution(AggregationEntity target)
    {
        target.SetBoolValue(targetIdName, true);
    }
}