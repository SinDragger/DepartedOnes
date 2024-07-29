using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSpecies : AggregationEntity
{
    /// <summary>
    /// 名称
    /// </summary>
    public string name;
    /// <summary>
    /// 物种类型
    /// </summary>
    public string species;
    //置换的属性
    public int maxlifeDelta;
    public int destructivePowerDelta;
    public int hitRateDelta;
    public int defendDelta;
    public float speedDelta;
    public int weightBearingDelta;
    public int moraleDeviationValue;
    /// <summary>
    /// 置换的图层
    /// </summary>
    public string[] replaceTextures;
    public string[] statusAttachs;
}
