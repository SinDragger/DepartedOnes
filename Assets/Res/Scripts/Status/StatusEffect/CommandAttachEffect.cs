using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandAttachEffect : StatusEffectTerm
{
    public string targetIdName;
    public override void Execution(AggregationEntity target)
    {
        if (target is CommandUnit)
        {
            target.SetBoolValue(targetIdName, true);
        }
    }
}