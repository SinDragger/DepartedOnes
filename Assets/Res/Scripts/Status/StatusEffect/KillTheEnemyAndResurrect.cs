using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTheEnemyAndResurrect : StatusEffectTerm
{
    public string reviveName;
    public override void Execution(AggregationEntity target)
    {
        if (target is SoldierStatus)
        {
            (target as SoldierStatus).onKill += OnUnitKill;
        }
        else Debug.Log(target.GetType().ToString());
    }

    void OnUnitKill(SoldierStatus soldierStatus)
    {
        if (soldierStatus.EntityData.speciesType != "Beast") {
            List<SoldierStatus> willSeperateList = new List<SoldierStatus>();

            soldierStatus.commander.RaiseAndSeperateSoldier(soldierStatus, reviveName);
            willSeperateList.Add(soldierStatus);


            if (willSeperateList.Count > 0)
            {
                UnitControlManager.instance.RaiseToTempCommand(willSeperateList, BattleManager.instance.controlBelong);
            }

        }
    }
    public override void ReverseExecution(AggregationEntity target)
    {
        (target as SoldierStatus).onKill -= OnUnitKill;
    }

}
