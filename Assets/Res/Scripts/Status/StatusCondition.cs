using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StatusCondition
{
    public virtual bool CheckCondition(EntityStatus entity) { return true;}
    public virtual bool CheckCondition(AggregationEntity entity) { return true;}
    public virtual void Init(AggregationEntity target) { }
    public virtual void Init(string[] parmaValue) { }
}
