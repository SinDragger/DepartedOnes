using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegionAttackState : LegionState
{
    LegionControl nowLegion;
    LegionControl targetLegion;
    public LegionAttackState(LegionControl legion,LegionControl attackTarget)
    {
        nowLegion = legion;
        targetLegion = attackTarget;
    }

    public override void OnUpdate(float deltaTime)
    {
        nowLegion.position = Vector2.MoveTowards(nowLegion.position, targetLegion.position, nowLegion.moveSpeed * TimeManager.Instance.nowDeltaTime / 3600f);

        if (Vector2.Distance(nowLegion.position, targetLegion.position) < GetMinCombatDistance())
        {
            LegionManager.Instance.WarBreakOut(nowLegion, targetLegion);
        }
        //向Manager申请获取对周围半径的可感知部队
    }

    float GetMinCombatDistance()
    {
        return 2f;
    }

    public override string GetLegionStateIcon()
    {
        return "LEGION_STATE_ATTACK";
    }
}
