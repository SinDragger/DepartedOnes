using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 区域控制进度
/// </summary>
public class SectorControlProcess : ITimeUpdatable
{
    public int belong;
    public float controlProcess;
    public float controlMax;
    public float controlSpeed;
    public bool needRemove => controlProcess <= 0f;
    public bool isMax => controlProcess == controlMax;

    List<LegionControl> legions = new List<LegionControl>();

    public SectorControlProcess(int belong)
    {
        this.belong = belong;
        controlMax = 100f;
        //默认自然消退速度
        //controlSpeed = -GameConfig.SectorControlDrop;
    }
    public bool IsControlEmpty()
    {
        return legions.Count == 0;
    }

    public void LegionStartControl(LegionControl legion)
    {
        if (legions.Contains(legion)) return;
        legions.Add(legion);
        controlSpeed += legion.ControlPower;
    }

    public void LegionLeaveControl(LegionControl legion)
    {
        if (!legions.Contains(legion)) return;
        legions.Remove(legion);
        controlSpeed -= legion.ControlPower;
    }

    public void TriggerDrop(float deltaTime)
    {
        controlProcess += GameConfig.SectorControlDrop * deltaTime / 3600f;
        controlProcess = Mathf.Clamp(controlProcess, 0, controlMax);
        controlProcess = 0;
    }

    public void OnUpdate(float deltaTime)
    {
        controlProcess += controlSpeed * deltaTime / 3600f;
        controlProcess = Mathf.Clamp(controlProcess, 0, controlMax);
    }
}
