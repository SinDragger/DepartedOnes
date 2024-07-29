using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 资源类
/// </summary>
public class Resource : AggregationEntity
{
    /// <summary>
    /// 显示用的名称
    /// </summary>
    public string name;
    /// <summary>
    /// 每单位重量-用于计算载重。默认是1
    /// </summary>
    public float weight = 1;
    /// <summary>
    /// 工作消耗
    /// </summary>
    public int workLoad;
    /// <summary>
    /// 配方消耗
    /// </summary>
    public EntityStack[] cost;
}
