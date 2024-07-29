using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_ShowUnlockDialog : EventTriggerData
{
    /// <summary>
    /// 解锁次数
    /// </summary>
    public string contextIdName;
    public override bool Process()
    {
        if (TempDataManager.Instance.GetData<int>(contextIdName, 0) == 0)
        {
            TempDataManager.Instance.SetData<int>(contextIdName, 1);
            UIManager.Instance.ShowChainUI("AreaUnlockDialogPanel", (ui) =>
            {
                (ui as AreaUnlockDialogPanel).Init(DataBaseManager.Instance.GetIdNameDataFromList<TipText>(contextIdName).containText);
            });
        }
        return true;
    }
}
