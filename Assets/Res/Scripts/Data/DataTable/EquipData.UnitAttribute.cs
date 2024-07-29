using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 装备数据
/// </summary>
public partial class EquipData : AggregationEntity, IXMLPrintable
{
    /// <summary>
    /// 最小伤害值
    /// </summary>
    public int minDamage;
    /// <summary>
    /// 最大伤害值
    /// </summary>
    public int maxDamage;
    /// <summary>
    /// 伤害类型
    /// </summary>
    public string damageType;
}