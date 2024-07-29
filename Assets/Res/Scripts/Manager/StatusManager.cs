using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : Singleton<StatusManager>
{
    /// <summary>
    /// 势力的状态表
    /// belong-Status
    /// </summary>
    Dictionary<int, List<StandardStatus>> forceTechList = new Dictionary<int, List<StandardStatus>>();

    /// <summary>
    /// 状态存储
    /// </summary>
    List<StandardStatus> statusRestore = new List<StandardStatus>();

    /// <summary>
    /// 有时序需求的实例存储
    /// </summary>
    List<EntityStatus> timeRelyStatus = new List<EntityStatus>();//时序相关的注册

    /// <summary>
    /// 条件判断存储
    /// </summary>
    HashSet<EntityStatus> conditionAbleEntityStatus = new HashSet<EntityStatus>();

    HashSet<EntityStatus> removeEntityStatus = new HashSet<EntityStatus>();
    /// <summary>
    /// 世界/环境效果（持续且持有离散区域的效果）
    /// </summary>
    public List<EntityStatus> worldEffectsEntityStatus = new List<EntityStatus>();
    protected override void Init()
    {
        GameManager.timeRelyMethods += Update;
        //测试能量恢复的硬增加
        var list = DataBaseManager.Instance.GetTargetDataList<StandardStatus>();
        if (list != null)
        {
            statusRestore = list;
        }
        base.Init();
    }

    public void ClearWorldEntity()
    {
        for (int i = 0; i < worldEffectsEntityStatus.Count; i++)
        {
            timeRelyStatus.Remove(worldEffectsEntityStatus[i]);
        }
        worldEffectsEntityStatus.Clear();
    }

    public StandardStatus GetStatus(string idName)
    {
        return statusRestore.Find((s) => s.idName == idName);
    }

    public EntityStatus RequestStatus(string idName, AggregationEntity entity, int belong = -1)
    {
        var status = statusRestore.Find((s) => s.idName == idName);
        return RequestStatus(status, entity, belong);
    }
    public EntityStatus RequestStatus(StandardStatus status, AggregationEntity entity, int belong = -1)
    {
        //从所有状态的存储找到对应的状态
        DataBaseManager.Instance.SetTempData("LastRequestTarget", status);
        //构建实体状态
        var result = new EntityStatus(status, entity);
        result.belong = belong;
        bool isActive = false;
        //判断是否有激活可能的状态
        if (status.ableCondition != null)
        {
            isActive = CheckConditions(result, status.ableCondition);
            //判断是否激活
            if (!isActive)
            {
                conditionAbleEntityStatus.Add(result);
            }
        }
        else
        {
            isActive = true;
        }
        if (isActive)
        {
            if (status.disableCondition != null)
            {
                //初始化 时没有激活就没激活 直接进入判定激活的set中
                isActive = !CheckConditions(result, status.disableCondition);
                if (isActive)
                    conditionAbleEntityStatus.Add(result);
            }
        }
        if (isActive)
        {
            result.nowState = 1;
            //根据StandardStatus的结构情况，确认是否是时间相关的Status
            if (status.IsTimeRelevanceEffect())
            {
                timeRelyStatus.Add(result);
            }
            TriggerEffect(result.dataModel, result.originStatus.activeEffect);
        }
        else
        {
            result.nowState = 0;
        }

        return result;
    }

    /// <summary>
    /// 移除需要知道来源与否
    /// </summary>
    /// <param name="entity"></param>
    public void RemoveStatus(EntityStatus entity)
    {
        removeEntityStatus.Add(entity);
    }

    public bool CheckConditions(EntityStatus entity, StatusCondition[] conditions)
    {
        for (int i = 0; i < conditions.Length; i++)
        {
            if (!conditions[i].CheckCondition(entity))
            {
                return false;
            }
        }
        return true;
    }

    private void Update(float deltaTime)
    {
        LogOffEntityStatusUpdate();
        CheckConditionUpdate();
        TimeRelyStatusUpdate();
    }

    void LogOffEntityStatusUpdate()
    {
        if (removeEntityStatus.Count != 0)
        {
            foreach (var item in removeEntityStatus)
            {
                conditionAbleEntityStatus.Remove(item);
                timeRelyStatus.Remove(item);
            }
            removeEntityStatus.Clear();
        }
    }

    /// <summary>
    /// 处理所有没生效的激活的检测
    /// </summary>
    void CheckConditionUpdate()
    {
        foreach (var entityStatus in conditionAbleEntityStatus)
        {
            bool able = true;
            if (entityStatus.originStatus.ableCondition != null && CheckConditions(entityStatus, entityStatus.originStatus.ableCondition))
            {
                able = true;
            }
            else
            {
                able = false;
            }
            if (able)
            {
                if (entityStatus.originStatus.disableCondition != null && CheckConditions(entityStatus, entityStatus.originStatus.disableCondition))
                {
                    able = false;
                }
            }
            //如果当前状态是激活的 且原始数据中的终止判断不为空且终止判断为True 结束并移除持续运行
            if (entityStatus.nowState == 1 && !able)
            {
                DeTriggerEffect(entityStatus.dataModel, entityStatus.originStatus.activeEffect);
                TriggerEffect(entityStatus.dataModel, entityStatus.originStatus.overEffect);
                timeRelyStatus.Remove(entityStatus);
                entityStatus.nowState = 0;

            }
            else if (entityStatus.nowState == 0 && able)
            {

                TriggerEffect(entityStatus.dataModel, entityStatus.originStatus.activeEffect);
                if (entityStatus.originStatus.IsTimeRelevanceEffect())
                {
                    timeRelyStatus.Add(entityStatus);
                }
                entityStatus.nowState = 1;
            }
        }
    }

    void TimeRelyStatusUpdate()
    {
        if (timeRelyStatus == null) return;
        for (int i = 0; i < timeRelyStatus.Count; i++)
        {
            DataBaseManager.Instance.SetTempData("LastUpdateTarget", timeRelyStatus[i]);
            TriggerEffect(timeRelyStatus[i].dataModel, timeRelyStatus[i].originStatus.continousEffect);
        }
    }


    public void TriggerEffect(AggregationEntity target, params StatusEffectTerm[] effects)
    {
        if (effects == null) return;
        foreach (var effect in effects)
        {
            effect.Execution(target);
        }
    }


    public void DeTriggerEffect(AggregationEntity target, params StatusEffectTerm[] effects)
    {
        if (effects == null) return;
        foreach (var effect in effects)
        {
            effect.ReverseExecution(target);
        }
    }

    //TODO 离开战场清除gb上的状态

}

public class EntityStatus
{
    public StandardStatus originStatus;
    public AggregationEntity dataModel;
    /// <summary>
    /// 0:未激活 1:激活      TODO 抽离成枚举 例如还有激活但等待目标的情况
    /// </summary>
    public int nowState;
    /// <summary>
    /// 堆叠数量
    /// </summary>
    public int heapNum;
    /// <summary>
    /// 已生效的堆叠层数
    /// </summary>
    public int nowActiveHeapNum;
    /// <summary>
    /// 生成时的时间戳
    /// </summary>
    public float timeStamp;
    public int belong;//判断状态来源与所属？敌方/中立/自己

    public EntityStatus(StandardStatus standardStatus, AggregationEntity aggregationEntity)
    {
        originStatus = standardStatus;
        dataModel = aggregationEntity;
        timeStamp = Time.realtimeSinceStartup;
    }


    public void StatusActiveEffectReverseExecution()
    {
        if (originStatus.activeEffect != null && nowState != 0)
        {
            foreach (var item in originStatus.activeEffect)
            {
                //TODO Heap的情况

                item.ReverseExecution(dataModel);
            }
        }
        //TODO 正在执行的
        //TODO 结束的问题

    }
}

