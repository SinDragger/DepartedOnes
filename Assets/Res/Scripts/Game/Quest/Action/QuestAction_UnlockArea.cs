using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestAction_UnlockArea : QuestEventAction
{
    public override void OnInvoke(EventData eventData)
    {
        var block = SectorBlockManager.Instance.GetBlock(LegionManager.Instance.nowLegion.position);
        foreach(var construction in block.constructions)
        {
            if (construction.hideLevel > 0)
            {
                construction.hideLevel--;
                if (construction.hideLevel == 0)
                {
                    ConstructionManager.Instance.GetMapUI(construction)?.OnShow();
                }
            }
        }
    }
}
