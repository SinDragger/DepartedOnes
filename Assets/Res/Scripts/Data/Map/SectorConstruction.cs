using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 区块之上的建筑-中间层，用于提供区块特别的内容
/// </summary>
public class SectorConstruction : AggregationEntity
{
    /// <summary>
    /// 从属
    /// </summary>
    public int belong
    {
        get
        {
            if (stationedLegions.Count == 0) return sectorBlock.belong;
            return stationedLegions[0].belong;
        }
    }

    /// <summary>
    /// 构造数据
    /// </summary>
    public Construction constructionData;
    public SectorBlock sectorBlock;
    public List<Work> workList = new List<Work>();
    public Vector2 position;
    public int hideLevel;
    public string constructionName;
    public EventTriggerData[] events;

    public float Size => constructionData.size;
    public int ResourceSpace => constructionData.storageSpace;
    public int ResourceNum
    {
        get
        {
            int value = 0;
            foreach (var resource in resourcesStore)
            {
                value += resource.Value;
            }
            return value;
        }

    }
    public Dictionary<string, int> resourcesStore = new Dictionary<string, int>();
    /// <summary>
    /// 工作者生物映射表
    /// </summary>
    Dictionary<string, int> workerSpeciesDic = new Dictionary<string, int>();

    //TODO:进行
    public int workingNum;
    public int workerNum
    {
        get
        {
            int value = 0;
            foreach (var pair in workerSpeciesDic)
            {
                value += pair.Value;
            }
            return value;
        }
    }

    /// <summary>
    /// 所驻扎的军团
    /// </summary>
    public List<LegionControl> stationedLegions = new List<LegionControl>();
    public SectorConstruction(Construction data)
    {
        constructionData = data;
        if (data.workStation != null)
        {
            foreach (var a in data.workStation)
            {
                for (int i = 0; i < a.num; i++)
                {
                    workList.Add(DataBaseManager.Instance.GetIdNameDataFromList<Work>(a.idName).Clone());
                }
            }
        }
        constructionName = data.name;
        //工作内容的导入
        //向管理器注册管理？
    }


    /// <summary>
    /// 被军团驻扎
    /// </summary>
    public void BeStatetioned(LegionControl legion)
    {
        EventManager.Instance.DispatchEvent(new EventData(GameEventType.OnConstructionBeStatetioned, 
            Constant_QuestEventDataKey.OnConstructionBeStatetioned_Con, constructionData,
            Constant_QuestEventDataKey.OnConstructionBeStatetioned_Legion, legion));
        QuestManager.Instance.CheckCompleteQuest();
        stationedLegions.Add(legion);
        if (legion.belong > 0)
        {
            string relatedevent = constructionData.relatedEvent;
            if (!string.IsNullOrEmpty(relatedevent) && relatedevent.StartsWith("OnceEvent"))
            {
                Dictionary<string, object> context = new Dictionary<string, object>();
                System.Action callback = () => DestroyConstruction(legion);
                context["Construction"] = this;
                context["Legion"] = legion;
                context["Callback"] = callback;
                if (events != null)
                {
                    foreach (var constructionEvent in events)
                    {
                        constructionEvent.context = context;
                        constructionEvent.Process();
                    }
                }
            }
            else if (!string.IsNullOrEmpty(relatedevent) && relatedevent.StartsWith("Quest"))
            {
                QuestManager.Instance.RegisterQuest(relatedevent.Split('_')[1]);
            }
        }
    }
    void DestroyConstruction(LegionControl legion)
    {
        legion.State = new LegionWaitState();
        ConstructionManager.Instance.DestoryConstruction(this);
        LegionManager.Instance.ChooseTargetLegion(legion);
        EventManager.Instance.DispatchEvent(new EventData(MapEventType.LEGION_SELECT, "Legion", legion));
        UIManager.Instance.SetNowSelect(LegionManager.Instance.GetUI(legion));
    }
    /// <summary>
    /// 被军团离开
    /// </summary>
    /// <param name="legion"></param>
    public void BeLeaved(LegionControl legion)
    {
        stationedLegions.Remove(legion);
        //foreach (var troop in legion.troops)
        //{
        //    if (UnitType.WORKER.ToString() == troop.unitType)
        //    {
        //        workerSpeciesDic.DictionaryRemove(troop.speciesDic);
        //    }
        //}
        //DispatchWorkerToWork();
    }

