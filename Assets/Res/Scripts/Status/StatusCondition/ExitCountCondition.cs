using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitCountCondition : StatusCondition
{
    public int exitCount;
    public override bool CheckCondition(EntityStatus entity)
    {
        return entity.heapNum > exitCount;
    }

    public override void Init(AggregationEntity target)
    {
       
    }
}

