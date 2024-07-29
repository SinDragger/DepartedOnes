using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏总控管理
/// </summary>
public class GameManager : MonoSingleton<GameManager>
{
    public int belong = 1;
    [HideInInspector]
    public List<Force> forceList = new List<Force>();
    [HideInInspector]
    public Force playerForce;
    [HideInInspector]
    public RoguePlayer playerData;
    public List<UnitData> buyableUnitList => playerForce.buyableUnitList;
    /// <summary>
    /// 资源存储-势力ID，资源名-数量
    /// 可以存储灭亡势力与未登场势力、分裂势力的资源储量
    /// </summary>
    Dictionary<int, Dictionary<string, int>> resourcesStore = new Dictionary<int, Dictionary<string, int>>();
    public bool fullViewTest;
    public static event Action<float> timeRelyMethods;
    public static event Action<float> realTimeRelyMethods;
    public GameObject endPanel;
    [HideInInspector]
    public bool testBattle;
    public Material edgeMat;
    //是否已经暂停
    public bool isPause;
    public bool isAIAttack;
    public bool defaultARPG;

    [HideInInspector]
    public bool troopInvincible;
    [HideInInspector]
    public bool playerInvincible;

    public int nowChallengeFlag;
    public AnimationCurve jumpCurve;
    public AnimationCurve arrowCurve;
    public AnimationCurve moraleCurve;
    public Text tipText;

    public int nowLayerCount;
    public int nowBattleRewardType;

    public int soulPointCount => playerForce.GetLimitedRes(Constant_AttributeString.RES_SOULPOINT);
    public int armyPointCount
    {
        get
        {
            return playerData.armyPoint;
        }
        set
        {
            playerData.armyPoint = value;
        }
    }

    private void Start()
    {
        belong = 1;
        Application.targetFrameRate = 60;
        SortedList<int, string> frontier = new SortedList<int, string>();
        EventManager.Instance.RegistEvent(Constant_Event.LEGION_REMOVE, (e) =>
        {
            CheckWin(e.GetValue<LegionControl>("Legion").belong);
        });

#if UNITY_EDITOR
        InputManager.Instance.mouseResponseChain.RegistInputResponse(MouseCode.MIDDLE_DOWN, 100, (o) =>
        {
            if (InputManager.Instance.IsNowRaycastMapLayer())
            {
                //在色块地图上寻找目标点位并移动
                Vector3 result = SectorBlockManager.Instance.GetNowPointPosition();
                var b = SectorBlockManager.Instance.GetBlock(result);
                if (b != null)
                    Debug.LogError($"{result} {ColorUtility.ToHtmlStringRGB(b.recognizeColor)}");
            }
            return false;
        });
#endif
        int hasShow = PlayerPrefs.GetInt("HasShowTip");
        if (hasShow == 0)
        {
            CoroutineManager.DelayedCoroutine(0.2f, () =>
            {
                //UIManager.Instance.ShowUI("EAUIPanel", groupName: "Main");
                PlayerPrefs.SetInt("HasShowTip", 1);
            });
        }
        playerForce = new Force(DataBaseManager.Instance.GetTargetDataList<ForceData>().Find((f) => f.id == belong));
        forceList.Add(playerForce);
        playerForce.ChangeLimitedResMax(Constant_AttributeString.RES_SOULPOINT, 1000);
    }

    public Force GetForce(int belong)
    {
        return forceList.Find((f) => f.data.id == belong);
    }

    public bool CheckTechAble(string techIdName)
    {
        return playerData.techUnlockList.Contains(techIdName);
    }

    public void SetTechAble(string techIdName)
    {
        playerData.techUnlockList.Add(techIdName);
    }

    public int GetNumber(string resName, int force = -1)
    {
        if (force == -1)
        {
            force = belong;
        }
        if (!resourcesStore.ContainsKey(force)) return 0;
        if (!resourcesStore[force].ContainsKey(resName)) return 0;
        return resourcesStore[force][resName];
    }
    /// <summary>
    /// TODO:修改成势力持有
    /// </summary>
    public void SetNumber(string resName, int value, int force = -1)
    {
        if (force == -1)
        {
            force = belong;
        }
        if (!resourcesStore.ContainsKey(force))
            resourcesStore[force] = new Dictionary<string, int>();
        if (!resourcesStore[force].ContainsKey(resName))
            resourcesStore[force][resName] = value;
        else
            resourcesStore[force][resName] = value;
    }
    /// <summary>
    /// 修改成势力持有
    /// </summary>
    public void ChangeNumber(string resName, int value, int force = -1)
    {
        if (force == -1)
        {
            force = belong;
        }
        if (!resourcesStore.ContainsKey(force))
            resourcesStore[force] = new Dictionary<string, int>();
        if (!resourcesStore[force].ContainsKey(resName))
            resourcesStore[force][resName] = value;
        else
            resourcesStore[force][resName] += value;
    }