    void DispatchWorkerToWork()
    {
        return;
        workingNum = 0;
        //workingSpeciesDic.Clear();
        //创建待分配拷贝
        Dictionary<string, int> waitDispatchSpeciesList = workerSpeciesDic.Clone();
        int temp = 0;
        //尝试满足每个工作
        foreach (var work in workList)
        {
            //work.workingNeed 需求尝试
            int needNum = work.workNeedNum;
            if (needNum == 0)
            {
                continue;
            }
            work.workingNum = 0;
            if (work.workingSpeciesDic == null) work.workingSpeciesDic = new Dictionary<string, int>();
            work.workingSpeciesDic.Clear();
            work.workEfficiency = 0f;
            //TODO:获取当前作业的最佳工种
            //根据算法填充入作业的，效率受影响变更
            //排序排序
            needNum = needNum - work.workingNum;
            while (needNum > 0)
            {
                string targetSpecies = work.GetMostSuitableSpecies(waitDispatchSpeciesList);
                if (string.IsNullOrEmpty(targetSpecies)) break;
                if (waitDispatchSpeciesList[targetSpecies] >= needNum)
                {
                    waitDispatchSpeciesList[targetSpecies] -= needNum;
                    work.workingSpeciesDic.Add(targetSpecies, needNum);
                    work.workingNum += needNum;
                    work.workEfficiency += ((float)needNum / (float)work.workNeedNum) * 1f;//TODO:100%工作效率变更至相应数量
                    workingNum += needNum;
                    needNum = 0;
                }
                else
                {
                    temp = waitDispatchSpeciesList[targetSpecies];
                    needNum -= temp;
                    work.workingSpeciesDic.Add(targetSpecies, temp);
                    work.workingNum += temp;
                    work.workEfficiency += ((float)temp / (float)work.workNeedNum) * 1f;
                    workingNum += temp;
                    waitDispatchSpeciesList[targetSpecies] = 0;
                }
            }
        }

    }

    /// <summary>
    /// 处理工作流转
    /// </summary>
    public void ProcessTime(float deltaTime)
    {
        //工作列表内的每项工作都进行运转
        foreach (var work in workList)
        {
            //计算
            if (work.workProcess < work.workload)
            {
                WorkProcess(work, deltaTime);
            }
            if (work.workload > 0 && work.workProcess >= work.workload)
            {
                if (ProcessWorkEffect(work, work.workCompleteEffect))
                {
                    break;
                }
            }
        }
        if (needRemoveWorkList != null && needRemoveWorkList.Count > 0)
        {
            workList.RemoveAll((w) => needRemoveWorkList.Contains(w));
            needRemoveWorkList.Clear();
            DispatchWorkerToWork();
        }
        //触发对应的工作效果
    }

    public void AddNewWork(Work work)
    {
        workList.Add(work);
        DispatchWorkerToWork();
    }

    void WorkProcess(Work work, float deltaTime)
    {
        float shouldProcess = work.workEfficiency * deltaTime;
        //校验每项资源储备的最大Process程度
        if (work.workCost != null && work.workCost.Length > 0)
        {
            float maxAbleProcessPercent = 1f;
            for (int i = 0; i < work.workCost.Length; i++)
            {
                //已经消耗的量
                int resNum = Mathf.CeilToInt(work.workCost[i].num * work.workProcess / work.workload);
                //预存储量
                resNum += GetResourceStore(work.workCost[i].idName);
                float resAbleProcessPercent = (float)resNum / (float)work.workCost[i].num;
                if (resAbleProcessPercent < maxAbleProcessPercent)
                {
                    maxAbleProcessPercent = resAbleProcessPercent;
                }
            }
            if (Mathf.Approximately(work.workProcess / work.workload, maxAbleProcessPercent))//不够
            {
                return;
            }
            else
            {
                if ((work.workProcess + shouldProcess) / work.workload > maxAbleProcessPercent)
                {
                    shouldProcess = maxAbleProcessPercent * work.workload - work.workProcess;
                }
            }

            for (int i = 0; i < work.workCost.Length; i++)
            {
                //已经消耗的量
                int resNum = Mathf.CeilToInt(work.workCost[i].num * work.workProcess / work.workload);
                int targetNum = Mathf.CeilToInt(work.workCost[i].num * (work.workProcess + shouldProcess) / work.workload);
                //预存储量
                ResourceConsume(work.workCost[i].idName, targetNum - resNum);
            }
            //存在这项工作的消耗
        }
        work.workProcess += shouldProcess;
    }

