using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StatusEffectTerm
{
    public bool IsTimeRelevanceEffect;

    public string describeBase;
    public virtual string describe => describeBase;
    public virtual void Execution(AggregationEntity target) { }
    public virtual void ReverseExecution(AggregationEntity target) { }
}