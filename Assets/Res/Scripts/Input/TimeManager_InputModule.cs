using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制器注册
/// </summary>
public class TimeManager_InputModule : InputModule
{
    public override void Init()
    {
        RegistQuickKeyDown(InputType.TIME_MAX, TimeManager.Instance.MaxTimeSpeed);
        RegistQuickKeyDown(InputType.TIME_MIN, TimeManager.Instance.MinTimeSpeed);
        RegistQuickKeyDown(InputType.TIME_ADD, TimeManager.Instance.AddTimeSpeed);
        RegistQuickKeyDown(InputType.TIME_MINUS, TimeManager.Instance.MinusTimeSpeed);
        RegistQuickKeyDown(InputType.TIME_SWITCH, TimeManager.Instance.SwitchTimeSpeed);
        base.Init();
    }
}
