using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefugeeManager : Singleton<RefugeeManager>
{
    public int refugeeNumber
    {
        get
        {
            return GameManager.instance.playerForce.GetLimitedRes("Refugee");
        }
        set
        {
            GameManager.instance.playerForce.SetLimitedRes("Refugee", value);
        }
    }

    public int ableRefugeeNum
    {
        get
        {
            return GameManager.instance.playerForce.GetLimitedRes("AbleRefugee");
        }
        set
        {
            GameManager.instance.playerForce.SetLimitedRes("AbleRefugee", value);
        }
    }
    public int workingRefugeeNumber => refugeeNumber - ableRefugeeNum;
    public List<Work> activeWorkList = new List<Work>();

    protected override void Init()
    {
        GameManager.timeRelyMethods += OnUpdate;
        GameManager.instance.playerForce.ChangeLimitedResMax("Refugee", int.MaxValue);
        GameManager.instance.playerForce.ChangeLimitedResMax("AbleRefugee", int.MaxValue);
    }

    public void GetRefugee(int number)
    {
        refugeeNumber += number;
        ableRefugeeNum += number;
    }

    public void GiveUpWork(Work work)
    {
        if (activeWorkList.Remove(work))
        {
            ableRefugeeNum += 1;
            work.workEfficiency = 0;
        }
    }

    public void DispatchToWork(Work work)
    {
        activeWorkList.Add(work);
        ableRefugeeNum -= 1;
        work.workEfficiency = 1;
    }

    void OnUpdate(float deltaTime)
    {
        for (int i = activeWorkList.Count - 1; i >= 0; i--)
        {
            if (WorkProcess(activeWorkList[i], deltaTime))
            {
                WorkComplete(activeWorkList[i]);
            }
        }
    }

    void WorkComplete(Work work)
    {
        ableRefugeeNum += 1;
        activeWorkList.Remove(work);
        work.isComplete = true;
    }

    Color blockColor;
    SectorBlock block;
    SectorConstruction construction;
    bool WorkProcess(Work work, float deltaTime)
    {
        float shouldProcess = work.workEfficiency * deltaTime;
        if (work.workProcess < work.workload)
        {
            work.workProcess += shouldProcess;
            if (work.workProcess >= work.workload)
            {
                foreach (var workEffect in work.workCompleteEffect)
                {
                    if (System.Enum.TryParse(workEffect.effectType, out WorkingEffectType type))
                    {
                        switch (type)
                        {
                            case WorkingEffectType.LAND_DISCOVER://资源获取-目标资源类型-数量
                                ColorUtility.TryParseHtmlString("#" + workEffect.effectParams[1], out blockColor);
                                block = SectorBlockManager.Instance.GetBlock(blockColor);
                                construction = block.constructions.Find((s) => s.idName == workEffect.effectParams[0]);
                                construction.hideLevel -= 1;
                                if (construction.hideLevel <= 0)
                                {
                                    ConstructionManager.Instance.GetMapUI(construction)?.OnShow();
                                }
                                block.searchAbleWorks.Remove(work);
                                //工作已完成
                                return true;
                            case WorkingEffectType.LAND_LOOT://资源获取-目标资源类型-数量
                                ColorUtility.TryParseHtmlString("#" + workEffect.effectParams[1], out blockColor);
                                block = SectorBlockManager.Instance.GetBlock(blockColor);
                                GameManager.instance.playerForce.resourcePool.ChangeResource(Constant_Resource.Wood, 1);
                                work.workProcess -= work.workload;
                                break;
                        }
                    }
                }
            }
        }
        return false;
    }

    public void RegistWork(Work work)
    {
        activeWorkList.Add(work);
        ableRefugeeNum -= 1;
    }

    public bool WorkIsWorking(Work work)
    {
        if (activeWorkList.Contains(work))
        {
            return true;
        }
        return false;
    }
}
