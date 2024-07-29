using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestResolver_GetTroop : QuestEventResolver
{
    public string[] expandTroopIds;

    public override int ResolveData(EventData data)
    {
        var troop = data.GetValue<TroopControl>(Constant_QuestEventDataKey.PlayerGetTroop);
        if (expandTroopIds.Contains(troop.idName))
            return 1;
        return 0;
    }
}