using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegionWaitState : LegionState
{
    //有序等待~无序等待

    public LegionWaitState()
    {

    }

    public LegionWaitState(float waitTime)
    {

    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
    }
    public override string GetLegionStateIcon()
    {
        return "LEGION_STATE_UNKNOWN";
    }
}
