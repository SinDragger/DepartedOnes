using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 军团AI管理
/// </summary>
public class LegionAIControl : ITimeUpdatable
{
    public int belong;

    //ai等级级别
    public int aiLevel;

    public LegionAIStatement nowStatement;

    public List<LegionControl> belongLegions = new List<LegionControl>();
    public List<SectorBlock> belongSectors = new List<SectorBlock>();


    public void AddDescisionLegion(LegionControl legion)
    {
        belongLegions.Add(legion);
    }

    /// <summary>
    /// 制定决策
    /// </summary>
    public void DecisionMaking()
    {
        //判断每个legion所属的Sector是否被均匀守备
        //将多出来的legion设定权重值为低
        //判断从属区域是否具备所需的内容

    }

    public void OnUpdate(float deltaTime)
    {
        if (belong != 1)
        {
            for (int i = 0; i < belongLegions.Count; i++)
            {
                LegionControl now = belongLegions[i];
                if (now.State is LegionDefendState)
                {
                    LegionManager.Instance.LegionPatrolToDefendBlock(now);
                }
                else if (now.State is LegionPatrolState)
                {
                    var target = LegionManager.Instance.LegionDeteched(now);
                    if (target != null)
                    {
                        now.State = new LegionAttackState(now, target);
                    }

                }
            }
        }
    }


}

public enum LegionAIStatement
{
    SECTOR_DEFENDER,//区域保护

}