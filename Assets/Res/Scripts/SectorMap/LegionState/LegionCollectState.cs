using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegionCollectState : LegionState
{
    LegionControl nowLegion;
    SectorBlock targetSector;
    float nowTime;
    public LegionCollectState(LegionControl legion, SectorBlock sector)
    {
        nowTime = 0f;
        nowLegion = legion;
        targetSector = sector;
    }
    public override void OnUpdate(float deltaTime)
    {
        nowTime += deltaTime;
        while (nowTime > GameConfig.CollectIntervalTime)
        {
            nowTime -= GameConfig.CollectIntervalTime;
            TriggerSectorCollect();
        }
        //根据当前地形给resourse填充
    }

    public float nowProgressPercent()
    {
        return nowTime / GameConfig.CollectIntervalTime;
    }

    /// <summary>
    /// 触发地形的数据刷新获取
    /// </summary>
    void TriggerSectorCollect()
    {
        var data = SectorBlockManager.Instance.map.GetTargetSectorBlockData(targetSector.recognizeColor);
        if (data.landform == "FOREST")
        {
            nowLegion.resourcePool.ChangeResource(Constant_Resource.Wood, 1);
        }
        else
        {
            nowLegion.resourcePool.ChangeResource(Constant_Resource.Wood, 1);
        }
    }
    public override string GetLegionStateIcon()
    {
        return "LEGION_STATE_OCCUPY";
    }
}
