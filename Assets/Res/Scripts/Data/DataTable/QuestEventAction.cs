using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestEventAction : AggregationEntity
{
    public virtual void OnInvoke(EventData eventData) { }

    public virtual string GetAwardText() { return ""; }
}
