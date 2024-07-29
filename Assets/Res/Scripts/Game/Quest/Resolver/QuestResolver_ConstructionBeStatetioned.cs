using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestResolver_ConstructionBeStatetioned : QuestEventResolver
{
    public string constructionId; //被驻扎的建筑
    public string legionId;      //驻扎的军队

    public override int ResolveData(EventData data)
    {
        Debug.Log(data.GetValue<Construction>(Constant_QuestEventDataKey.OnConstructionBeStatetioned_Con).idName);
        Debug.Log(data.GetValue<LegionControl>(Constant_QuestEventDataKey.OnConstructionBeStatetioned_Legion).idName);
        if (data.GetValue<Construction>(Constant_QuestEventDataKey.OnConstructionBeStatetioned_Con).idName == constructionId &&
            data.GetValue<LegionControl>(Constant_QuestEventDataKey.OnConstructionBeStatetioned_Legion).idName == legionId)
            return 1;
        return 0;
    }
}
