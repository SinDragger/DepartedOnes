using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBuildingDeplay : MonoBehaviour
{
    public string targetBuildingName;
    int i = 0;
    public void TempBuildingDeploy()
    {
        //判断是否有Legion的位置，然后构筑一个建筑
        string target = default;
        if (i % 2 == 0)
        {
            target = "LoggingCamp";
        }
        else
        {
            target = "Stope";
        }
        if (!string.IsNullOrEmpty(targetBuildingName))
        {
            target = targetBuildingName;
        }
        i++;
        Vector2 pos = LegionManager.Instance.NowLegionPosition();
        if (LegionManager.Instance.nowLegion == default)
        {
            pos = SectorBlockManager.Instance.GetNowChooseBlockCenter();
            ConstructionManager.Instance.DeployConstruction(target, pos);
        }
        else
        {
            var sectorConstruction = ConstructionManager.Instance.DeployConstructionSite(target, pos);
            if (sectorConstruction != null)
            {
                LegionManager.Instance.LegionOccupyConstruction(LegionManager.Instance.nowLegion, sectorConstruction);
            }
        }
        //target = "ConstructionSite";
    }
}
