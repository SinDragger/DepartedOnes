using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;

public class LegionManager : MonoSingleton_AutoCreate<LegionManager>
{
    List<LegionControl> legions = new List<LegionControl>(20);
    List<LegionMarkUI> marks = new List<LegionMarkUI>(20);
    List<LegionAIControl> ais = new List<LegionAIControl>(5);
    public LegionControl nowLegion;
    public List<WarBattle> autoWarBattleList = new List<WarBattle>(6);

    /// <summary>
    /// 势力从属图标
    /// </summary>
    Dictionary<int, Sprite> legionSpriteMarks = new Dictionary<int, Sprite>();
    public event System.Action<LegionControl> onLegionElininated;
    protected override void Init()
    {
        base.Init();
        InputManager.Instance.mouseResponseChain.RegistInputResponse(MouseCode.LEFT_DOWN, 1, InputLeftDown);
        InputManager.Instance.mouseResponseChain.RegistInputResponse(MouseCode.RIGHT_UP, 1, InputRightUp);
        GameManager.timeRelyMethods += UpdateByTime;
    }
    int nowId = 0;

    public int RegistLegionId()
    {
        nowId++;
        return nowId;
    }

    public LegionControl GetLegionById(int legionId)
    {
        return legions.Find((legion) => legion.Id == legionId);
    }

    public LegionControl DeployTargetLegionData(LegionDeployData deployData)
    {
        LegionControl newLegion = new LegionControl(deployData);
        RegistLegion(newLegion);
        DeployLegionToTargetPos(newLegion, new Vector2(deployData.posX, deployData.posY));
        return newLegion;
    }

    public LegionControl DeployTargetLegionData(string legionDataId, Vector2 pos, int belong, bool isEmpty = false)
    {
        LegionControl newLegion = new LegionControl(legionDataId, belong, isEmpty);
        newLegion.position = pos;
        RegistLegion(newLegion);
        DeployLegionToTargetPos(newLegion, pos);
        return newLegion;
    }

    public LegionControl SeperateLegion(LegionControl legion, int number)
    {
        LegionControl newLegion = legion.SperateLegion(number);// new LegionControl(legion.idName, legion.belong, true);
        RegistLegion(newLegion);
        DeployLegionToTargetPos(newLegion, legion.position);
        return newLegion;
    }

    public void LegionPackUpConstractionResource(SectorConstruction sectorConstruction)
    {
        foreach (var legion in legions)
        {
            if (legion.State is LegionOccupyState && (legion.State as LegionOccupyState).sectorConstruction.Equals(sectorConstruction))
            {
                (legion.State as LegionOccupyState).PackUpAllResource();
            }
        }
    }
    public Vector2 NowLegionPosition()
    {
        if (nowLegion != null) return nowLegion.position;
        return default;
    }
    public Sprite GetTargetBelongMark(int belong)
    {
        return DataBaseManager.Instance.GetSpriteByIdName($"ForceFlag_{belong}");
    }

