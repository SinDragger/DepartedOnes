using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestEventResolver : AggregationEntity
{
    public string eventName;
    public virtual int ResolveData(EventData data) { return 0; }
}
