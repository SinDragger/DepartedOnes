using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 装备数据
/// </summary>
public partial class UnitData : AggregationEntity, IXMLPrintable
{
    /// <summary>
    /// 大小倍数
    /// </summary>
    public float ocupySize = 1f;
    /// <summary>
    /// 生命值
    /// </summary>
    public int maxLife;

    public string originIdName;

    /// <summary>
    /// 破坏力
    /// </summary>
    public int destructivePower;

    /// <summary>
    /// 命中技巧
    /// </summary>
    public int hitRate;

    /// <summary>
    /// 防御等级
    /// </summary>
    public int defendLevel;

    /// <summary>
    /// 负重能力
    /// </summary>
    public int weightBearing;

    /// <summary>
    /// 移动速度
    /// </summary>
    public float speed;

    /// <summary>
    /// 士气 TODO 需要做成从种族中去获取 再加上调整值
    /// </summary>
    public int morale;

    /// <summary>
    /// 点数消耗
    /// </summary>
    public int cost;

    /// <summary>
    /// 点数消耗总额
    /// </summary>
    public int costDelta;

    public UnitData Clone()
    {
        return (UnitData)this.MemberwiseClone();
    }
    /// <summary>
    /// 创造一个其他生物的副本
    /// </summary>
    public UnitData CloneToOtherSpecies(string newSpeciesType, string newSubSpeciesType = null)
    {
        //数据层逆向附加。
        UnitData result = (UnitData)this.MemberwiseClone();
        Species oldSpecies = DataBaseManager.Instance.GetIdNameDataFromList<Species>(speciesType);
        Species newSpecies = DataBaseManager.Instance.GetIdNameDataFromList<Species>(newSpeciesType);
        result.speciesType = newSpeciesType;
        //TODO:滞销不符合的特性。
        result.maxLife -= oldSpecies.basicMaxLife;
        result.maxLife += newSpecies.basicMaxLife;

        //TODO:附加所有特性。
        result.hitRate -= oldSpecies.basicHitRate;
        result.hitRate += newSpecies.basicHitRate;

        result.defendLevel -= oldSpecies.basicDefendLevel;
        result.defendLevel += newSpecies.basicDefendLevel;

        result.weightBearing -= oldSpecies.basicWeightBearing;
        result.weightBearing += newSpecies.basicWeightBearing;

        result.speed /= oldSpecies.basicSpeed;
        result.speed *= newSpecies.basicSpeed;

        result.morale -= oldSpecies.basicMorale;
        result.maxLife += newSpecies.basicMorale;

        if (!string.IsNullOrEmpty(result.subSpeciesType))
        {
            var oldSubSpecies = DataBaseManager.Instance.GetIdNameDataFromList<SubSpecies>(result.subSpeciesType);
            result.maxLife -= oldSubSpecies.maxlifeDelta;
            result.hitRate -= oldSubSpecies.hitRateDelta;
            result.defendLevel -= oldSubSpecies.defendDelta;
            result.weightBearing -= oldSubSpecies.weightBearingDelta;
            result.speed -= oldSubSpecies.speedDelta;
            result.morale -= oldSubSpecies.moraleDeviationValue;
        }
        if (!string.IsNullOrEmpty(newSubSpeciesType))
        {
            var newSubSpecies = DataBaseManager.Instance.GetIdNameDataFromList<SubSpecies>(newSubSpeciesType);
            result.maxLife += newSubSpecies.maxlifeDelta;
            result.hitRate += newSubSpecies.hitRateDelta;
            result.defendLevel += newSubSpecies.defendDelta;
            result.weightBearing += newSubSpecies.weightBearingDelta;
            result.speed += newSubSpecies.speedDelta;
            result.morale += newSubSpecies.moraleDeviationValue;
            result.subSpeciesType = newSubSpeciesType;
        }
        else
            result.subSpeciesType = null;

        //遍历两者的int和float内容并进行综合附加
        //确定拷贝等级
        //需要重新计算负重与速度 以及被动技能生效
        List<string> speciesStatusAttach = new List<string>();
        if (result.statusAttachs != null)
            speciesStatusAttach.AddRange(result.statusAttachs);
        //if (result.statusAttachs)
        if (oldSpecies.statusAttachs != null)
            foreach (var oldSpeciesStatusAttach in oldSpecies.statusAttachs)
            {
                if (speciesStatusAttach.Remove(oldSpeciesStatusAttach))
                {
                    result.RemoveStatus(DataBaseManager.Instance.GetIdNameDataFromList<StandardStatus>(oldSpeciesStatusAttach));
                }
            }
        if (newSpecies.statusAttachs != null)
            foreach (var newSpeciesStatusAttach in newSpecies.statusAttachs)
            {
                speciesStatusAttach.Add(newSpeciesStatusAttach);
                result.GetStatus(DataBaseManager.Instance.GetIdNameDataFromList<StandardStatus>(newSpeciesStatusAttach));
            }
        //移除原有种族特性 添加新的种族特性 
        //删除原有的种族特性  准备
        result.statusAttachs = speciesStatusAttach.ToArray();
        result.originIdName = idName;
        return result;
    }

    void GetStatus(StandardStatus standardStatus)
    {
        if (standardStatus.ableCondition != null)
        {
            foreach (var condition in standardStatus.ableCondition)
            {
                if (condition is UnitCondition)
                {
                    if (!condition.CheckCondition(this)) return;
                }
                else
                {
                    return;
                }
            }
        }
        foreach (var term in standardStatus.activeEffect)
        {
            if (term is ChangeUnitData)
            {
                term.Execution(this);
            }
            else
            {
                return;
            }
        }
    }

    void RemoveStatus(StandardStatus standardStatus)
    {
        if (standardStatus.ableCondition != null)
        {
            foreach (var condition in standardStatus.ableCondition)
            {
                if (condition is UnitCondition)
                {
                    if (!condition.CheckCondition(this)) return;
                }
                else
                {
                    return;
                }
            }
        }
        foreach (var term in standardStatus.activeEffect)
        {
            if (term is ChangeUnitData)
            {
                term.ReverseExecution(this);
            }
            else
            {
                return;
            }
        }
    }

}