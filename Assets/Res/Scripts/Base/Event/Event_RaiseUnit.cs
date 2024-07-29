using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_RaiseUnit : EventTriggerData
{
    public string unitIdName;
    public override bool Process()
    {
        LegionControl legion = context["Legion"] as LegionControl;
        UnitData unitData = DataBaseManager.Instance.GetIdNameDataFromList<UnitData>(unitIdName);
        //System.Action callback = context["Callback"] as System.Action;
        UIManager.Instance.ShowChainUI("RaiseUnitPanel", (r) =>
        {
            (r as RaiseUnitPanel).Init(legion, unitData, null);
        });
        return true;
    }
}