    bool ResourceConsume(string idName, int num)
    {
        if (num == 0) return true;
        //优先消耗已有的
        if (resourcesStore.ContainsKey(idName))
        {
            int store = resourcesStore[idName];
            if (store > num)
            {
                resourcesStore[idName] -= num;
                return true;
            }
            else
            {
                num -= resourcesStore[idName];
                resourcesStore.Remove(idName);
            }
        }
        if (num == 0) return true;

        if (stationedLegions.Count != 0)
        {
            for (int i = 0; i < stationedLegions.Count; i++)
            {
                num = stationedLegions[i].resourcePool.ResourceConsume(idName, num);
                if (num == 0) return true;
            }
        }
        //Debug.LogError("Error:CannotUseOut");
        return false;
    }

    int GetResourceStore(string idName)
    {
        int result = 0;
        if (resourcesStore.ContainsKey(idName))
        {
            result += resourcesStore[idName];
        }
        if (stationedLegions.Count != 0)
        {
            for (int i = 0; i < stationedLegions.Count; i++)
            {
                result += stationedLegions[i].resourcePool.GetResourceStore(idName);
            }
        }
        return result;
    }

    List<Work> needRemoveWorkList = null;
    /// <summary>
    /// TODO:拆解内容
    /// </summary>
    /// <param name="workEffects"></param>
    bool ProcessWorkEffect(Work work, params WorkEffect[] workEffects)
    {
        foreach (var workEffect in workEffects)
        {
            if (System.Enum.TryParse(workEffect.effectType, out WorkingEffectType type))
            {
                var force = GameManager.instance.GetForce(belong);
                switch (type)
                {
                    case WorkingEffectType.RESOURCE_GAIN://资源获取-目标资源类型-数量
                        //如果地区控制度够直接转入势力库存
                        if (SectorBlockManager.Instance.GetBlock(position).GetProgress(belong) > 20f)
                        {
                            if (force != null)
                            {
                                force.resourcePool.ChangeResource(workEffect.effectParams);
                                work.workProcess -= work.workload;
                            }
                            break;
                        }
                        int ableNum = ResourceSpace - ResourceNum;
                        ableNum = Mathf.Clamp(int.Parse(workEffect.effectParams[1]), 0, ableNum);
                        if (ableNum > 0)
                        {
                            if (resourcesStore.ContainsKey(workEffect.effectParams[0]))
                            {
                                resourcesStore[workEffect.effectParams[0]] += ableNum;
                            }
                            else
                            {
                                resourcesStore[workEffect.effectParams[0]] = ableNum;
                            }
                            work.workProcess -= work.workload;
                        }
                        break;
                    case WorkingEffectType.RESOURCE_PROCESS://资源获取-目标资源类型-数量
                        if (force != null)
                        {
                            force.ResourceProcess();
                            //force.resourcePool.ChangeResource(workEffect.effectParams);
                            work.workProcess -= work.workload;
                        }
                        break;
                    case WorkingEffectType.CONSTRUCTION_TRANSFER://建筑转化-目标建筑ID
                        ConstructionTransfer(workEffect.effectParams[0]);
                        return true;
                    case WorkingEffectType.EQUIP_PRODUCE://装备生产-装备单位Type-目标量-目标军团id
                        //向目标军团进行填充补足
                        int targetNum = int.Parse(workEffect.effectParams[1]);
                        targetNum -= 1;
                        stationedLegions[0].ReinforceUnits(workEffect.effectParams[0], Constant_AttributeString.SPECIES_EQUIP, 1);
                        if (targetNum <= 0)
                        {
                            var targetLegion = LegionManager.Instance.GetLegionById(int.Parse(workEffect.effectParams[2]));
                            //发起运输
                            //Debug.LogError("Complete");
                            if (needRemoveWorkList == null) needRemoveWorkList = new List<Work>();
                            needRemoveWorkList.Add(work);
                        }
                        else
                        {
                            workEffect.effectParams[1] = targetNum.ToString();
                            //目标量减少
                            //当前驻守军队获得储备
                        }
                        work.workProcess -= work.workload;
                        break;
                    case WorkingEffectType.REINFORCE_UNIT://补充Unit
                        //向目标军团进行填充补足
                        if (stationedLegions.Count > 0)
                        {

                        }
                        else
                        {
                            var newLegion = LegionManager.Instance.DeployTargetLegionData(workEffect.effectParams[0], position, belong == -1 ? 0 : belong, true);
                            LegionManager.Instance.LegionOccupyConstruction(newLegion, this);
                            //创建临时的军团
                        }
                        stationedLegions[0].ReinforceUnits(workEffect.effectParams[1], int.Parse(workEffect.effectParams[2]));
                        //判断是否是构筑一支分离队伍去进行其他事务
                        work.workProcess -= work.workload;
                        break;
                    case WorkingEffectType.BEAST_NEST_LOGIC://分离巡逻队-巡逻队召回与巡逻队补充

                        //向目标军团进行填充补足
                        //主队的繁殖
                        if (stationedLegions.Count > 0)
                        {
                            if (stationedLegions[0].TotalNum < 200)
                            {
                                stationedLegions[0].ReinforceUnits(workEffect.effectParams[2], int.Parse(workEffect.effectParams[3]));
                                work.workProcess -= work.workload;
                            }
                            int nowLevel = GetIntValue("NestLevel");
                            //主队分离
                            int needSeperateNumber = GetIntValue("NeedSeperateNumber");
                            //int needSeperateNumber = 99;
                            if (nowLevel == 0)
                            {
                                SetIntValue("NestLevel", ++nowLevel);
                                needSeperateNumber += 3;
                                //SetIntValue("NeedSeperateNumber", ++needSeperateNumber);
                                SetIntValue("NeedSeperateNumber", needSeperateNumber);
                                work.workEfficiency = 1f + nowLevel * 0.5f;
                                //生产效果增幅
                            }
                            //不准繁殖
                            //else if (stationedLegions[0].TotalNum > int.Parse(workEffect.effectParams[workEffect.effectParams.Length - 1]) + 100)
                            //{
                            //    var selfblock = SectorBlockManager.Instance.GetBlock(position);
                            //    if (selfblock.GetAttableNeighbour().Count != 0)
                            //    {
                            //        var attackTroop = LegionManager.Instance.SeperateLegion(stationedLegions[0], stationedLegions[0].TotalNum / 2);
                            //        LegionManager.Instance.LegionAttackNearBlock(attackTroop);
                            //    }
                            //}
                            if (stationedLegions[0].TotalNum > 100 && needSeperateNumber > 0)
                            {
                                int number = Random.Range(10, 40);
                                //进行主队的分离与巡逻
                                var newPatrol = LegionManager.Instance.SeperateLegion(stationedLegions[0], number);
                                SetIntValue("NeedSeperateNumber", --needSeperateNumber);
                                LegionManager.Instance.LegionPatrolToDefendBlock(newPatrol);
                                System.Action<LegionControl> ifBeenElinated = default;
                                ifBeenElinated = (l) =>
                                {
                                    //当巡逻队被干掉时 再加一队巡逻队
                                    if (l == newPatrol)
                                    {
                                        SetIntValue("NeedSeperateNumber", GetIntValue("NeedSeperateNumber") + 1);
                                        LegionManager.Instance.onLegionElininated -= ifBeenElinated;
                                    }
                                };
                                LegionManager.Instance.onLegionElininated += ifBeenElinated;
                            }
                        }
                        break;
                }
            }
        }
        return false;
    }

    public void ConstructionTransfer(string idName)
    {
        workList.Clear();
        Construction data = DataBaseManager.Instance.GetIdNameDataFromList<Construction>(idName);
        constructionData = data;
        if (data.workStation != null)
        {
            foreach (var a in data.workStation)
            {
                for (int i = 0; i < a.num; i++)
                {
                    workList.Add(DataBaseManager.Instance.GetIdNameDataFromList<Work>(a.idName).Clone());
                }
            }
        }
        idName = data.idName;
        constructionName = data.name;
        DispatchWorkerToWork();
        ConstructionManager.Instance.ConstractionUIChange(this, idName);
    }

}