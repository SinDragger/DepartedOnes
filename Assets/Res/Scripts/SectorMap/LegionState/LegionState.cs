using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 军团状态
/// </summary>
public abstract class LegionState:ITimeUpdatable
{
    public virtual void EnterState()
    {

    }
    /// <summary>
    /// 状态刷新
    /// </summary>
    public virtual void OnUpdate(float deltaTime)
    {

    }
    public virtual void LeaveState()
    {

    }

    public virtual string GetLegionStateIcon()
    {
        return "LEGION_STATE_UNKNOWN";
    }
}
