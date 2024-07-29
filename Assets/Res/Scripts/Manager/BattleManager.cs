using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
/// <summary>
/// 游戏内的Manager
/// </summary>
public class BattleManager : MonoSingleton<BattleManager>
{
    /// <summary>
    /// 战场资源存储
    /// </summary>
    Dictionary<int, Dictionary<string, int>> tempResourcesStore = new Dictionary<int, Dictionary<string, int>>();//资源存储

    public List<IBattleManageSingleton> managerList;
    /// <summary>
    /// 效应器
    /// </summary>
    public List<BaseAffectEntity> affectEntities = new List<BaseAffectEntity>();
    public BattleMiniMap miniMap;
    public BattleTroopIndicate battleTroopIndicate;
    public Transform canvasParent;
    public Transform terrianParent;
    public GameObject parent;
    public ChooseWheel buildingChooseWheel;
    InputModule inputModule = new BattleManager_InputModule();
    public ForceData[] forces;
    public Building nowChooseBuilding;
    public int controlBelong;
    public int timeFrame;
    public bool hasInit;
    public bool isActive;

    public event Action mapToBattle;
    public event Action battleToMap;
    public BattleMap nowBattleMap;
    public static bool autoBattle;
    public bool autoCharge;

    public float enermyBattleWill;

    public void UpdateMiniMap()
    {
        miniMap.needUpdateThisFrame = true;
    }

    public void ShowMiniMap()
    {
        miniMap.gameObject.SetActive(true);
    }
    public void CloseMiniMap()
    {
        miniMap.gameObject.SetActive(false);
    }
    public void CloseBattleTroopIndicate()
    {
        battleTroopIndicate.gameObject.SetActive(false);
    }
    public void ShowBattleTroopIndicate()
    {
        battleTroopIndicate.gameObject.gameObject.SetActive(true);
    }
    public void Start()
    {
        Init();
    }
    /// <summary>
    /// 是否当前地图是战斗地图
    /// </summary>
    public bool isNonBattleMap = false;
    public void CheckBattleWin()
    {
        //if (isNonBattleMap) { return; }
        //场面没有
        if (UnitControlManager.instance.IsEnermyAllDie())
        {
            OnGameWin();
        }
        else if (UnitControlManager.instance.IsPlayerAllDie())
        {
            OnGameFail();
        }
    }

    /// <summary>
    /// 部署完毕开始战斗
    /// </summary>
    public void StartBattle()
    {
        UnitControlManager.instance.InitCommanders();
        //触发所有的标准战斗逻辑
    }

    void OnGameWin()
    {
        //if (RogueManager.instance.rogueLegion != null)
        //{
        //    RogueManager.instance.OnRogueBattleWin();
        //    return;
        //}
        //OnBattleEnd();
        EventManager.Instance.DispatchEvent(new EventData(GameEventType.OnPlayerBattleWin.ToString(), Constant_QuestEventDataKey.PlayerWinBattle, SceneManager.Instance.now));
        //显示结算页面
        //显示下一个目的地页面
        GameManager.instance.NextPlayableNode();
        //并且移除目标Flag
    }

    void OnGameFail()
    {
        if (RogueManager.instance.rogueLegion != null)
        {
            RogueManager.instance.OnRogueBattleFail();
            return;
        }
        CoroutineManager.DelayedCoroutine(2f, () =>
        {
            if (UnitControlManager.instance.IsPlayerAllDie())
            {
                OnBattleEnd();
                SceneManager.Instance.BackToStartMenu(0.2f);
                UIManager.Instance.ShowUI("BattleFalsePanel");
            }
        });
        //if (SceneManager.Instance.now != null)
        //{
        //    SceneManager.Instance.ToSectionBlockMap(2);
        //}
        //else
        //{
        //    SceneManager.Instance.BackToStartMenu(0.2f);
        //}
        //并且移除目标Flag
    }

    public void OnBattleEnd()
    {
        isActive = false;
        inputModule.Deactive();
        isNonBattleMap = false;
    }

