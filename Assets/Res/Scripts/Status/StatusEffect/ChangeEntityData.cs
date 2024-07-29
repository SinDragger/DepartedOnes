using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeEntityData : StatusEffectTerm
{
    public EntityStack[] intArray;
    public override void Execution(AggregationEntity target)
    {
        for (int i = 0; i < intArray.Length; i++)
        {
            target.ChangeIntValue(intArray[i].idName, intArray[i].num);
        }
    }
    public override void ReverseExecution(AggregationEntity target)
    {
        for (int i = 0; i < intArray.Length; i++)
        {
            target.ChangeIntValue(intArray[i].idName, -intArray[i].num);
        }
    }
}
