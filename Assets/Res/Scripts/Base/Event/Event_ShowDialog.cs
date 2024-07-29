using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_ShowDialog : EventTriggerData
{
    /// <summary>
    /// 解锁次数
    /// </summary>
    public string contextIdName;
    public override bool Process()
    {
        UIManager.Instance.ShowChainUI("SimpleDialogPanel", (ui) =>
        {
            (ui as SimpleDialogPanel).Init(DataBaseManager.Instance.GetIdNameDataFromList<TipText>(contextIdName).containText, null);
        });
        return true;
    }
}
