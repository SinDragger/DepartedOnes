using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using UnityEngine;

public class RogueManager : MonoSingleton<RogueManager>
{
    Dictionary<string, int> resourcesStore = new Dictionary<string, int>();
    public int rogueTurn { get; private set; }
    public LegionControl rogueLegion;

    [SerializeField, ChildGameObjectsOnly] private RogueTopDetailUI rogueTopDetailUI;
    [SerializeField, ChildGameObjectsOnly] private RogueMidDetailUI rogueMidDetailUI;

    private RogueData rogueData;
    private RogueNodeData currentNode;
    //进入肉鸽模式的部队
    private LegionControl mapLegion;

    /// <summary>
    /// 预载数据，肉鸽副本，或者是读取存档等数据传递
    /// </summary>
    public void InitData(LegionControl legion, string scenarioId)
    {
        mapLegion = legion;
        rogueData = DataBaseManager.Instance.GetTargetDataList<RogueData>().Find(i => i.scenarioId == scenarioId);
        rogueLegion = new LegionControl(rogueData.rogueLegionId, legion.belong, false);

        resourcesStore[Constant_RogueData.LegionRes_Fatigue] = 0;
        resourcesStore[Constant_RogueData.LegionRes_Supplies] = 0;

        rogueTurn = 0;
        currentNode = rogueData.rogueNodeDatas[0];
        EnterRogueBattle(rogueData.firstBattleScenceId);
    }


    public void OnRogueBattleFail()
    {
        //TODO 失败ui什么什么
        //SceneManager.Instance.BackToStartMenu();
        SceneManager.Instance.ToSectionBlockMap(2);
    }

    public void OnRogueBattleWin()
    { 
        SwitchToNextNode();
        ShowRoguePanelUI();
        BattleManager.instance.ClearBattleField();
    }



    private void ShowRoguePanelUI()
    {
        rogueTopDetailUI.InitByLegionData(rogueLegion);
        rogueTopDetailUI.UpdateResUI(resourcesStore[Constant_RogueData.LegionRes_Fatigue], resourcesStore[Constant_RogueData.LegionRes_Supplies]);
        rogueTopDetailUI.OnShow();

        rogueMidDetailUI.UpdateRogueStory(currentNode);
        rogueMidDetailUI.OnShow();
    }

    private void HideRoguePanelUI()
    {
        rogueTopDetailUI.OnHide();
        rogueMidDetailUI.OnHide();
    }

    private void SwitchToNextNode()
    {
        currentNode = Array.Find(rogueData.rogueNodeDatas, i => (i.nodeName == currentNode.nextNodeName));
    }


    public void ExcuteSelectionBehavior(RogueNodeSelectionData selectionData)
    {
        switch (selectionData.uiBehavior)
        {
            case UIBehavior.HideUI:
                HideRoguePanelUI();
                break;
            case UIBehavior.ToNextNode:
                SwitchToNextNode();
                ShowRoguePanelUI();
                break;
            case UIBehavior.EndEvent:
                if (rogueTurn >= 0)
                {
                    SceneManager.Instance.ToSectionBlockMap(1);
                    HideRoguePanelUI();
                    LegionControl award = new LegionControl(rogueData.rogueLegionId, mapLegion.belong, false);
                    foreach (var troop in award.troops)
                    {
                        mapLegion.AddTroop(troop);
                    }
                    return;
                }
                //从没有前置节点的节点内重roll
                currentNode = Array.FindAll(rogueData.rogueNodeDatas, i => i.preNodeName.IsNullOrWhitespace()).GetRandomElement();

                ShowRoguePanelUI();
                rogueTurn++;
                break;
        }
        foreach (var behavior in selectionData.behaviors)
            behavior.Excute();
    }


    /// <summary>
    /// 行为调用，改变资源
    /// </summary>
    /// <param name="resourceName"></param>
    /// <param name="value"></param>
    public void ResourceChange(string resourceName, int value)
    {
        if (!resourcesStore.ContainsKey(resourceName))
        {
            resourcesStore[resourceName] = value;
        }
        else
        {
            resourcesStore[resourceName] += value;
        }
        rogueTopDetailUI.UpdateResUI(resourcesStore[Constant_RogueData.LegionRes_Fatigue], resourcesStore[Constant_RogueData.LegionRes_Supplies]);
    }

    /// <summary>
    /// 行为调用，开始战斗
    /// </summary>
    /// <param name="scenceId"></param>
    public void EnterRogueBattle(string scenceId)
    {
        SceneManager.Instance.SetupBlackBg();
        StartCoroutine(SceneManager.Instance.SwitchToBattleScene($"RogueMode_{scenceId}", (Action)(() =>
        {
            for (int i = 0; i < GameConfig.MAX_STACK - 1; i++)
            {
                if (rogueLegion.stackTroops.ContainsKey(i))
                {
                    foreach (var troop in rogueLegion.stackTroops[i])
                    {
                        if (troop.nowNum > 0)
                            BattleManager.instance.nowBattleMap.PreLoadDeployData(troop, rogueLegion.belong, i);
                    }
                }
                BattleManager.instance.nowBattleMap.ApplyLineDeploy();
            }
            //if (rogueLegion.mainGeneral != null)
            //{
            //    BattleManager.instance.nowBattleMap.DeployGeneral(rogueLegion.mainGeneral, rogueLegion.belong);
            //}
        })));
    }
}