    /// <summary>
    /// 清理战场UI与特效
    /// </summary>
    public void ClearBattleField()
    {
        if (nowBattleMap == null) return;
        for (int i = 0; i < affectEntities.Count; i++)
        {
            affectEntities[i].EndEffect(true);
        }
        affectEntities.Clear();
        //对象池回收场上东西
        CoroutineManager.TriggerAllCoroutine();
        Destroy(nowBattleMap.gameObject);
        miniMap.OnReset();
        battleTroopIndicate.OnReset();
        nowBattleMap = null;
        for (int i = 0; i < managerList.Count; i++)
        {
            managerList[i].OnDeactive();
        }
        StatusManager.Instance.ClearWorldEntity();
        battleToMap?.Invoke();
    }

    public IEnumerator TestLoadScene()
    {
        Init();
        var p = Resources.Load<GameObject>("TestScene");
        nowBattleMap = Instantiate(p as GameObject, terrianParent).GetComponent<BattleMap>();
        nowBattleMap.RandomTarrian();
        UpdateNavMesh();
        yield return 0;
        Active();
    }

    public IEnumerator LoadScene(string sceneName)
    {
        Init();
        PlayerPrefs.SetInt("LoadStart", 1);
        var p = Resources.Load<GameObject>(sceneName);
        nowBattleMap = Instantiate(p, terrianParent).GetComponent<BattleMap>();
        PlayerPrefs.SetInt("LoadStart", 2);
        UpdateNavMesh();
        if (nowBattleMap.playerBelong != 0)
        {
            controlBelong = nowBattleMap.playerBelong;
        }
        else
        {
            controlBelong = GameManager.instance.belong;
        }
        yield return 0;
        Active();
    }

    [Button]
    public void UpdateNavMesh()
    {
        GetComponent<LocalNavMeshBuilder>().UpdateNavMesh();
    }

    public void Active()
    {
        isActive = true;
        inputModule.Active();
        mapToBattle?.Invoke();
        miniMap.Trigger();
        battleTroopIndicate.OnInit();
        for (int i = 0; i < managerList.Count; i++)
        {
            managerList[i].OnActive();
        }
    }
    /// <summary>
    /// 不知道
    /// </summary>
    public void DeActive()
    {
        isActive = false;
    }

    public void Init()
    {
        if (hasInit) return;
        managerList = new List<IBattleManageSingleton>(GetComponentsInChildren<IBattleManageSingleton>(true));
        //TODO:判断当前的游戏模式并增加内容
        //分离InputModule 用于方便进行可操作性的校验与改变
        inputModule.Init();
        //注册事件监听
        if (GameManager.instance.testBattle)
        {
            return;
        }
        //游戏内的情况
        EventManager.Instance.RegistEvent(Constant_Event.TROOP_REMOVE, (e) =>
        {
            CheckBattleWin();
        });
        //DataBaseManager.Instance.Create();
        managerList.Sort((a, b) => a.priority.CompareTo(b.priority));
        for (int i = 0; i < managerList.Count; i++)
        {
            managerList[i].Init();
        }
        hasInit = true;
    }


    /// <summary>
    /// 变更目标势力的资源储备——可自定义资源存储
    /// </summary>
    public bool ResourceChange(int belong, string resourceName, int value, bool force = false)
    {
        if (!tempResourcesStore.ContainsKey(belong))
        {
            tempResourcesStore[belong] = new Dictionary<string, int>();
        }
        var dic = tempResourcesStore[belong];
        //检查目标belong的resource情况
        if (!dic.ContainsKey(resourceName)) dic[resourceName] = value;
        if (value < 0 && dic[resourceName] - value < 0)
        {
            if (force) dic.Remove(resourceName);
            return false;
        }
        dic[resourceName] += value;
        return true;
    }

    public void RegistAffect(BaseAffectEntity affectEntity)
    {
        affectEntities.Add(affectEntity);
    }

