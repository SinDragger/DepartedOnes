using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoSingleton_AutoCreate<SceneManager>
{

    public GameObject targetGameScene;
    public GameObject targetMapScene;
    public GameObject battleResult;
    public Transform cameraControl;
    public Image bgTrans;
    public WarBattle now;
    public bool isNeedPrepareBattle;
    public void TriggerToWarBattle(LegionControl a, LegionControl b)
    {
        if (now != null) return;
        now = new WarBattle(a, b);
        StartCoroutine(WarSceneSwitch(a, b));
    }

    public void TriggerRogueBattleWar(WarBattle warBattle)
    {
        if (now != null) return;
        now = warBattle;
        StartCoroutine(WarScene(warBattle.targetWarBattleMap));
    }

    public void TriggerToWarBattle(WarBattle warBattle)
    {
        if (now != null) return;
        now = warBattle;
        StartCoroutine(WarSceneSwitch(warBattle.combatLegions[0], warBattle.combatLegions[1]));
    }

    public void BackToStartMenu(float time = 3f)
    {
        StartCoroutine(BackToStartMenuCor(time));
    }
    IEnumerator BackToStartMenuCor(float time)
    {
        yield return new WaitForSeconds(time);
        bgTrans.DOFade(1f, 0.1f);
        yield return new WaitForSeconds(0.12f);
        targetGameScene.SetActive(false);
        if (BattleManager.instance)
            BattleManager.instance.ClearBattleField();
        GameManager.instance.BackToStartMenu();
        yield return new WaitForSeconds(0.1f);
        bgTrans.DOFade(0f, 0.1f);
        now = null;
    }
    public void ToSectionBlockMap(int winner, float waitTime = 0.3f)
    {
        //结算战斗胜利获取
        //敌方部队损失——转换按钮
        if (now != null)
        {
            now.winnerBelongTo = winner;
            LegionManager.Instance.WarComplete(now);
            UIManager.Instance.ShowUI("BattleResultPanel", (ui) =>
            {
                (ui as BattleResultPanel).Init(now);
            });

            //TODO：玩家战斗后结果，先计算出资源，再将玩家没有带走的资源放入地块\
        }
        QuestManager.Instance.ShowQuestUI();
        StartCoroutine(MapSceneSwitch(waitTime));
    }

    public void SetupBlackBg()
    {
        bgTrans.color = Color.black;
    }

    public IEnumerator SwitchToBattleScene(string sceneName, System.Action afterLoad)
    {
        bgTrans.DOFade(1f, 0.1f);
        yield return new WaitForSeconds(0.12f);
        targetGameScene.SetActive(true);
        targetMapScene.SetActive(false);
        CameraControl.Instance.SetCameraToBattle();
        CameraControl.Instance.activeLimited = true;
        //TODO 改掉默认开
        QuestManager.Instance.HideQuestUI();
        BattleManager.instance.ShowMiniMap();
        BattleManager.instance.ShowBattleTroopIndicate();
        GridMapManager.instance.ResetData();
        yield return BattleManager.instance.LoadScene(sceneName);
        afterLoad?.Invoke();
        yield return new WaitForSeconds(0.2f);
        bgTrans.DOFade(0f, 0.3f);
    }

    Vector3 positionRecord;
    IEnumerator WarScene(string warBattleMap)
    {
        QuestManager.Instance.HideQuestUI();
        GameManager.instance.SetTimeFrozen(true);
        TimeManager.Instance.SetToStop();
        //yield return new WaitForSeconds(0.1f);
        bgTrans.DOFade(1f, 0.1f);
        //yield return new WaitForSeconds(0.1f);
        targetGameScene.SetActive(true);
        targetMapScene.SetActive(false);
        CameraControl.Instance.SetCameraToBattle();
        CameraControl.Instance.activeLimited = true;
        GridMapManager.instance.ResetData();
        //等待 Game加载完
        yield return BattleManager.instance.TestLoadScene();
        //是否是自动进场的
        if (isNeedPrepareBattle)
        {
            BattleManager.instance.nowBattleMap.targetBattleMapId = warBattleMap;
            yield return PrepareBattleStart(BattleManager.instance.nowBattleMap);
        }
        //异步等待 部队部署
        bgTrans.DOFade(0f, 0.1f);
        BattleManager.instance.nowBattleMap.FinalDeploy();
        yield return 0;
        BattleManager.instance.StartBattle();
        GameManager.instance.SetTimeFrozen(false);
    }
    IEnumerator WarSceneSwitch(LegionControl a, LegionControl b)
    {
        QuestManager.Instance.HideQuestUI();
        GameManager.instance.SetTimeFrozen(true);
        TimeManager.Instance.SetToStop();
        //yield return new WaitForSeconds(0.1f);
        Vector2 middle = (a.position + b.position) / 2;
        positionRecord = new Vector3(middle.x, 0, middle.y);
        bgTrans.DOFade(1f, 0.1f);
        //yield return new WaitForSeconds(0.1f);
        targetGameScene.SetActive(true);
        targetMapScene.SetActive(false);
        CameraControl.Instance.SetCameraToBattle();
        CameraControl.Instance.activeLimited = true;
        GridMapManager.instance.ResetData();
        //等待 Game加载完
        yield return BattleManager.instance.TestLoadScene();
        //是否是自动进场的
        if (now.behaviourChain != null)
        {
            int line = 0;
            float distance = now.nowDistance / 4f + 2f;
            for (int i = 3; i > 0; i--)
            {
                for (int j = 0; j < now.battleTroopsList[i].Count; j++)
                {
                    var battleTroop = now.battleTroopsList[i][j];
                    BattleManager.instance.nowBattleMap.PreLoadDeployData(battleTroop.originTroop, battleTroop.belong, line, distance);
                }
                line++;
                BattleManager.instance.nowBattleMap.ApplyLineDeploy();
            }
            line = 0;
            for (int i = 4; i < 8; i++)
            {
                for (int j = 0; j < now.battleTroopsList[i].Count; j++)
                {
                    var battleTroop = now.battleTroopsList[i][j];
                    BattleManager.instance.nowBattleMap.PreLoadDeployData(battleTroop.originTroop, battleTroop.belong, line, distance);
                }
                line++;
                BattleManager.instance.nowBattleMap.ApplyLineDeploy();
            }
            UnitControlManager.instance.TriggerEnermyAggressive();
            //if (a.mainGeneral != null)
            //{
            //    BattleManager.instance.nowBattleMap.DeployGeneral(a.mainGeneral, a.belong);
            //}
            //if (b.mainGeneral != null)
            //{
            //    BattleManager.instance.nowBattleMap.DeployGeneral(b.mainGeneral, b.belong);
            //}
            //增加敌人主动进攻
        }
        else
        {
            //配置双方的CreateFlag
            for (int i = 0; i < GameConfig.MAX_STACK - 1; i++)
            {
                if (a.stackTroops.ContainsKey(i))
                {
                    foreach (var troop in a.stackTroops[i])
                    {
                        if (troop.nowNum != 0)
                            BattleManager.instance.nowBattleMap.PreLoadDeployData(troop, a.belong, i);
                    }
                }
                BattleManager.instance.nowBattleMap.ApplyLineDeploy();

                if (b.stackTroops.ContainsKey(i))
                {
                    foreach (var troop in b.stackTroops[i])
                    {
                        if (troop.nowNum != 0)
                            BattleManager.instance.nowBattleMap.PreLoadDeployData(troop, b.belong, i);
                    }
                }
                BattleManager.instance.nowBattleMap.ApplyLineDeploy();
            }
            //if (a.mainGeneral != null)
            //{
            //    BattleManager.instance.nowBattleMap.DeployGeneral(a.mainGeneral, a.belong);
            //}
            //if (b.mainGeneral != null)
            //{
            //    BattleManager.instance.nowBattleMap.DeployGeneral(b.mainGeneral, b.belong);
            //}
        }
        if (isNeedPrepareBattle)
        {
            yield return PrepareBattleStart(BattleManager.instance.nowBattleMap);
        }
        //异步等待 部队部署
        bgTrans.DOFade(0f, 0.1f);
        BattleManager.instance.nowBattleMap.FinalDeploy();
        yield return 0;
        BattleManager.instance.StartBattle();
        GameManager.instance.SetTimeFrozen(false);
    }

    bool isCompletePrepare;
    IEnumerator PrepareBattleStart(BattleMap map)
    {
        yield return new WaitForSeconds(0.1f);
        isCompletePrepare = false;
        UIManager.Instance.ShowUI("BattlePreparePanel",(ui)=> {
            (ui as BattlePreparePanel).callback = () => isCompletePrepare = true;
            (ui as BattlePreparePanel).Init(map);
        });
        yield return new WaitUntil(()=>isCompletePrepare);
    }

    IEnumerator MapSceneSwitch(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        bgTrans.DOFade(1f, 0.1f);
        yield return new WaitForSeconds(0.12f);
        BattleManager.instance.ClearBattleField();
        //cameraControl.localPosition = new Vector3(64f, 0f, 64f);
        if (positionRecord != default)
        {
            cameraControl.position = positionRecord;
        }
        targetGameScene.SetActive(false);
        targetMapScene.SetActive(true);
        CameraControl.Instance.activeLimited = false;
        CameraControl.Instance.SetCameraToMap();
        TimeManager.Instance.SetToRecovery();
        System.GC.Collect();
        yield return new WaitForSeconds(0.1f);
        bgTrans.DOFade(0f, 0.1f);
        now = null;
    }
}
