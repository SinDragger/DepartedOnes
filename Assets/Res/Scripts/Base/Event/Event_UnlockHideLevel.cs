using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_UnlockHideLevel : EventTriggerData
{
    /// <summary>
    /// 解锁次数
    /// </summary>
    public int unlockLevels = 1;
    public override bool Process()
    {
        for (int i = 0; i < unlockLevels; i++)
        {
            SectorBlockManager.Instance.UpdateToNextShowLevel();
        }
        return true;
    }
}
