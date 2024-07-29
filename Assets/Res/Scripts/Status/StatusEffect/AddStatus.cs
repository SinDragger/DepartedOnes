using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class AddStatus : StatusEffectTerm
{
    public string statusName;

    public override void Execution(AggregationEntity target)
    {
        SoldierStatus soldier = target as SoldierStatus;
        foreach (var status in soldier.status)
        {
            if (status.originStatus.idName == statusName)
                return;
        }
        soldier.status.Add(StatusManager.Instance.RequestStatus(statusName, soldier, soldier.commander.belong));
    }

    public override void ReverseExecution(AggregationEntity target)
    {

    }
}
