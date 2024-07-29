using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDeployData
{
    /// <summary>
    /// 目标军团名称
    /// </summary>
    public string idName;
    /// <summary>
    /// 隐藏等级（用于发掘）
    /// </summary>
    public int hideLevel;
    /// <summary>
    /// 横向位置
    /// </summary>
    public float posX;
    public float posY;
    //工程的初始进度

    public EventTriggerData[] events;
}
