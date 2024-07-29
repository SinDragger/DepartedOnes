using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗中的State
/// </summary>
public class LegionCombatState : LegionState
{
    LegionControl nowLegion;
    public LegionCombatState(LegionControl legion)
    {
        nowLegion = legion;
    }
    public override void EnterState()
    {
        LegionManager.Instance.SetUI(nowLegion, (ui) =>
        {
            ui.IntoBattleMode();
        });
        base.EnterState();
    }
    public override void LeaveState()
    {
        LegionManager.Instance.SetUI(nowLegion, (ui) =>
        {
            ui.LeaveBattleMode();
        });
        base.LeaveState();
        //
    }
    public override string GetLegionStateIcon()
    {
        return "LEGION_STATE_BATTLE";
    }
}
