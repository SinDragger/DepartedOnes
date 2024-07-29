using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Species : AggregationEntity
{
    /// <summary>
    /// 名称
    /// </summary>
    public string name;
    /// <summary>
    /// 使用的基础默认模型
    /// </summary>
    public string baseModelSet;
    /// <summary>
    /// 物种特殊模型附加
    /// </summary>
    public string additionModelSet;
    /// <summary>
    /// 基准生命值
    /// </summary>
    public int basicMaxLife;
    /// <summary>
    /// 基准破坏力
    /// </summary>
    public int basicDestructivePower;
    /// <summary>
    /// 基准命中技巧
    /// </summary>
    public int basicHitRate;
    /// <summary>
    /// 基准防御等级
    /// </summary>
    public int basicDefendLevel;
    /// <summary>
    /// 基准移动速度
    /// </summary>
    public float basicSpeed;
    /// <summary>
    /// 基准负重能力
    /// </summary>
    public int basicWeightBearing;
    /// <summary>
    /// 基准工作能力
    /// </summary>
    public int basicWorkAbility;
    /// <summary>
    /// 基准士气值
    /// </summary>
    public int basicMorale;

    //增加天赋能力，种族特性具备
    /// <summary>
    ///  StandardStatus的IDname 准备
    /// </summary>
    public string[] statusAttachs;

    public EntityStack[] cost;
}
