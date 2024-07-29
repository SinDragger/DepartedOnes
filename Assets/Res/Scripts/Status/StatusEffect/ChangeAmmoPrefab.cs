using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ChangeAmmoPrefab : StatusEffectTerm
{
    public string ammoName;
    // Start is called before the first frame update
    public override void Execution(AggregationEntity target)
    {
        CommandUnit command = target as CommandUnit;
        command.ammoName = ammoName;


    }
    public override void ReverseExecution(AggregationEntity target)
    {
        CommandUnit command = target as CommandUnit;
        command.ammoName = default;

    }
}