    void CheckWin(int belong)
    {
    }

    public void BackToStartMenu()
    {
        UIManager.Instance.ShowUI("GameStartPanel");
    }

    public void EndGame()
    {
        Application.Quit();
    }

    bool timeFrozen;
    /// <summary>
    /// 设置时间点冻结
    /// </summary>
    public void SetTimeFrozen(bool value)
    {
        timeFrozen = value;
    }
    Sequence sequence;
    public void ShowTip(string content)
    {
        if (sequence != null)
        {
            sequence.Kill();
            sequence = null;
        }
        tipText.transform.SetAsLastSibling();
        tipText.text = content;
        tipText.transform.localPosition = new Vector3(0, -50, 0);
        tipText.color = Color.clear;
        tipText.transform.gameObject.SetActive(true);
        sequence = DOTween.Sequence();
        sequence.Append(tipText.transform.DOLocalMoveY(0, 0.3f))
            .Insert(0f, tipText.DOColor(Color.black, 0.2f))
            .AppendInterval(0.4f)
            .Insert(1.3f, tipText.DOColor(Color.clear, 0.15f))
            .Insert(1.3f, tipText.transform.DOLocalMoveY(50, 0.15f));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            //敌人全灭
            BattleManager.instance.CheckBattleWin();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            UIManager.Instance.ShowUI("BattleWinPanel");
            //敌人全灭
            //SectorBlockManager.Instance.UpdateToNextShowLevel();
            //ShowTip("测试一下");
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            UIManager.Instance.ShowUI("BattleFalsePanel");
            //playerForce.resourcePool.ChangeResource(Constant_Resource.FUR, 100);
            //playerForce.resourcePool.ChangeResource(Constant_Resource.Wood, 100);
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            //BattleManager.instance.autoCharge = true;
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            //BattleManager.instance.autoCharge = false;
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            //LegionManager.Instance.nowLegion.ChangeEquip(DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>("Pike_Broken"), 120, true);
            //LegionManager.Instance.nowLegion.ChangeEquip(DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>("AxeSheild_Broken"), 120, true);
            //LegionManager.Instance.nowLegion.ChangeEquip(DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>("Sword_Broken"), 120, true);
            //Time.timeScale = 1f;
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            //playerInvincible = true;
            //你的单位无敌
        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            //GraphicUtil.GetPointDegree(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1));
            //GraphicUtil.GetPointDegree(new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1));
            //GraphicUtil.GetPointDegree(new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 0));
            //GraphicUtil.GetPointDegree(new Vector2(0, 0), new Vector2(1, 0), new Vector2(-1, 0));
            //GraphicUtil.GetPointDegree(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, -1));
            //troopInvincible = true;
            //所有单位无敌
        }
        if (Input.GetKeyDown(KeyCode.F9))
        {
            //playerInvincible = false;
            //troopInvincible = false;
            //取消无敌
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            //边界防守
            //随机Border
            //LegionManager.Instance.LegionPatrolToDefendBorder(LegionManager.Instance.nowLegion);
            //RogueManager.instance.InitData(LegionManager.Instance.nowLegion, "TestRogueScenario");
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.ShowUI("PausePanel");
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            //LegionManager.Instance.LegionPatrolToDefendBlock(LegionManager.Instance.nowLegion);
        }
        realTimeRelyMethods?.Invoke(Time.unscaledDeltaTime);
        if (!timeFrozen)
        {
            TimeManager.Instance.UpdateTime(Time.deltaTime);
            timeRelyMethods?.Invoke(TimeManager.Instance.nowDeltaTime);
        }

    }
    bool hasLoad = false;

    public void LoadFirstMap(bool forceLoad = false)
    {
        if (hasLoad && !forceLoad) return;
        if (!hasLoad)
        {
            hasLoad = true;
            //测试复活
            LegionManager.Instance.onLegionElininated += (l) =>
            {
                if (l.idName == "TestPlayerLegion")
                {
                    UIManager.Instance.ShowUI("ResurrectionPanel");
                }
            };
            PlayerPrefs.DeleteKey("RefugeeShow");
            PlayerPrefs.DeleteKey("LabourShow");
            PlayerPrefs.DeleteKey("DownDetailShow");
        }
        InitManagers();
        SectorBlockManager.Instance.gameObject.SetActive(true);
        CameraControl.Instance.activeLimited = false;
        SectorBlockManager.Instance.LoadTargetMap("Test");
        CreateQuest();
    }

    public void EnterChallengeMode()
    {
        SceneManager.Instance.SetupBlackBg();
        nowChallengeFlag = PlayerPrefs.GetInt("ChallengeNum");
        StartCoroutine(SceneManager.Instance.SwitchToBattleScene($"ChallengeMode_{nowChallengeFlag}", () =>
        {

        }));
    }


    public Color GetForceColor(int belong)
    {
        var force = DataBaseManager.Instance.GetTargetDataList<ForceData>().Find((f) => f.id == belong);
        return ColorUtil.ColorTrans(force.recognizeColor);
    }

    public void NextPlayableNode()
    {
        nowLayerCount += 1;
        if (nowBattleRewardType == 0)
        {
            armyPointCount += 10;
        }
        UIManager.Instance.ShowUI("BattleCompletePanel");
    }

    public void StartGame()
    {
        playerData = new RoguePlayer();
        nowLayerCount = 0;
#if UNITY_EDITOR
        //nowLayerCount = 6;
#endif
        playerData.buyableUnitList = playerForce.buyableUnitList;
        UIManager.Instance.ShowUI("StartChoosePanel");
        return;
        defaultARPG = true;
        SceneManager.Instance.SetupBlackBg();
        PlayerPrefs.SetInt("LoadStart", 0);
        PlayerPrefs.SetInt("NowTutorialState", 0);
        int load = PlayerPrefs.GetInt("LoadStart");
        if (load > 0)
        {
            Debug.LogError(load);
        }
        int nowState = PlayerPrefs.GetInt("NowTutorialState", 0);
        InitManagers();
        StartCoroutine(SceneManager.Instance.SwitchToBattleScene("Chapter_0", () =>
        {
            ARPGManager.Instance.canShowSkillUI = false;
            BattleManager.instance.isNonBattleMap = true;
            BattleManager.instance.miniMap.pointScale = 0.3f;
            BattleManager.instance.miniMap.showScale = 6f;
            BattleManager.instance.CloseMiniMap();
            BattleManager.instance.CloseBattleTroopIndicate();
            General general = new General(DataBaseManager.Instance.GetIdNameDataFromList<GeneralData>("TestGeneral"));
            var tutorialScene = BattleManager.instance.nowBattleMap.GetComponentInChildren<ITutorialScene>();
            var leaveArea = BattleManager.instance.nowBattleMap.GetComponentInChildren<LeaveArea>();
            //部署主角
            if (nowState == 0)
            {
                Vector3 bornPos = tutorialScene.GetPosition("StartTutorialPosition");
                Vector3 faceTo = tutorialScene.GetPosition("StartTutorialFaceTo");
                if (bornPos != default)
                {
                    BattleManager.instance.nowBattleMap.DeployGeneral(general, belong, bornPos, faceTo);
                }
                else
                {
                    BattleManager.instance.nowBattleMap.DeployGeneral(general, belong, 0);
                }
            }
            else
            {
                BattleManager.instance.nowBattleMap.DeployGeneral(general, belong, 0);
            }
            BattleManager.instance.nowBattleMap.FinalDeploy();
            tutorialScene.PlayTutorial();
            leaveArea.nextArea = "TutorialMap";
        }));
    }

    /// <summary>
    /// 进入战斗-层数
    /// </summary>
    /// <param name="layer"></param>
    public void EnterRogueBattle(int layer)
    {
        //TestWarBreakOut("TestPlayerLegion", "CraftsmanCampBattleLegion");
        SceneManager.Instance.now = null;
        WarBattle newWar = new WarBattle();
        if (SceneManager.Instance.now != null) return;//无法开启两个战场
        newWar.targetWarBattleMap = $"Start";
        //newWar.targetWarBattleMap = $"Layer_2_Boss";
        if (!string.IsNullOrEmpty(ChooseNextPanel.nextMapName))
        {
            newWar.targetWarBattleMap = ChooseNextPanel.nextMapName;
            ChooseNextPanel.nextMapName = null;
        }
        SceneManager.Instance.isNeedPrepareBattle = true;
        var data = DataBaseManager.Instance.GetIdNameDataFromList<BattleMapData>(newWar.targetWarBattleMap);
        if (data != null)
            BattleMapTerrainGenerator.targetSeed = data.randomMapSeed;
        SceneManager.Instance.SetupBlackBg();
        SceneManager.Instance.TriggerRogueBattleWar(newWar);
        //TestWarBreakOut(new LegionControl(attackSide, 1), new LegionControl(defendSide, 2));
        //TestWarBreakOut("TestPlayerLegion", "TestEnermyLegion_5_1");
    }


    /// <summary>
    /// 进入战斗
    /// </summary>
    /// <param name="idName"></param>
    public void EnterRogueBattle(string idName)
    {

    }

    public void TestWarBreakOut(string attackSide, string defendSide)
    {
        SceneManager.Instance.now = null;
        TestWarBreakOut(new LegionControl(attackSide, 1), new LegionControl(defendSide, 2));
    }
    public void TestWarBreakOut(LegionControl attackLegion, LegionControl defendLegion)
    {
        WarBattle newWar = new WarBattle(attackLegion, defendLegion);
        if (SceneManager.Instance.now != null) return;//无法开启两个战场
        SceneManager.Instance.isNeedPrepareBattle = true;
        SceneManager.Instance.SetupBlackBg();
        SceneManager.Instance.TriggerToWarBattle(newWar);
    }

    void StartGameTutorialMap()
    {
        SceneManager.Instance.SetupBlackBg();
        defaultARPG = true;
        //BattleManager.instance.isNonBattleMap = true;
        StartCoroutine(SceneManager.Instance.SwitchToBattleScene("TutorialMap"/*Chapter_0*/, () =>
        {
            //TODO:查找场景教程脚本并激活
            //defaultARPG = true;
            ARPGManager.Instance.canShowSkillUI = false;
            BattleManager.instance.isNonBattleMap = true;
            BattleManager.instance.miniMap.pointScale = 0.3f;
            BattleManager.instance.miniMap.showScale = 6f;
            BattleManager.instance.CloseMiniMap();
            BattleManager.instance.CloseBattleTroopIndicate();
            General general = new General(DataBaseManager.Instance.GetIdNameDataFromList<GeneralData>("TestGeneral"));
            var tutorialScene = BattleManager.instance.nowBattleMap.GetComponentInChildren<ITutorialScene>();
            var battelMap = BattleManager.instance.nowBattleMap;
            //部署主角
            Vector3 bornPos = tutorialScene.GetPosition("Point")/*battelMap.baseBornPoint.position*/;
            Vector3 faceTo = /*tutorialScene.GetPosition("StartTutorialFaceTo")*/battelMap.baseBornPoint.rotation.eulerAngles;
            if (bornPos != default)
            {
                BattleManager.instance.nowBattleMap.DeployGeneral(general, belong, bornPos, faceTo);
            }
            else
            {
                BattleManager.instance.nowBattleMap.DeployGeneral(general, belong, 0);
            }
            BattleManager.instance.nowBattleMap.FinalDeploy();
            tutorialScene.PlayTutorial();
        }));
    }

    void InitManagers()
    {
        if (RefugeeManager.Instance != null) ;
        if (LabourWorkManager.Instance != null) ;
    }

    public void LeaveArea(string nextArea = "")
    {
        if (nextArea == "TutorialMap")
        {
            SceneManager.Instance.SetupBlackBg();
            BattleManager.instance.OnBattleEnd();
            BattleManager.instance.ClearBattleField();
            StartGameTutorialMap();
        }
        else
        {
            defaultARPG = false;
            CameraControl.Instance.activeLimited = false;
            SceneManager.Instance.SetupBlackBg();
            BattleManager.instance.OnBattleEnd();
            SceneManager.Instance.ToSectionBlockMap(0, 0f);
            LoadFirstMap();
        }
    }

    public void EnterArea(string areaName)
    {
        defaultARPG = true;
        StartCoroutine(SceneManager.Instance.SwitchToBattleScene(areaName, () =>
        {
            General general = new General(DataBaseManager.Instance.GetIdNameDataFromList<GeneralData>("TestGeneral"));
            BattleManager.instance.nowBattleMap.DeployGeneral(general, GameManager.instance.belong, 0);
        }));
    }

    public void OnReset()
    {
        SectorBlockManager.Instance.OnReset();
        LegionManager.Instance.OnReset();
        ConstructionManager.Instance.OnReset();
    }

    [Button]
    public void CreateQuest()
    {
        playerForce.ChangeLimitedRes(Constant_AttributeString.RES_SOULPOINT, 100);
        QuestManager.Instance.RegisterQuest("RaiseUnit");
    }
}
