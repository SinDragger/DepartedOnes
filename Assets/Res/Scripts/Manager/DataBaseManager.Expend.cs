using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class DataBaseManager : Singleton<DataBaseManager>
{
    const string baseUnitDataId = "BasicHumanoid";
    public string GetModelName(UnitData unitdata)
    {
        return GetModelName(unitdata.armourEquipSetId, unitdata.weaponEquipSetId, unitdata.speciesType);
    }
    public string GetModelName(TroopEntity data)
    {
        return GetModelName(data.armourEquipSet.data.idName, data.weaponEquipSet.data.idName, data.speciesType);
    }
    /// <summary>
    /// 根据单位种族与装备寻找该加载的模型（包括特殊英雄类型）
    /// </summary>
    /// <param name="equipSetName"></param>
    /// <param name="unitModelName"></param>
    /// <returns></returns>
    public string GetModelName(string armourEquipSetId, string weaponEquipSetId, string speciesType)
    {
        var list = GetTargetDataList<EquipAbleModelData>();
        var model = list.Find((t) => t.baseSpeciesName.Equals(speciesType));
        if (model != null)
        {
            return model.idName;
        }
        return list[0].idName;
    }

    public string GetSpeciesName(string speciesType)
    {
        return speciesType;
    }

    public ModelMotionType GetModelMotionType(UnitData unit)
    {
        if (unit.weaponEquipSetId == null) return default;
        if (System.Enum.TryParse<ModelMotionType>(GetIdNameDataFromList<EquipSetData>(unit.weaponEquipSetId).TargetActionModel, out ModelMotionType result))
        {
            return result;
        }
        return default;
    }


    public ModelMotionType GetModelMotionType(TroopEntity troopEntity)
    {
        if (troopEntity.weaponEquipSet == null) return default;
        if (System.Enum.TryParse<ModelMotionType>(troopEntity.weaponEquipSet.data.TargetActionModel, out ModelMotionType result))
        {
            return result;
        }
        return default;
    }

    public UnitData GetSpeciesTypeUnitData(string unitDataIdName, string speciesIdName = default, string speciesSubIdName = default)
    {
        var unitData = GetIdNameDataFromList<UnitData>(unitDataIdName);
        return GetSpeciesTypeUnitData(unitData, speciesIdName, speciesSubIdName);
    }
    public UnitData GetSpeciesBaseUnit(string speciesIdName = default, string speciesSubIdName = default)
    {
        //TODO:从固定人形的加载进行异化
        var unitData = GetIdNameDataFromList<UnitData>(baseUnitDataId);
        return GetSpeciesTypeUnitData(unitData, speciesIdName, speciesSubIdName);
    }
    public UnitData GetSpeciesTypeUnitData(UnitData unitData, string speciesIdName = default, string speciesSubIdName = default)
    {
        if (!string.IsNullOrEmpty(speciesIdName) && unitData.speciesType != speciesIdName || !string.IsNullOrEmpty(speciesSubIdName) && unitData.subSpeciesType != speciesSubIdName)
        {
            string unitDataIdName = unitData.idName;
            if (!string.IsNullOrEmpty(unitData.originIdName))
            {
                unitDataIdName = unitData.originIdName;
            }
            string key = $"{unitDataIdName}_{speciesIdName}_{speciesSubIdName}";
            UnitData newUnitData = GetIdNameDataFromList<UnitData>(key);
            if (newUnitData == null)
            {
                newUnitData = unitData.CloneToOtherSpecies(speciesIdName, speciesSubIdName);
                AddNewIdNameDataToList(key, newUnitData);
            }
            unitData = newUnitData;
        }
        return unitData;
    }

    int flag;
    public string GetSelfCreateIdName()
    {
        flag++;
        return "SelfCreate" + flag.ToString();
    }
}

/// <summary>
/// 模型的偏向于选用
/// </summary>
public enum ModelMotionType
{
    NONE,//无偏向
    MELEE,//单手近战
    POLEARMS,//长柄武器
    TWOHANDED,//双手近战
    RANGE,//远程弓弩
    SPECIAL,//特殊
    RAISED,//唤醒者
}