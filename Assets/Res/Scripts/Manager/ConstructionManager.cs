using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// </summary>
public class ConstructionManager : Singleton<ConstructionManager>, ITimeUpdatable
{
    List<SectorConstruction> constructions = new List<SectorConstruction>();
    List<SectorConstructionUI> uis = new List<SectorConstructionUI>();
    protected override void Init()
    {
        GameManager.timeRelyMethods += OnUpdate;
        base.Init();
    }

    public SectorConstructionUI GetMapUI(string constructionName)
    {
        for (int i = 0; i < uis.Count; i++)
        {
            if (uis[i].sectorConstruction.constructionName == constructionName) return uis[i];
        }
        return null;
    }

    public SectorConstructionUI GetMapUI(SectorConstruction sectorConstruction)
    {
        for (int i = 0; i < uis.Count; i++)
        {
            if (uis[i].sectorConstruction == sectorConstruction) return uis[i];
        }
        return null;
    }

    public SectorConstruction GetClosestConstruction(string idName, Vector2 pos, int belong = 0)
    {
        float minDistance = float.MaxValue;
        float distance = 0f;
        SectorConstruction result = null;
        for (int i = 0; i < constructions.Count; i++)
        {
            if (constructions[i].idName == idName)
            {
                if (belong != 0 && constructions[i].belong != belong) continue;
                distance = Vector2.Distance(constructions[i].position, pos);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    result = constructions[i];
                }
            }
        }
        return result;
    }

    public SectorConstruction GetClosestConstruction(Vector2 pos)
    {
        float minDistance = float.MaxValue;
        float distance = 0f;
        SectorConstruction result = null;
        for (int i = 0; i < constructions.Count; i++)
        {
            distance = Vector2.Distance(constructions[i].position, pos);
            if (distance < minDistance)
            {
                minDistance = distance;
                result = constructions[i];
            }
        }
        return result;
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var construction in constructions)
        {
            construction.ProcessTime(deltaTime);
        }
    }

    public bool CanDeployConstruction(Construction data, Vector2 pos)
    {
        float distance = 0f;
        for (int i = 0; i < constructions.Count; i++)
        {
            distance = Vector2.Distance(pos, constructions[i].position);
            if (distance < (data.size + constructions[i].Size) / 2)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// </summary>
    /// <param name="idName"></param>
    /// <param name="pos"></param>
    public SectorConstruction DeployConstruction(string idName, Vector2 pos, int hideLevel = 0)
    {
        Construction data = DataBaseManager.Instance.GetIdNameDataFromList<Construction>(idName);
        SectorBlock sector = SectorBlockManager.Instance.GetBlock(pos);
        SectorConstruction sectorConstruction = new SectorConstruction(data);
        sectorConstruction.position = pos;
        sectorConstruction.idName = data.idName;
        sector.constructions.Add(sectorConstruction);
        sectorConstruction.sectorBlock = sector;
        constructions.Add(sectorConstruction);
        sectorConstruction.hideLevel = hideLevel;
        var g = GameObject.Instantiate(DataBaseManager.Instance.ResourceLoad<GameObject>("Prefab/BuildingControl"), pos, Quaternion.identity, BuildingParentMark.instance.transform);
        g.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        var ui = g.GetComponent<SectorConstructionUI>();
        ui.mapPos = new Vector2(pos.x, pos.y);
        ui.LoadConstructionImage(data.idName);
        ui.ChangeSize(data.size);
        ui.sectorConstruction = sectorConstruction;
        if (hideLevel > 0)
        {
            ui.OnHide();
            sector.searchAbleWorks.Add(GetBuildSearchWork(idName, sector.recognizeColor));
            sector.AddWork(0.8f, GetBuildSearchWork(idName, sector.recognizeColor));
        }
        uis.Add(ui);
        return sectorConstruction;
    }

    public void DeployConstruction(BuildingDeployData deployData)
    {
        var pos = new Vector2(deployData.posX, deployData.posY);
        SectorBlock sector = SectorBlockManager.Instance.GetBlock(pos);
        var construction = DeployConstruction(deployData.idName, pos, deployData.hideLevel + sector.hideLevel);
        construction.events = deployData.events;
    }


    /// <summary>
    /// </summary>
    public SectorConstruction DeployConstructionSite(string idName, Vector2 pos)
    {
        string constructionSiteName = "ConstructionSite";
        Construction data = DataBaseManager.Instance.GetIdNameDataFromList<Construction>(constructionSiteName);
        if (!CanDeployConstruction(data, pos)) return null;
        SectorBlock sector = SectorBlockManager.Instance.GetBlock(pos);
        SectorConstruction sectorConstruction = new SectorConstruction(data);
        sectorConstruction.position = pos;
        sectorConstruction.idName = data.idName;
        sectorConstruction.workList.Add(GetBuildTransferWork(idName));
        sector.constructions.Add(sectorConstruction);
        sectorConstruction.sectorBlock = sector;
        constructions.Add(sectorConstruction);

        var g = GameObject.Instantiate(DataBaseManager.Instance.ResourceLoad<GameObject>("Prefab/BuildingControl"), pos, Quaternion.identity, BuildingParentMark.instance.transform);
        g.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        var ui = g.GetComponent<SectorConstructionUI>();
        ui.mapPos = new Vector2(pos.x, pos.y);
        ui.LoadConstructionImage(data.idName);
        ui.ChangeSize(data.size);
        ui.sectorConstruction = sectorConstruction;
        uis.Add(ui);
        return sectorConstruction;
    }

    public void ConstractionUIChange(SectorConstruction sectorConstruction, string idName)
    {
        var ui = uis.Find((i) => i.sectorConstruction == sectorConstruction);
        ui.LoadConstructionImage(idName);
    }

    Work GetBuildTransferWork(string idName)
    {
        Work work = DataBaseManager.Instance.GetIdNameDataFromList<Work>("Building").Clone();
        Construction data = DataBaseManager.Instance.GetIdNameDataFromList<Construction>(idName);
        WorkEffect workEffect = new WorkEffect(WorkingEffectType.CONSTRUCTION_TRANSFER.ToString(), idName);
        work.workload = data.workload;
        work.workCost = data.cost;
        work.workCompleteEffect = new WorkEffect[] {
            workEffect
        };
        return work;
    }

    Work GetBuildSearchWork(string idName, Color sectorRegColor)
    {
        Work work = DataBaseManager.Instance.GetIdNameDataFromList<Work>("LandSearch").Clone();
        Construction data = DataBaseManager.Instance.GetIdNameDataFromList<Construction>(idName);
        WorkEffect workEffect = new WorkEffect(WorkingEffectType.LAND_DISCOVER.ToString(), idName, ColorUtility.ToHtmlStringRGB(sectorRegColor));
        work.workCompleteEffect = new WorkEffect[] {
            workEffect
        };
        return work;
    }

    public void DestoryConstruction(SectorConstruction sectorConstruction)
    {
        sectorConstruction.sectorBlock.constructions.Remove(sectorConstruction);
        constructions.Remove(sectorConstruction);
        GameObject.Destroy(GetMapUI(sectorConstruction).gameObject);
    }

    ///// <summary>
    ///// </summary>
    //public void ConstructionAddLegionEquipWork(SectorConstruction sectorConstruction, LegionControl legion, TroopControl troop, int needNum)
    //{
    //    var equipSet = DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>(troop.mainEquip);
    //    Work work = DataBaseManager.Instance.GetIdNameDataFromList<Work>("EquipProduce").Clone();
    //    WorkEffect workEffect = new WorkEffect(WorkingEffectType.EQUIP_PRODUCE.ToString(), troop.idName, needNum.ToString(), legion.Id.ToString());
    //    work.workCost = equipSet.Cost;
    //    work.workCompleteEffect = new WorkEffect[] {
    //        workEffect
    //    };
    //    sectorConstruction.AddNewWork(work);
    //}

    public void OnReset()
    {
        List<SectorConstruction> constructions = new List<SectorConstruction>();
        List<SectorConstructionUI> uis = new List<SectorConstructionUI>();
        for (int i = 0; i < constructions.Count; i++)
        {

        }
        constructions.Clear();
        for (int i = 0; i < uis.Count; i++)
        {
            GameObject.Destroy(uis[i].gameObject);
        }
        uis.Clear();
    }
}
