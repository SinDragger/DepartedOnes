using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestResolver_KillTarget : QuestEventResolver
{
    public string[] killTargetIds;

    public override int ResolveData(EventData data)
    {
        int result = 0;
        var warData = data.GetValue<WarBattle>(Constant_QuestEventDataKey.PlayerWinBattle);
        if (warData != default)
        {
            foreach (var legion in warData.GetEnermyLegions())
            {
                if(killTargetIds.Contains(legion.idName))
                    result++;
            }
        }
        return result;
    }
}
