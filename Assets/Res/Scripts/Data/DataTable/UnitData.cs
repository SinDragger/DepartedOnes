using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 装备数据
/// </summary>
public partial class UnitData : AggregationEntity, IXMLPrintable
{
    /// <summary>
    /// 名称
    /// </summary>
    public string name;

    /// <summary>
    /// 描述
    /// </summary>
    public string des;

    /// <summary>
    /// 部队类型
    /// </summary>
    public UnitType unitType;

    /// <summary>
    /// 单位数量级
    /// </summary>
    public int soldierNum;

    /// <summary>
    /// 武器装备样式
    /// </summary>
    public string weaponEquipSetId;

    /// <summary>
    /// 护甲装备样式
    /// </summary>
    public string armourEquipSetId;

    /// <summary>
    /// 使用的标准兵源生物(模板)
    /// </summary>
    public string speciesType;
    /// <summary>
    /// 调整项内容
    /// </summary>
    public string subSpeciesType;

    public string[] castableSpells;

    public string[] statusAttachs;

    public EntityStack[] resContain;
    public string PrintXML()
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        stringBuilder.Append(CastUtil.OutPutXML(this));
        return stringBuilder.ToString();
    }
    //召唤生物-xxMin后死亡

    public Dictionary<string, int> GetEquipTotalCost()
    {
        Dictionary<string, int> result = new Dictionary<string, int>();
        var weapon = DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>(weaponEquipSetId);
        var armour = DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>(armourEquipSetId);
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

    public Dictionary<string, int> GetTroopTotalCost()
    {
        Dictionary<string, int> result = new Dictionary<string, int>();
        var weapon = DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>(weaponEquipSetId);
        var armour = DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>(armourEquipSetId);
        if (weapon.Cost != null)
        {
            for (int i = 0; i < weapon.Cost.Length; i++)
            {
                string idName = weapon.Cost[i].idName;
                if (!result.ContainsKey(idName))
                {
                    result[idName] = weapon.Cost[i].num * soldierNum;
                }
                else
                {
                    result[idName] += weapon.Cost[i].num * soldierNum;
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
                    result[idName] = armour.Cost[i].num * soldierNum;
                }
                else
                {
                    result[idName] += armour.Cost[i].num * soldierNum;
                }
            }
        }
        return result;
    }
    public Dictionary<string, int> GetTroopTotalBuildCost()
    {
        Dictionary<string, int> result = new Dictionary<string, int>();
        var weapon = DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>(weaponEquipSetId);
        var armour = DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>(armourEquipSetId);
        if (weapon.Cost != null)
        {
            for (int i = 0; i < weapon.Cost.Length; i++)
            {
                string idName = weapon.Cost[i].idName;
                if (!result.ContainsKey(idName))
                {
                    result[idName] = weapon.Cost[i].num * soldierNum;
                }
                else
                {
                    result[idName] += weapon.Cost[i].num * soldierNum;
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
                    result[idName] = armour.Cost[i].num * soldierNum;
                }
                else
                {
                    result[idName] += armour.Cost[i].num * soldierNum;
                }
            }
        }
        if (resContain != null)
        {
            foreach (var pair in resContain)
            {
                result.Add(pair.idName, pair.num * soldierNum);
            }
        }
        return result;
    }

    public void FitWeaponUnitType()
    {
        unitType = UnitType.FIGHTER;
    }
}
public enum UnitType
{
    SUMMONOBJECT,//召唤物
    FIGHTER,//战士
    CASTER,//施法者
    WORKER,//工作者
    MONSTER,//怪物
    RAISED,//被复生者
    EQUIP,//装备
}