    /// <summary>
    /// 军团去巡逻保护目标区域
    /// </summary>
    public void LegionPatrolToDefendBlock(LegionControl legion, SectorBlock block = null)
    {
        if (block == null)
            block = SectorBlockManager.Instance.GetBlock(legion.position);
        Vector2[] posArray = block.ExtracBorderPoints();
        posArray = FormationUtil.BoderPointShrink(posArray, block.gravityPoint, 6f);
        Vector2 legionPos = legion.position;
        float min = float.MaxValue;
        int flag = 0;
        for (int i = 0; i < posArray.Length; i++)
        {
            float distance = Vector2.Distance(legionPos, posArray[i]);
            if (distance < min)
            {
                flag = i;
                min = distance;
            }
        }
        legion.State = new LegionMoveState(legion, posArray[flag], (f) =>
        {
            if (f == 0)
            {
                legion.State = new LegionPatrolState(legion, posArray, flag);
            }
        });
    }
    public LegionControl LegionDeteched(LegionControl legion)
    {
        var target = legions.Find((l) => l.belong != legion.belong && Vector2.Distance(legion.position, l.position) < legion.nowDetectRange);
        return target;
    }
    public void LegionAttackNearBlock(LegionControl legion, SectorBlock block = null)
    {
        var selfblock = SectorBlockManager.Instance.GetBlock(legion.position);
        if (block == null)
        {
            var ableNeighbour = selfblock.GetAttableNeighbour();
            if (ableNeighbour.Count == 0) return;
            block = ableNeighbour[UnityEngine.Random.Range(0, ableNeighbour.Count)];
        }
        Border border = selfblock.FindBorderWith(block);
        legion.State = new LegionMoveState(legion, border.GetCenterPoint(), (f) =>
        {
            if (f == 0)
            {
                legion.State = new LegionMoveState(legion, block.gravityPoint, (distance) =>
                {
                    if (distance == 0)
                    {
                        //建立兽巢
                        var sectorBeastNest = block.constructions.Find((c) => c.idName == "BeastNest");
                        if (sectorBeastNest == null)
                        {
                            sectorBeastNest = ConstructionManager.Instance.DeployConstruction("BeastNest", block.gravityPoint);
                            LegionOccupyConstruction(legion, sectorBeastNest);
                        }
                        else
                        {
                            if (sectorBeastNest.stationedLegions.Count > 0)
                            {
                                LegionMerge(legion, sectorBeastNest.stationedLegions[0]);
                            }
                            else
                            {
                                LegionOccupyConstruction(legion, sectorBeastNest);
                            }
                            //部队融合！
                        }
                    }
                });
            }
        });
    }

    /// <summary>
    /// 军团去巡逻保护目标区域
    /// </summary>
    public void LegionPatrolToDefendBorder(LegionControl legion, SectorBlock block = null, Border border = null)
    {
        if (block == null)
            block = SectorBlockManager.Instance.GetBlock(legion.position);
        if (border == null)
        {
            foreach (var sideBorder in block.borders)
            {
                if (sideBorder.HasDoubleColor())
                {
                    border = sideBorder;
                    //SectorBlockManager.Instance.GetBlock(sideBorder.GetAnotherColor(block.recognizeColor));
                    break;
                }
            }
            //获取一个Block隔壁的
        }
        Vector2[] posArray = border.ExtracBorderPoints();
        posArray = FormationUtil.BoderPointShrink(posArray, block.gravityPoint, 6f);
        Vector2 legionPos = legion.position;
        float min = float.MaxValue;
        int flag = 0;
        for (int i = 0; i < posArray.Length; i++)
        {
            float distance = Vector2.Distance(legionPos, posArray[i]);
            if (distance < min)
            {
                flag = i;
                min = distance;
            }
        }
        legion.State = new LegionMoveState(legion, posArray[flag], (f) =>
        {
            if (f == 0)
            {
                legion.State = new LegionPatrolState(legion, posArray, flag);
            }
        });
    }

