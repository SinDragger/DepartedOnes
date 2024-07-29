using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceData : AggregationEntity
{
    public int id;
    /// <summary>
    /// 部队名称
    /// </summary>
    public string name;
    /// <summary>
    /// 统称
    /// </summary>
    public string nickName;
    /// <summary>
    /// 识别颜色
    /// </summary>
    public string recognizeColor;
    /// <summary>
    /// 核心物种-默认兵种图标使用的样式
    /// </summary>
    public string mainSpecies;
    /// <summary>
    /// 可使用的武器列表
    /// </summary>
    public string[] ableWeaponEquipSets;
    /// <summary>
    /// 可使用的护甲列表
    /// </summary>
    public string[] ableArmourEquipSets;
    /// <summary>
    /// 可使用的主要兵源列表
    /// </summary>
    public string[] ableSpecies;
    /// <summary>
    /// 可使用的延伸兵源列表
    /// </summary>
    public string[] ableSubSpieces;
    /// <summary>
    /// 可使用的兵种列表
    /// </summary>
    public string[] ableUnitDatas;
    public string[] buyableUnitDatas;
    /// <summary>
    /// 可使用的兵种训练
    /// </summary>
    public string[] ableUnitTrainStatus;
    //兵种树？
    //人名库？
    public string[] basicUnlimitedWeaponIds;
}
