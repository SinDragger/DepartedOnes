using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GeneralData : AggregationEntity
{
    /// <summary>
    /// 基础兵种样式
    /// </summary>
    public string unitType;

    public string speciesType; 

    /// <summary> 
    /// 贴图IdName
    /// </summary>
    public string texIdName;
    /// <summary>
    /// 头像图片
    /// </summary>
    public string headIconTexIdName;
    //人物技能 skillIdName->skillData
    public string[] skillIdNames;

    //物品->itemName->skillNamd->SkillData
    public string itemName;

    public float maxEnergy;
    /// <summary>
    /// 非General上的状态类似光环，眩晕，燃烧之类的每个实例都会有的Status
    /// </summary>
    public string[] statusAttachs;

    public string[] selfStatusAttachs;
}
