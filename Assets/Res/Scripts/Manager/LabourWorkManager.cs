using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 劳动力管理-推进工作
/// </summary>
public class LabourWorkManager : Singleton<LabourWorkManager>
{
    /// <summary>
    /// 劳动力点数
    /// </summary>
    public int labourPoint;

    protected override void Init()
    {
        base.Init();
        GameManager.timeRelyMethods += OnUpdate;
    }

    void OnUpdate(float deltaTime)
    {
        labourPoint = RefugeeManager.Instance.ableRefugeeNum;
    }

}
