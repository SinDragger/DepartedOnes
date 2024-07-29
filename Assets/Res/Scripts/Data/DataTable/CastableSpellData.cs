using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastableSpellData : AggregationEntity
{
    /// <summary>
    /// 名称
    /// </summary>
    public string name;
    /// <summary>
    /// 描述名称
    /// </summary>
    public string des;
    public string costDes;
    public int soulPointCost;
    public float radius;
    public string iconResIdName;
    public EffectTerm[] effects;
    public int heapMax;

    public int multiCast;
    public float areaRandom;
    public int releaseType;
}

