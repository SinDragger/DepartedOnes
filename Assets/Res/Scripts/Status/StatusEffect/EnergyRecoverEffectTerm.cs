using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyRecoverEffectTerm : StatusEffectTerm
{
    /// <summary>
    /// 释放技能所需能量
    /// </summary>
    public override void Execution(AggregationEntity target)
    {
        float nowEnergy = target.GetFloatValue(Constant_AttributeString.GENERAL_ENERGY);
        float maxEnergy = target.GetFloatValue(Constant_AttributeString.GENERAL_ENERGYMAX);
        float recovery  = target.GetFloatValue(Constant_AttributeString.GENERAL_ENERGY_RECOVERY_VALUE);
        if (nowEnergy < maxEnergy)
        {
            target.SetFloatValue(Constant_AttributeString.GENERAL_ENERGY, Mathf.Min(nowEnergy + recovery * Time.deltaTime, maxEnergy));
        }
    }
}

