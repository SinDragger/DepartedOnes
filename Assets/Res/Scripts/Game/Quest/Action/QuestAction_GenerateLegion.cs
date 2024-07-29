using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestAction_GenerateLegion : QuestEventAction
{
    public string legionId;
    public int legionBelong;
    public int posX;
    public int posY;

    public override void OnInvoke(EventData eventData)
    {
        var legion = LegionManager.Instance.DeployTargetLegionData(legionId, new Vector2(posX, posY), legionBelong);
        legion.State = new LegionDefendState(legion);
    }
}
