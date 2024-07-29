using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachEffectParticle : StatusEffectTerm
{
    public float prefabName;
    public override void Execution(AggregationEntity target)
    {
        CommandUnit command = target as CommandUnit;
        //command.ForEachAlive;

    }
    public override void ReverseExecution(AggregationEntity target)
    {
        CommandUnit command = target as CommandUnit;
        //command.nowWeaponSpeedDelta -= speed;
    }
}