    bool InputLeftDown(object param)
    {
        //左键交互
        if (InputManager.Instance.inputMode == InputPointType.UI)
        {
            //获取目标的点击
            var t = InputManager.Instance.NowRaycastFind<ISelectAble>(5, 1);
            if (t != null)
            {
                t.OnUIClick();
                return true;
            }
        }
        return false;
    }
    //右键交互移动
    bool InputRightUp(object param)
    {
        LegionControl useLegion = nowLegion;
        //战斗中的部队无法控制
        if (useLegion == null) return false;
        if (useLegion.State is LegionCombatState) return true;
        //当前交互的为我方的单位
        if (useLegion.belong == GameManager.instance.belong)
        {
            var t = InputManager.Instance.NowRaycastFind<ISelectAble>(5, 2);
            if (t != null)
            {
                if (t is LegionMarkUI)
                {
                    //判断是否点到了敌人头上
                    LegionMarkUI legionOnClick = t as LegionMarkUI;
                    if (legionOnClick && useLegion.belong == GameManager.instance.belong)
                    {
                        if (legionOnClick.legion.belong != GameManager.instance.belong)
                        {
                            useLegion.State = new LegionAttackState(useLegion, legionOnClick.legion);
                            return true;
                        }
                    }
                }
                if (t is SectorConstructionUI)
                {
                    //判断是否点到了建筑物头上
                    SectorConstructionUI constructionOnClick = t as SectorConstructionUI;
                    if (constructionOnClick)
                    {
                        int constructionBelong = constructionOnClick.sectorConstruction.belong;
                        if ((constructionBelong == 0 || constructionBelong == useLegion.belong) && constructionOnClick.sectorConstruction.stationedLegions.Count == 0)
                        {
                            useLegion.State = new LegionMoveState(useLegion, constructionOnClick.mapPos, (distance) =>
                            {
                                if (distance == 0)
                                {
                                    useLegion.State = new LegionOccupyState(useLegion, constructionOnClick.sectorConstruction);
                                    if (UIManager.Instance.IsSelected(GetUI(useLegion)))
                                    {
                                        EventManager.Instance.DispatchEvent(new EventData(MapEventType.CONSTRUCTION_SELECT, "Building", constructionOnClick));
                                        UIManager.Instance.SetNowSelect(constructionOnClick, GetUI(useLegion));
                                    }
                                }
                            }, Color.gray);
                            return true;
                        }
                        else if (constructionOnClick.sectorConstruction.belong != useLegion.belong && constructionOnClick.sectorConstruction.stationedLegions.Count > 0)
                        {
                            //攻击
                            useLegion.State = new LegionAttackState(useLegion, constructionOnClick.sectorConstruction.stationedLegions[0]);
                            return true;
                        }
                        //点到了自己军团驻扎的建筑——无视掉，只进行移动？部队轮换交替——进入后挤压出来
                        return true;
                    }
                }
            }
            if (InputManager.Instance.IsNowRaycastMapLayer())
            {
                UIManager.Instance.SetNowSelect(GetUI(useLegion));
                //在色块地图上寻找目标点位并移动
                Vector3 result = SectorBlockManager.Instance.GetNowPointPosition();
                var block = SectorBlockManager.Instance.GetBlock(result);
                if (block == null || block.hideLevel > 0) return false;
                if (result != default)
                {
                    if (useLegion.State is LegionCombatState) return false;
                    Vector2 targetPosition = new Vector2(result.x, result.z);
                    if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftShift)))
                    {
                        if (useLegion.State is LegionMoveState)
                        {
                            (useLegion.State as LegionMoveState).AddRoute(targetPosition);
                        }
                        else if (useLegion.State is LegionPatrolState)
                        {
                            (useLegion.State as LegionPatrolState).AddRoute(targetPosition);
                        }
                        else
                        {
                            useLegion.State = new LegionMoveState(useLegion, targetPosition, (f) =>
                            {
                                if (f == 0)
                                {
                                    useLegion.State = new LegionWaitState();
                                }
                            }, Color.green);
                        }
                    }
                    else
                    {
                        if (Input.GetKey(KeyCode.LeftControl))
                        {
                            useLegion.State = new LegionPatrolState(useLegion, targetPosition);
                        }
                        else
                        {
                            useLegion.State = new LegionMoveState(useLegion, targetPosition, (f) =>
                            {
                                if (f == 0)
                                {
                                    useLegion.State = new LegionWaitState();
                                }
                            }, Color.green);
                        }
                    }
                }
            }
        }
        return false;
    }

    public void LegionOccupyConstruction(LegionControl legion, SectorConstruction sectorConstruction)
    {
        legion.State = new LegionOccupyState(legion, sectorConstruction);
        var constructionOnClick = ConstructionManager.Instance.GetMapUI(sectorConstruction);
        if (UIManager.Instance.IsSelected(GetUI(legion)))
        {
            EventManager.Instance.DispatchEvent(new EventData(MapEventType.CONSTRUCTION_SELECT, "Building", constructionOnClick));
            UIManager.Instance.SetNowSelect(constructionOnClick, GetUI(legion));
        }
    }

    public void RemoveLegionControl(LegionControl targetLegion)
    {
        var mark = marks.Find((m) => m.legion == targetLegion);
        if (mark)
        {
            mark.gameObject.SetActive(false);//回收
            marks.Remove(mark);
        }
        legions.Remove(targetLegion);
        onLegionElininated?.Invoke(targetLegion);
        SectorBlockManager.Instance.GetBlock(targetLegion.nowSectorReg)?.LegionLeave(targetLegion);
    }

    /// <summary>
    /// 将军团加入到游戏中的序列之中
    /// </summary>
    public void RegistLegion(LegionControl newLegion)
    {
        legions.Add(newLegion);
        //待优化
        newLegion.moveSpeed = 20f;
        //根据belong创建对应LegionAI控制与划分归属
        LegionAIControl legionAI = ais.Find((item) => item.belong == newLegion.belong);
        if (legionAI == null)
        {
            legionAI = new LegionAIControl();
            legionAI.belong = newLegion.belong;
            legionAI.aiLevel = 1;
            ais.Add(legionAI);
        }
        legionAI.AddDescisionLegion(newLegion);
    }

    public void WarBreakOut(LegionControl a, LegionControl b)
    {
        //TODO:如果在Occupy状态时，将Construction的状态设置为War状态的封印——停止所有备战
        //防止生产行为导致的战场增援
        if (a.State is LegionCombatState && b.State is LegionCombatState) return;

        a.LastState = a.State;
        b.LastState = b.State;
        //非歼灭战的时候
        a.State = new LegionCombatState(a);
        b.State = new LegionCombatState(b);
        WarBattle newWar = new WarBattle(a, b);
        if (SceneManager.Instance.now != null) return;//无法开启两个战场
        if ((a.belong == GameManager.instance.belong && a.mainGeneral != null || b.belong == GameManager.instance.belong && b.mainGeneral != null))
        {
            BattleManager.autoBattle = true;
            SceneManager.Instance.isNeedPrepareBattle = true;
            SceneManager.Instance.TriggerToWarBattle(newWar);
        }
        else
        {
            //进行战场初始化
            newWar.InitAutoWarBattle();
            autoWarBattleList.Add(newWar);
            if (a.belong == GameManager.instance.belong || b.belong == GameManager.instance.belong)
                UIManager.Instance.ShowUI("BattleSandTablePanel", (ui) =>
                {
                    (ui as BattleSandTablePanel).Init(newWar);
                });
        }
        var g = Instantiate(DataBaseManager.Instance.ResourceLoad<GameObject>("Prefab/BattleFieldMark"), newWar.centorPos, Quaternion.identity, LegionParentMark.instance.transform);
        var mark = g.GetComponent<BattleFieldMark>();
        mark.Init(newWar);
        mark.SetPosition(newWar.centorPos);
        markList.Add(mark);
        //进入交战状态
        //弹出交战前准备界面
        //通知场景转换进行该场景加载转换
    }
    public List<BattleFieldMark> markList = new List<BattleFieldMark>();




    public void LegionEnterSectorConstruction(LegionControl legion, SectorConstruction construction)
    {
        string targetScence = construction.constructionData.relatedMapId;
        if (targetScence.StartsWith("RougueMode"))
        {
            string id = targetScence.Split('_')[1];
            RogueManager.instance.InitData(legion, id);
        }
        else if (targetScence.StartsWith("Event"))
        {
            Dictionary<string, object> context = new Dictionary<string, object>();
            //System.Action callback = () => LeaveConstruction(legion, construction);
            context["Construction"] = construction;
            context["Legion"] = legion;
            //context["Callback"] = callback;
            if (construction.events != null)
            {
                foreach (var constructionEvent in construction.events)
                {
                    constructionEvent.context = context;
                    constructionEvent.Process();
                }
            }
        }
        else
        {
            StartCoroutine(SceneManager.Instance.SwitchToBattleScene($"ConstructionMode_{targetScence}", () =>
            {
                for (int i = 0; i < GameConfig.MAX_STACK - 1; i++)
                {
                    if (legion.stackTroops.ContainsKey(i))
                    {
                        foreach (var troop in legion.stackTroops[i])
                        {
                            if (troop.nowNum > 0)
                                BattleManager.instance.nowBattleMap.PreLoadDeployData(troop, legion.belong, i);
                        }
                    }
                    BattleManager.instance.nowBattleMap.ApplyLineDeploy();
                }
                if (legion.mainGeneral != null)
                {
                    BattleManager.instance.nowBattleMap.DeployGeneral(legion.mainGeneral, legion.belong);
                }
            }));
        }
    }

    public void WarComplete(WarBattle warBattle)
    {
        autoWarBattleList.Remove(warBattle);
        foreach (var legion in warBattle.combatLegions)
        {
            if (legion.IsEliminate())
            {
                RemoveLegionControl(legion);
            }
            else
            {
                legion.uiDataChanged = true;
                if (legion.LastState is LegionAttackState)
                {
                    legion.State = new LegionDefendState(legion);
                }
                else
                {
                    legion.State = legion.LastState;
                }
                //恢复自己之前State目的
            }
        }
        var mark = markList.Find((m) => m.reflectWar == warBattle);
        if (mark != null)
        {
            markList.Remove(mark);
            Destroy(mark.gameObject);
        }
        QuestManager.Instance.CheckCompleteQuest();
    }

    public void DeployLegionToNowChooseSector(LegionControl newLegion)
    {
        Vector2 targetPos = SectorBlockManager.Instance.GetNowChooseBlockCenter();
        newLegion.position = targetPos;
        var g = Instantiate(DataBaseManager.Instance.ResourceLoad<GameObject>("Prefab/LegionControl"), targetPos, Quaternion.identity, LegionParentMark.instance.transform);
        var ui = g.GetComponent<LegionMarkUI>();
        marks.Add(ui);
        ui.SetPosition(targetPos);
        ui.Init(newLegion);
    }

    public void DeployLegionToTargetPos(LegionControl newLegion, Vector2 targetPos)
    {
        newLegion.position = targetPos;
        var g = Instantiate(DataBaseManager.Instance.ResourceLoad<GameObject>("Prefab/LegionControl"), targetPos, Quaternion.identity, LegionParentMark.instance.transform);
        var ui = g.GetComponent<LegionMarkUI>();
        marks.Add(ui);
        ui.SetPosition(targetPos);
        ui.Init(newLegion);
    }

    public void DeployPlayerLegionToStartPos()
    {
        DeployTargetLegionData("TestPlayerLegion", new Vector2(4, 76), 1);
    }

    private void UpdateByTime(float deltaTime)
    {
        //军团执行当前行为
        UpdateNowLegionBehaviour(deltaTime);

        //刷新排序
        marks.Sort((a, b) => a.transform.localPosition.y.CompareTo(b.transform.localPosition.y));
        for (int i = 0; i < marks.Count; i++)
        {
            marks[i].transform.SetAsFirstSibling();
            //校验所处位置与自身隐匿度是否能显示
        }
        for (int i = 0; i < ais.Count; i++)
        {
            ais[i].OnUpdate(deltaTime);
        }
        for (int i = 0; i < autoWarBattleList.Count; i++)
        {
            autoWarBattleList[i].UpdateAutoResult(deltaTime);
        }
    }


    /// <summary>
    /// 更新当前所有部队的行为
    /// </summary>
    void UpdateNowLegionBehaviour(float deltaTime)
    {
        //更新位置相关数据
        foreach (var legion in legions)
        {
            legion.State.OnUpdate(deltaTime);
            SectorBlock nowBlock = SectorBlockManager.Instance.GetBlock(legion.position);
            if (nowBlock == null) continue;
            if (nowBlock.recognizeColor != legion.nowSectorReg)
            {
                if (legion.nowSectorReg != default)
                {
                    SectorBlockManager.Instance.GetBlock(legion.nowSectorReg).LegionLeave(legion);
                }
                nowBlock.LegionEnter(legion);
                legion.nowSectorReg = nowBlock.recognizeColor;
            }
            ProcessLegionInBlock(legion, nowBlock, deltaTime);
            //更新侦测范围
            legion.nowDetectRange = legion.BaseDetectRange;// * (1 + progress / 100f);
        }
        //是否暴露所有军团
        bool ExposeAllLegion = false;
        //更新逻辑与UI显示
        foreach (var legion in legions)
        {
            if (ExposeAllLegion)
            {
                legion.isExposed = true;
            }
            else
            {
                SectorBlock nowBlock = SectorBlockManager.Instance.GetBlock(legion.nowSectorReg);

                if (legion.belong != GameManager.instance.belong)
                {
                    if (nowBlock.hideLevel <= 0)
                    {
                        if (legion.State is LegionOccupyState)
                        {
                            legion.isExposed = (legion.State as LegionOccupyState).sectorConstruction.hideLevel <= 0;
                        }
                        else
                        {
                            bool isExposedToPlayer = IsLegionExposed(legion);
                            if (isExposedToPlayer)
                            {
                                legion.isExposed = true;
                            }
                            else
                            {
                                //if (nowBlock.playerViewPercent <= 0f)
                                //{
                                //    legion.isExposed = false;
                                //}
                                //else
                                if (nowBlock.playerViewPercent >= 5f)
                                {
                                    legion.isExposed = true;
                                }
                                else
                                {
                                    legion.isExposed = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        legion.isExposed = false;
                    }
                }
            }
            SetUI(legion, (ui) =>
            {
                if (!(legion.State is LegionOccupyState))
                {
                    ui.SetPosition(legion.position);
                }

                ui.SetRemind(0, legion.belong == GameManager.instance.belong ? 0 : legion.nowDetectRange * 2, legion.belong == GameManager.instance.belong ? ColorUtil.selfDetectColor : ColorUtil.enermyDetectColor);
                if (legion.isExposed)
                {
                    ui.OnShow();
                }
                else
                {
                    ui.OnHide();
                }
            });
        }
    }

    void ProcessLegionInBlock(LegionControl legion, SectorBlock block, float deltaTime)
    {
        //交战中的不受理
        if (legion.State is LegionCombatState) return;
        //是否为友方占据的地点
        bool isFriendZone = legion.belong == block.belong;
        //默认都能获取到公共池子
        if (isFriendZone || true)
        {
            if (legion.resourcePool.resourceCarry.Count > 0)
            {
                var force = GameManager.instance.GetForce(legion.belong);
                if (force != null)
                {
                    force.resourcePool.GetResource(legion.resourcePool);
                }
                legion.uiDataChanged = true;
            }
        }
        if (legion.mainGeneral != null)
        {
            if (isFriendZone)
            {
                if (legion.mainGeneral.LifeRecovery(deltaTime * 5))
                {
                    //legion.uiDataChanged = true;
                }
            }
            else
            {
                if (legion.mainGeneral.LifeRecovery(deltaTime))
                {
                    //legion.uiDataChanged = true;
                }
            }
        }
    }

    public List<LegionControl> GetAttackAbleList(LegionControl legion)
    {
        List<LegionControl> result = null;
        for (int i = 0; i < legions.Count; i++)
        {
            if (legions[i].belong != legion.belong && legion.nowDetectRange > Vector2.Distance(legions[i].position, legion.position))
            {
                if (result == null) result = new List<LegionControl>();
                result.Add(legions[i]);
            }
        }
        return result;
    }
    bool IsLegionExposed(LegionControl legion)
    {
        for (int i = 0; i < legions.Count; i++)
        {
            if (legions[i].belong == 1 && legions[i].nowDetectRange > Vector2.Distance(legions[i].position, legion.position))
            {
                return true;
            }
        }
        return false;
    }

    public void SetUI(LegionControl legion, System.Action<LegionMarkUI> action)
    {
        LegionMarkUI ui = marks.Find((m) => m.legion == legion);
        if (ui)
        {
            action?.Invoke(ui);
        }
    }

    public void LegionMerge(LegionControl from, LegionControl to)
    {
        from.MergeTo(to);

        if (from.IsEliminate())
        {
            RemoveLegionControl(from);
        }
    }

    public LegionMarkUI GetUI(LegionControl legion)
    {
        return marks.Find((m) => m.legion == legion);
    }

    /// <summary>
    /// TODO: 查找的idname需要保持唯一
    /// </summary>
    /// <param name="legionName"></param>
    /// <returns></returns>
    public LegionMarkUI GetUI(string legionName)
    {
        return marks.Find((m) => m.legion.idName == legionName);
    }

    public void OnReset()
    {
        for (int i = 0; i < legions.Count; i++)
        {
            //TODO:对已占用资源进行卸载
            //回收结构体
        }
        legions.Clear();
        for (int i = 0; i < marks.Count; i++)
        {
            Destroy(marks[i].gameObject);
        }
        marks.Clear();
        for (int i = 0; i < ais.Count; i++)
        {
        }
        ais.Clear();
        nowLegion = null;
        nowId = 0;
    }

    public void ChooseTargetLegion(LegionControl targetLegion)
    {
        nowLegion = targetLegion;
    }
}
