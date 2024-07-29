using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RogueNodeBehavior : AggregationEntity
{
    public virtual bool NodeCondition()
    {
        return true;
    }
}
