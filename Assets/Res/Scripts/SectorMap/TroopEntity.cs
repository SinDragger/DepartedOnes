using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 部队的数据通用实例
/// </summary>
public class TroopEntity : AggregationEntity
{
    public UnitData originData;
    public Species species;//覆盖Data的更上层
    public string speciesType;//覆盖Data的更上层
    public string subSpeciesType;//覆盖Data的更上层
    public UnitType unitType;
    public EquipSetEntity weaponEquipSet;
    public EquipSetEntity armourEquipSet;

    /// <summary>
    /// 部队规模上限数量
    /// </summary>
    public int nowNum;
    /// <summary>
    /// 人数缺口
    /// </summary>
    public int shortage => maxNum - nowNum;
    public int maxNum;
    /// <summary>
    /// 生命值
    /// </summary>
    public int maxLife;
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
    //————延伸调整数据————
    /// <summary>
    /// 重量负担
    /// </summary>
    public int weightBurden;
    /// <summary>
    /// 士气基准值
    /// </summary>
    public int moraleBenchmark;
    //public string[] castableSpells;
    //public string[] statusAttachs;
    public TroopEntity() { }
    public TroopEntity(string unitIdName) {
        originData = DataBaseManager.Instance.GetIdNameDataFromList<UnitData>(unitIdName);
        weaponEquipSet = new EquipSetEntity(originData.weaponEquipSetId);
        armourEquipSet = new EquipSetEntity(originData.armourEquipSetId);
        maxLife = originData.maxLife;
        speciesType = originData.speciesType;
        species = DataBaseManager.Instance.GetIdNameDataFromList<Species>(speciesType);
        subSpeciesType = originData.subSpeciesType;
        destructivePower = originData.destructivePower;
        hitRate = originData.hitRate;
        defendLevel = originData.defendLevel;
        weightBearing = originData.weightBearing;
        speed = originData.speed;
        unitType = originData.unitType;
        maxNum = originData.soldierNum;
        moraleBenchmark = originData.morale;
    }
    public TroopEntity(UnitData unitData) {
        originData = unitData;
        weaponEquipSet = new EquipSetEntity(originData.weaponEquipSetId);
        armourEquipSet = new EquipSetEntity(originData.armourEquipSetId);
        maxLife = originData.maxLife;
        speciesType = originData.speciesType;
        species = DataBaseManager.Instance.GetIdNameDataFromList<Species>(speciesType);
        subSpeciesType = originData.subSpeciesType;
        destructivePower = originData.destructivePower;
        hitRate = originData.hitRate;
        defendLevel = originData.defendLevel;
        weightBearing = originData.weightBearing;
        speed = originData.speed;
        unitType = originData.unitType;
        maxNum = originData.soldierNum;
        moraleBenchmark = originData.morale;
    }
    //反向生成UnitData;

    public Dictionary<string, int> GetEquipTotalCost()
    {
        Dictionary<string, int> result = new Dictionary<string, int>();
        var weapon = weaponEquipSet.data;
        var armour = armourEquipSet.data;
        if (weapon.Cost != null)
        {
            for (int i = 0; i < weapon.Cost.Length; i++)
            {
                string idName = weapon.Cost[i].idName;
                if (!result.ContainsKey(idName))
                {
                    result[idName] = weapon.Cost[i].num;
                }
                else
                {
                    result[idName] += weapon.Cost[i].num;
                }
            }
        }
        if (armour.Cost != null)
        {
            for (int i = 0; i < armour.Cost.Length; i++)
            {
                string idName = armour.Cost[i].idName;
                if (!result.ContainsKey(idName))
                {
                    result[idName] = armour.Cost[i].num;
                }
                else
                {
                    result[idName] += armour.Cost[i].num;
                }
            }
        }
        return result;
    }

    public void ChangeWeapon(EquipSetData weaponData)
    {
        weaponEquipSet = new EquipSetEntity(weaponData);
        EventManager.Instance.DispatchEvent(new EventData(GameEventType.OnPlayerConfigEquip, Constant_QuestEventDataKey.PlayerConfigEquip, weaponData));
        QuestManager.Instance.CheckCompleteQuest();
    }
    public void ChangeWeapon(string weaponData)
    {
        weaponEquipSet = new EquipSetEntity(weaponData);
        EventManager.Instance.DispatchEvent(new EventData(GameEventType.OnPlayerConfigEquip, Constant_QuestEventDataKey.PlayerConfigEquip, weaponData));
        QuestManager.Instance.CheckCompleteQuest();
    }

    public void ChangeArmour(EquipSetData armourData)
    {
        armourEquipSet = new EquipSetEntity(armourData);
    }
    public void ChangeArmour(string armourData)
    {
        armourEquipSet = new EquipSetEntity(armourData);
    }
}
