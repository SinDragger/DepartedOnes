using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegionDefendState : LegionState
{
    LegionControl nowLegion;
    public LegionDefendState(LegionControl legion)
    {
        nowLegion = legion;
    }

    public override void OnUpdate(float deltaTime)
    {
        var list = LegionManager.Instance.GetAttackAbleList(nowLegion);
        if (list != null)
        {
            if (!GameManager.instance.isAIAttack) return;
            //TODO:增加目标选择
            nowLegion.State = new LegionAttackState(nowLegion,list[0]);
        }
        //向Manager申请获取对周围半径的可感知部队
    }

    public override string GetLegionStateIcon()
    {
        return "LEGION_STATE_DEFEND";
    }
}
