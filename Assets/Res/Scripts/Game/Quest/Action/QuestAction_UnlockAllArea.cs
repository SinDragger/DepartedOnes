using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestAction_UnlockAllArea : QuestEventAction
{
    public override void OnInvoke(EventData eventData)
    {
        SectorBlockManager.Instance.UpdateToNextShowLevel();
        SectorBlockManager.Instance.UpdateToNextShowLevel();
        SectorBlockManager.Instance.UpdateToNextShowLevel();
    }
}
