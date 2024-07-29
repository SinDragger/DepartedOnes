using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableRemoteEffect : StatusEffectTerm
{
    public override void Execution(AggregationEntity target)
    {
        target.SetBoolValue("DisableRemote", true);
        if (target is SoldierStatus)
            (target as SoldierStatus).commander.SetBoolValue("DisableRemote", true);
    }
}