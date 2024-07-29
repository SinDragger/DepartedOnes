using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 势力数据类-持有数据源与可改变配置部分
/// </summary>
public class Force
{
    public ForceData data;
    public Species mainSpecies;
    public List<EquipSetData> weaponSetList = new List<EquipSetData>();
    public List<EquipSetData> armourSetList = new List<EquipSetData>();
    public List<UnitData> unitList = new List<UnitData>();
    public List<UnitData> buyableUnitList = new List<UnitData>();
    public List<Species> speciesList = new List<Species>();
    public List<SubSpecies> subSpeciesList = new List<SubSpecies>();
    public ResourcePool resourcePool = new ResourcePool();

    public Dictionary<EquipSetData, int> equipWeaponStore = new Dictionary<EquipSetData, int>();
    public Dictionary<EquipSetData, int> equipArmourStore = new Dictionary<EquipSetData, int>();
    Dictionary<string, int> limitedResStore = new Dictionary<string, int>();
    Dictionary<string, int> limitedResMax = new Dictionary<string, int>();

    public Force(ForceData data)
    {
        this.data = data;
        if (!string.IsNullOrEmpty(data.mainSpecies))
            mainSpecies = DataBaseManager.Instance.GetIdNameDataFromList<Species>(data.mainSpecies);
        foreach (var value in data.ableSpecies)
        {
            speciesList.Add(DataBaseManager.Instance.GetIdNameDataFromList<Species>(value));
        }
        foreach (var value in data.ableSubSpieces)
        {
            subSpeciesList.Add(DataBaseManager.Instance.GetIdNameDataFromList<SubSpecies>(value));
        }
        foreach (var value in data.ableUnitDatas)
        {
            unitList.Add(DataBaseManager.Instance.GetIdNameDataFromList<UnitData>(value));
        }
        foreach (var value in data.buyableUnitDatas)
        {
            buyableUnitList.Add(DataBaseManager.Instance.GetIdNameDataFromList<UnitData>(value));
        }
        foreach (var value in data.ableWeaponEquipSets)
        {
            weaponSetList.Add(DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>(value));
        }
        foreach (var value in data.ableArmourEquipSets)
        {
            armourSetList.Add(DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>(value));
        }
    }

    public int GetLimitedRes(string idName)
    {
        if (!limitedResStore.ContainsKey(idName))
        {
            return 0;
        }
        else
        {
            return limitedResStore[idName];
        }
    }

    public void ChangeLimitedRes(string idName, int value)
    {
        int max = GetLimitedResMax(idName);
        if (!limitedResStore.ContainsKey(idName))
        {
            if (value > max) value = max;
            limitedResStore[idName] = value;
        }
        else
        {
            if (limitedResStore[idName] + value > max)
            {
                limitedResStore[idName] = max;
            }
            else
            {
                limitedResStore[idName] += value;
            }
        }
        
    }

    public void SetLimitedRes(string idName, int value)
    {
        int max = GetLimitedResMax(idName);
        if (!limitedResStore.ContainsKey(idName))
        {
            if (value > max) value = max;
            limitedResStore[idName] = value;
        }
        else
        {
            if (value > max)
            {
                limitedResStore[idName] = max;
            }
            else
            {
                limitedResStore[idName] = value;
            }
        }
    }

    public int GetLimitedResMax(string idName)
    {
        if (!limitedResMax.ContainsKey(idName)) return 0;
        return limitedResMax[idName];
    }

    public void ChangeLimitedResMax(string idName, int value)
    {
        if (!limitedResMax.ContainsKey(idName)) limitedResMax[idName] = value;
        else limitedResMax[idName] += value;
    }

    List<(string, string)> processList = new List<(string, string)>()
    {
        ("FUR","LEATHER"),
    };
    public void ResourceProcess()
    {
        int number = 10;
        for (int i = 0; i < processList.Count; i++)
        {
            int storeNumber = resourcePool.GetResourceStore(processList[i].Item1);
            if (storeNumber == 0) continue;
            if (storeNumber >= number)
            {
                resourcePool.ChangeResource(processList[i].Item1, -number);
                resourcePool.ChangeResource(processList[i].Item2, number);
                return;
            }
            else
            {
                resourcePool.ChangeResource(processList[i].Item1, -storeNumber);
                resourcePool.ChangeResource(processList[i].Item2, storeNumber);
                number -= storeNumber;
            }
        }
    }
}
