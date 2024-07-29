using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopControl : AggregationEntity
{
    //部队类型-决定部队使用的模型。否则会导致装备混乱
    /// <summary>
    /// 部队规模上限数量
    /// </summary>
    public int maxNum;
    public int nowNum
    {
        get
        {
            return troopEntity.nowNum;
        }
        set
        {
            troopEntity.nowNum = value;
        }
    }
    /// <summary>
    /// 人数缺口
    /// </summary>
    public int shortage => maxNum - nowNum;
    /// <summary>
    /// 装备来自的势力-用于区别
    /// </summary>
    public int belong;
    /// <summary>
    /// 部队存在实体——装备、属性、种族构成等
    /// </summary>
    public TroopEntity troopEntity;

    //存储一些更定制化的部队内容

    /// <summary>
    /// 快速构建单种类的部队
    /// </summary>
    /// <param name="num"></param>
    public TroopControl(string unitIdName, string speciesType, int num, int belong = 0)
    {
        //单位主体标识ID-通用
        idName = unitIdName;
        troopEntity = new TroopEntity(DataBaseManager.Instance.GetSpeciesTypeUnitData(unitIdName, speciesType));
        nowNum = num;
        this.belong = belong;
        SpineAtlasManager.Instance.RegisterSpineAbleUse(troopEntity.originData, troopEntity.speciesType, troopEntity.subSpeciesType, this.belong);
        maxNum = troopEntity.maxNum;
    }

    public TroopControl(string unitIdName, string speciesType, string subSpeciesType, int num, int belong = 0)
    {
        //单位主体标识ID-通用
        idName = unitIdName;
        troopEntity = new TroopEntity(DataBaseManager.Instance.GetSpeciesTypeUnitData(unitIdName, speciesType, subSpeciesType));
        nowNum = num;
        this.belong = belong;
        SpineAtlasManager.Instance.RegisterSpineAbleUse(troopEntity.originData, troopEntity.speciesType, troopEntity.subSpeciesType, this.belong);
        maxNum = troopEntity.maxNum;
    }
    public TroopControl(string unitIdName, int num, int belong)
    {
        //单位主体名称
        idName = unitIdName;
        this.belong = belong;
        troopEntity = new TroopEntity(DataBaseManager.Instance.GetSpeciesTypeUnitData(unitIdName));
        nowNum = num;
        SpineAtlasManager.Instance.RegisterSpineAbleUse(troopEntity.originData, troopEntity.speciesType, troopEntity.subSpeciesType, belong);
        maxNum = troopEntity.maxNum;
    }
    public TroopControl(string unitIdName, int belong)
    {
        //单位主体名称
        idName = unitIdName;
        this.belong = belong;
        troopEntity = new TroopEntity(DataBaseManager.Instance.GetSpeciesTypeUnitData(unitIdName));
        nowNum = troopEntity.maxNum;
        SpineAtlasManager.Instance.RegisterSpineAbleUse(troopEntity.originData, troopEntity.speciesType, troopEntity.subSpeciesType, belong);
        maxNum = troopEntity.maxNum;
    }
    public TroopControl(UnitData unitData, int num, int belong)
    {
        //单位主体名称
        idName = unitData.idName;
        this.belong = belong;
        troopEntity = new TroopEntity(unitData);
        nowNum = num;
        SpineAtlasManager.Instance.RegisterSpineAbleUse(troopEntity.originData, troopEntity.speciesType, troopEntity.subSpeciesType, belong);
        maxNum = troopEntity.maxNum;
    }

    /// <summary>
    /// 是否只是装备
    /// </summary>
    /// <returns></returns>
    public bool IsEquipSet()
    {
        return troopEntity.speciesType == Constant_AttributeString.SPECIES_EQUIP;
    }

    public bool ReinforceNumber(int num)
    {
        nowNum += num;
        return true;
    }

    public void LostNum(int num = 1)
    {
        nowNum -= num;
    }
}
