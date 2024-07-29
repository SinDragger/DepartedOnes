using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 工作-持续占用劳力 并 产生效果
/// </summary>
public class Work : AggregationEntity
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
    /// 工作类型
    /// </summary>
    public string workType;
    /// <summary>
    /// 运转消耗
    /// </summary>
    public EntityStack[] workCost;
    /// <summary>
    /// 运转所需劳力需求内容
    /// </summary>
    public EntityStack[] workingNeed;
    /// <summary>
    /// 当前Duration的Percent级别
    /// </summary>
    public int needDurationPercent;
    /// <summary>
    /// 工作所需劳力数
    /// </summary>
    public int workNeedNum;
    /// <summary>
    /// 工作量。=0时为持续运转效果。WorkComplete不存在
    /// </summary>
    public int workload;
    /// <summary>
    /// 可被消耗的
    /// </summary>
    public int consumable;
    /// <summary>
    /// 运转时效果
    /// </summary>
    public WorkEffect[] workingEffect;
    /// <summary>
    /// 运转完成效果
    /// </summary>
    public WorkEffect[] workCompleteEffect;
    //——————————当前值————————————
    /// <summary>
    /// 工作进度
    /// </summary>
    public float workProcess;
    /// <summary>
    /// 工作人数
    /// </summary>
    public int workingNum;
    ///// <summary>
    ///// 工作效率
    ///// </summary>
    //public int workingSpeed;
    /// <summary>
    /// 工作者们
    /// </summary>
    public Dictionary<string, int> workingSpeciesDic;

    public float workEfficiency;
    public bool isComplete;

    public string GetMostSuitableSpecies(Dictionary<string, int> workerSpeciesDic)
    {
        //粗略填充，TODO 之后变更
        foreach (var pair in workerSpeciesDic)
        {
            if (pair.Value > 0) return pair.Key;
        }
        return default;
    }

    public Work Clone()
    {
        return this.MemberwiseClone() as Work;
    }
    public void LoadResourceChange(Dictionary<string, int> resourcePool)
    {
        for (int i = 0; i < workCompleteEffect.Length; i++)
        {
            if (workCompleteEffect[i].effectType == WorkingEffectType.RESOURCE_GAIN.ToString())
            {
                string[] array = workCompleteEffect[i].effectParams;
                if (resourcePool.ContainsKey(array[0]))
                {
                    resourcePool[array[0]] += int.Parse(array[1]);
                }
                else
                {
                    resourcePool.Add(array[0], int.Parse(array[1]));
                }
            }
        }
    }
}

public class WorkEffect
{
    /// <summary>
    /// 效果类型
    /// </summary>
    public string effectType;
    /// <summary>
    /// 效果参数
    /// </summary>
    public string[] effectParams;

    public WorkEffect() { }
    public WorkEffect(params string[] paramsString)
    {
        effectType = paramsString[0];
        effectParams = new string[paramsString.Length - 1];
        for (int i = 1; i < paramsString.Length; i++)
        {
            effectParams[i - 1] = paramsString[i];
        }
    }

}
/// <summary>
/// TODO:挪入标准预设Type之中
/// </summary>
public enum WorkingEffectType
{
    /// <summary>
    /// 建筑转化（工地->其他建筑）
    /// </summary>
    CONSTRUCTION_TRANSFER,
    /// <summary>
    /// 资源获取
    /// </summary>
    RESOURCE_GAIN,
    /// <summary>
    /// 资源转化
    /// </summary>
    RESOURCE_PROCESS,
    /// <summary>
    /// 装备生产-装备单位Type-目标量-目标军团id
    /// </summary>
    EQUIP_PRODUCE,
    /// <summary>
    /// 装备补给
    /// </summary>
    CAST_PREPARE,
    /// <summary>
    /// 增援单位
    /// </summary>
    REINFORCE_UNIT,
    /// <summary>
    /// 野兽巢穴升级
    /// </summary>
    BEAST_NEST_LOGIC,
    /// <summary>
    /// 地区搜索
    /// </summary>
    LAND_DISCOVER,
    /// <summary>
    /// 地区搜刮
    /// </summary>
    LAND_LOOT,
}
public enum WorkingType
{
    STRENGTH,//劳力工作
    //术法工作
}