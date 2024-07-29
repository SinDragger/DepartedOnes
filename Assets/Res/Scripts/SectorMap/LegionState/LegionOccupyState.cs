using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地点占领
/// </summary>
public class LegionOccupyState : LegionState
{
    LegionControl nowLegion;
    public SectorConstruction sectorConstruction;
    public LegionOccupyState(LegionControl legion, SectorConstruction sectorConstruction)
    {
        nowLegion = legion;
        this.sectorConstruction = sectorConstruction;
        //军团驻扎进目标构造体之中
        sectorConstruction.BeStatetioned(nowLegion);
        LegionManager.Instance.SetUI(nowLegion, (ui) =>
        {
            var sectorConstructionUI = ConstructionManager.Instance.GetMapUI(sectorConstruction);
            ui.IntoOccupyMode(sectorConstructionUI);
        });
    }

    public override void LeaveState()
    {
        base.LeaveState();
        sectorConstruction.BeLeaved(nowLegion);
        LegionManager.Instance.SetUI(nowLegion, (ui) =>
        {
            ui.LeaveOccupyMode();
        });
    }

    public void PackUpAllResource()
    {
        nowLegion.resourcePool.resourceCarry.DictionaryAppend(sectorConstruction.resourcesStore);
        sectorConstruction.resourcesStore.Clear();
    }

    public override string GetLegionStateIcon()
    {
        return "LEGION_STATE_OCCUPY";
    }
}
