using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachSoliderParticleEffect : StatusEffectTerm
{
    public string effectIdName;
    public override void Execution(AggregationEntity target)
    {
        if (target is CommandUnit)
        {
            CommandUnit command = target as CommandUnit;
            for (int i = 0; i < command.troopsData.Count; i++)
            {
                command.troopsData[i].model.AddEffect(effectIdName);
            }
        }
        else
        {
            SoldierStatus status = target as SoldierStatus;
            status.model.AddEffect(effectIdName);
        }

    }
    public override void ReverseExecution(AggregationEntity target)
    {
      
        if (target is CommandUnit)
        {
            CommandUnit command = target as CommandUnit;
            for (int i = 0; i < command.troopsData.Count; i++)
            {
                command.troopsData[i].model.RemoveEffect(effectIdName);
            }
        }
        else
        {
            SoldierStatus status = target as SoldierStatus;
            status.model.RemoveEffect(effectIdName);
        }
    }


}