    public BaseAffectEntity GetRelatedEntity(GameObject target, int flag)
    {
        for (int i = 0; i < affectEntities.Count; i++)
        {
            if (affectEntities[i].IsRelatedTo(target, flag))
            {
                return affectEntities[i];
            }
        }
        return null;
    }
    public void EndEntityEffect(BaseAffectEntity entity)
    {
        entity.EndEffect(false);
        entity.isEnd = true;
        affectEntities.Remove(entity);
    }

    float rightCountDown;
    Vector3 prevRightPos;
    void Update()
    {
        if (!isActive) return;
        for (int i = 0; i < managerList.Count; i++)
        {
            managerList[i].OnUpdate();
        }
        for (int i = 0; i < affectEntities.Count; i++)
        {
            affectEntities[i].UpdateAffect(Time.deltaTime);
        }
        //移除标记为End的AffectEntity
        for (int i = affectEntities.Count - 1; i >= 0; i--)
        {
            if (affectEntities[i].isEnd)
            {
                affectEntities.RemoveAt(i);
            }
        }
        miniMap.OnUpdate();
        rightCountDown += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (!isActive) return;
        UnitControlManager.instance?.UpdateTroopCommand(Time.fixedDeltaTime);
        int maxUnitCount = 0;
        int nowUnitCount = 0;
        foreach (var command in UnitControlManager.instance.GetEnermyCommanders())
        {
            maxUnitCount += command.entityData.originData.soldierNum;
            nowUnitCount += command.aliveCount;
        }
        if (maxUnitCount != 0)
        {
            enermyBattleWill = (float)nowUnitCount / (float)maxUnitCount;
        }
    }

    /// <summary>
    /// 跟非UI交互的点击(左键)
    /// </summary>
    /// <param name="clickPoint"></param>
    public void ActiveClick(Vector2 clickPoint, int inputType = 0)
    {

        var ray = Camera.main.ScreenPointToRay(clickPoint);
        if (InputManager.Instance.nowMouseOnWorldCollider == null) return;
        bool isFocus = false;
        if (inputType == 1)
        {
            float distance = Vector3.Distance(prevRightPos, InputManager.Instance.mouseWorldPos);
            if (distance < 5f && rightCountDown < 0.3f)
            {
                isFocus = true;
            }
            rightCountDown = 0f;
            prevRightPos = InputManager.Instance.mouseWorldPos;
        }
        bool hasProcess = false;
        SoldierModel attackTarget = null;
        //判断鼠标点击位置有没有SoldierModel 有就进攻
        GridMapManager.instance.gridMap.GetCircleGridContainTypeWithAction<SoldierModel>(InputManager.Instance.mouseWorldPos, 1f, (soldierModel) =>
        {
            if (!hasProcess)
            {
                switch (inputType)
                {
                    case 0:
                        UnitControlManager.instance.TargetCommondModelClick(soldierModel.commander);
                        if (soldierModel.commander.belong != controlBelong)
                        {
                            UnitControlManager.instance.ResetControl();
                        }
                        hasProcess = true;
                        break;
                    case 1:
                        if (soldierModel.commander.belong != controlBelong)
                        {
                            attackTarget = soldierModel;
                            UnitControlManager.instance.MoveAttackTarget(attackTarget.commander, InputManager.Instance.mouseWorldPos, isFocus);
                            hasProcess = true;
                        }
                        break;
                }
            }

        });

        if (!hasProcess)
        {
            if (inputType != 0)
            {


                UnitControlManager.instance.ChooseToMove(InputManager.Instance.mouseWorldPos, isFocus);
            }
            else
            {
                UnitControlManager.instance.ResetControl();
            }
        }
    }
}
public interface IBattleManageSingleton
{
    /// <summary>
    /// 优先级
    /// </summary>
    int priority { get; }
    /// <summary>
    /// 初始化-可异步
    /// </summary>
    void Init(System.Action callback = null);
    /// <summary>
    /// 帧刷新
    /// </summary>
    void OnUpdate();

    void OnActive();

    void OnDeactive();

}