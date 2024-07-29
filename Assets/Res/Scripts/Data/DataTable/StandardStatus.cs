using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态：状态具备多个效应器与自身的数据实体|控制
/// Heap堆叠纳入数据聚合体内部
/// </summary>
public class StandardStatus : AggregationEntity
{
    //步骤
    public string name;//策划定制的名字 方便项目内部了解
    public StatusLayerType type;
    public string describe;
    public string effectDescribe;
    public int needHide;
    public int heapMax = -1;//-1为非堆叠 >0为可堆叠
    public StatusCondition[] ableCondition;
    public StatusCondition[] disableCondition;
    public StatusEffectTerm[] activeEffect;
    public StatusEffectTerm[] continousEffect;
    public StatusEffectTerm[] overEffect;

    /// <summary>
    /// 是否时间相关
    /// </summary>
    public bool IsTimeRelevanceEffect()
    {
        return continousEffect != null;
    }
}


public enum StatusLayerType
{
    /// <summary>
    /// 单位的status
    /// </summary>
    UNIT_STATUS,
    /// <summary>
    ///指挥官的Status 
    /// </summary>
    COMMAND_STATUS,
}