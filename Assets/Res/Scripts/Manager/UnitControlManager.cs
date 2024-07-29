using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class UnitControlManager : MonoSingleton<UnitControlManager>, IBattleManageSingleton
{
    /// <summary>
    /// 当前控制框选的部队序列
    /// </summary>
    public HashSet<int> controls = new HashSet<int>();
    List<ForceData> forces;
    List<Commander> commanders = new List<Commander>();
    List<CommandUnit> troopCommanders = new List<CommandUnit>();

    List<HashSet<int>> controlsFormation = new List<HashSet<int>>();
    public HashSet<int> controlsFormation1 = new HashSet<int>();
    public HashSet<int> controlsFormation2 = new HashSet<int>();
    public HashSet<int> controlsFormation3 = new HashSet<int>();
    public HashSet<int> controlsFormation4 = new HashSet<int>();
    /// <summary>
    /// TODO:设置成可操作
    /// </summary>
    public List<ControlFlag> controlFlagList = new List<ControlFlag>();

    public float minblockDistance = 0.1f;

    public int priority => 10;

    public Vector3 rightMouseCenter = default;

    Vector3 faceTo;

    public List<GameObject> guidesList = new List<GameObject>();

    public Dictionary<CommandUnit, Vector3[]> desTargetDic = new Dictionary<CommandUnit, Vector3[]>();

    public Dictionary<CommandUnit, int> commandWidthDic = new Dictionary<CommandUnit, int>();

    /// <summary>
    /// 已激活时的模块
    /// </summary>
    public CommandFormation_InputModule formationInputModule = new CommandFormation_InputModule();
    public FrameSelect_InputModule frameSelectInputModule = new FrameSelect_InputModule();
    public event System.Action onNewCommandAdd;

    Ray ray;
    RaycastHit m_HitInfo;

    public void Init(System.Action callback = null)
    {
        instance = this;
        //controlBelong = GameManager.instance.belong;//TODO：需要变成分离的加载
        //注册单位控制的逻辑
        CameraControl.Instance.forceSynAction += () =>
        {
            foreach (var model in troopCommanders)
            {
                model.ChangeFaceTo(-CameraControl.Instance.mainCamera.transform.forward);
            }
        };
        formationInputModule.Init();
        frameSelectInputModule.Init();
    }

    ControlFlag hideFlag;

    public void HideControlUI(CommandUnit targetCommand)
    {
        hideFlag = controlFlagList.Find((c) => c.commander == targetCommand);
        if (hideFlag)
            hideFlag.gameObject.SetActive(false);
    }

    public List<CommandUnit> GetEnermyCommanders()
    {
        List<CommandUnit> result = new List<CommandUnit>();
        foreach (var command in troopCommanders)
        {
            if (command.belong != BattleManager.instance.controlBelong)
            {
                result.Add(command);
            }
        }
        return result;
    }

    public List<CommandUnit> GetAllianceCommanders()
    {
        List<CommandUnit> result = new List<CommandUnit>();
        foreach (var command in troopCommanders)
        {
            if (command.belong == BattleManager.instance.controlBelong)
            {
                result.Add(command);
            }
        }
        return result;
    }




    public int GetAliveEnermyNumber()
    {
        int count = 0;
        foreach (var command in troopCommanders)
        {
            if (command.belong != BattleManager.instance.controlBelong && command.troopState != TroopState.DESTROYED)
            {
                count += command.aliveCount;
            }
        }
        return count;
    }

    public int GetAliveAllianceNumber()
    {
        int count = 0;
        foreach (var command in troopCommanders)
        {
            if (command.belong == BattleManager.instance.controlBelong && command.troopState != TroopState.DESTROYED)
            {
                count += command.aliveCount;
            }
        }
        return count;
    }

    public void TriggerEnermyAggressive()
    {
        foreach (var c in troopCommanders)
        {
            if (c.belong != BattleManager.instance.controlBelong)
            {
                c.TriggerCommandToAggressive();
            }
        }
    }

    public void ShowControlUI()
    {
        if (hideFlag)
            hideFlag.gameObject.SetActive(true);
    }
    /// <summary>
    /// 当前使用的编队组
    /// </summary>
    int nowFormationFlag = -1;
    public void SetFormation(int formationNumber)
    {
        if (nowFormationFlag == formationNumber) return;
        nowFormationFlag = formationNumber;
        if (controlsFormation.Count < 4)
        {
            for (int i = 0; i < 4; i++)
            {
                controlsFormation.Add(new HashSet<int>());
            }
        }
        controlsFormation[formationNumber].Clear();
        controlsFormation[formationNumber].UnionWith(controls);
    }

    public List<Sprite> GetFormationUIs(int flag)
    {
        List<Sprite> result = new List<Sprite>();
        if (controlsFormation.Count > flag && controlsFormation[flag].Count > 0)
        {
            foreach (var value in controlsFormation[flag])
            {
                if (troopCommanders[value].troopState != TroopState.DESTROYED && troopCommanders[value].aliveCount > 0)
                {
                    result.Add(controlFlagList[value].typeImage.sprite);
                }
            }
        }
        return result;
    }

    public int GetFormationAbleCommanderNum(int flag)
    {
        int count = 0;
        if (controlsFormation.Count > flag && controlsFormation[flag].Count > 0)
        {
            foreach (var value in controlsFormation[flag])
            {
                if (troopCommanders[value].troopState != TroopState.DESTROYED && troopCommanders[value].aliveCount > 0)
                {
                    count++;
                }
            }
        }
        return count;
    }
    public int GetFormationAbleCommanderAliveNum(int flag)
    {
        int count = 0;
        if (controlsFormation.Count > flag && controlsFormation[flag].Count > 0)
        {
            foreach (var value in controlsFormation[flag])
            {
                if (troopCommanders[value].troopState != TroopState.DESTROYED && troopCommanders[value].aliveCount > 0)
                {
                    count += troopCommanders[value].aliveCount;
                }
            }
        }
        return count;
    }
    public void UseFormation(int flag)
    {
        if (!InputManager.Instance.isShiftMulAble)
        {
            if (nowFormationFlag != flag)
                nowFormationFlag = flag;
            else
                return;
        }
        else
        {
            nowFormationFlag = -1;
        }
        int ableCommandNumber = GetFormationAbleCommanderNum(flag);
        if (ableCommandNumber > 0)
        {
            foreach (var i in controls)
            {
                SetOnChoose(troopCommanders[i], false);
            }
            if (!InputManager.Instance.isShiftMulAble)
                controls.Clear();
            foreach (var value in controlsFormation[flag])
            {
                controls.Add(value);
            }
            foreach (var i in controls)
            {
                SetOnChoose(troopCommanders[i], true);
            }
        }
    }

    public void InitCommanders()
    {
        for (int i = 0; i < commanders.Count; i++)
        {
            commanders[i].SetCenterPos(Vector3.zero);
        }
    }

    public void OnActive()
    {
    }

    public void OnUpdate()
    {

    }

    public void OnDeactive()
    {
        //回收池回收所有单位模型
        foreach (var commander in troopCommanders)
        {
            commander.ActionToAllUnit((s) =>
            {
                s.OnReset();
                GameObjectPoolManager.Instance.Recycle(s.model.gameObject, "Prefab/" + s.model.modelName);
            });
            commander.EndStatus(commander.status);
            commander.TroopState = TroopState.DESTROYED;
        }
        controls.Clear();
        troopCommanders.Clear();
        commanders.Clear();
        controlsFormation.Clear();
        rightMouseCenter = default;

        foreach (var controlFlag in controlFlagList)
        {
            GameObjectPoolManager.Instance.Recycle(controlFlag.gameObject, controlFlag.gameObject.name);
            controlFlag.SetOnChoose(false);
            controlFlag.gameObject.SetActive(true);
        }
        controlFlagList.Clear();
    }


    public void ForEachNowControl(System.Action<CommandUnit> action)
    {
        foreach (var i in controls)
        {
            action?.Invoke(troopCommanders[i]);
        }
    }

    public void ForEachTargetBelong(int belong, System.Action<CommandUnit> action)
    {
        for (int i = 0; i < troopCommanders.Count; i++)
        {
            if (troopCommanders[i].belong == belong)
            {
                action?.Invoke(troopCommanders[i]);
            }
        }
    }

    public Commander GetCommand(int belong)
    {
        Commander legionCommander = commanders.Find((c) => c.belong == belong);
        if (legionCommander == null)
        {
            legionCommander = new Commander();
            legionCommander.belong = belong;
            commanders.Add(legionCommander);
        }
        return legionCommander;
    }


    //注册的是数据头部
    public void RegistCommander(CommandUnit troopCommander)
    {
        Commander legionCommander = commanders.Find((c) => c.belong == troopCommander.belong);
        if (legionCommander == null)
        {
            legionCommander = new Commander();
            legionCommander.belong = troopCommander.belong;
            commanders.Add(legionCommander);
        }
        legionCommander.ableCommands.Add(troopCommander);
        troopCommanders.Add(troopCommander);
        troopCommander.ChangeFaceTo(-CameraControl.Instance.mainCamera.transform.forward);
        onNewCommandAdd?.Invoke();
    }

    public void RegistGeneralCommandFlag(CommandUnit commander, General general)
    {
        var flag = GameObjectPoolManager.Instance.Spawn("Prefab/ControlGeneralFlag", BattleManager.instance.canvasParent).GetComponent<ControlFlag>();
        flag.gameObject.SetActive(true);
        flag.Init(commander);
        flag.iconImage.sprite = general.GetHeadIcon();
        controlFlagList.Add(flag);
    }

    public void RegistTroopCommandFlag(CommandUnit commander, TroopEntity entityData, int belong)
    {
        var flag = GameObjectPoolManager.Instance.Spawn("Prefab/ControlFlag", BattleManager.instance.canvasParent).GetComponent<ControlFlag>();
        flag.gameObject.name = "Prefab/ControlFlag";
        flag.gameObject.SetActive(true);
        flag.Init(commander);
        flag.InitDetail(entityData);
        flag.SetBackColor(GameManager.instance.GetForceColor(belong));
        controlFlagList.Add(flag);
    }

    public void RegistTroopCommandFlag(CommandUnit commander, UnitType unitType, int belong)
    {
        var flag = GameObjectPoolManager.Instance.Spawn("Prefab/ControlFlag", BattleManager.instance.canvasParent).GetComponent<ControlFlag>();
        flag.gameObject.name = "Prefab/ControlFlag";
        flag.gameObject.SetActive(true);
        flag.Init(commander);
        flag.InitDetail(unitType, null);
        flag.SetBackColor(GameManager.instance.GetForceColor(belong));
        controlFlagList.Add(flag);
    }

    public Vector3 GetMostDenseEnermyPos(int belong)
    {
        CommandUnit result = null;
        int nowMax = 0;
        for (int i = 0; i < troopCommanders.Count; i++)
        {
            if (troopCommanders[i].belong == belong || troopCommanders[i].troopState == TroopState.DESTROYED) continue;
            if (result == null)
            {
                result = troopCommanders[i];
                nowMax = GetRangeMaxAbleNumber(result);
                continue;
            }
            int temp = GetRangeMaxAbleNumber(troopCommanders[i]);
            if (temp > nowMax)
            {
                nowMax = temp;
                result = troopCommanders[i];
            }
        }
        return result.lastPosition;
    }

    public int GetRangeMaxAbleNumber(CommandUnit command)
    {
        var hashSet = GridMapManager.instance.gridMap.GetCircleGridContainType<SoldierModel>(command.lastPosition, 10f);
        int result = 0;
        foreach (var c in hashSet)
        {
            if (c.commander.belong != command.belong)
            {
                continue;
            }
            result++;
        }
        return result;
    }

    public CommandUnit GetEnermyInRange(CommandUnit from, float range)
    {
        CommandUnit result = null;
        CommandUnit generalResult = null;
        float minDistance = 0f;
        float tempDistance;
        for (int i = 0; i < troopCommanders.Count; i++)
        {
            if (troopCommanders[i].belong == from.belong || troopCommanders[i].troopState == TroopState.DESTROYED) continue;
            tempDistance = Vector3.Distance(troopCommanders[i].lastPosition, from.lastPosition);
            if (tempDistance > range) continue;//区块筛选
            if (result == null)
            {
                if (troopCommanders[i].IsGeneral)
                {
                    generalResult = troopCommanders[i];
                }
                else
                {
                    result = troopCommanders[i];
                    minDistance = tempDistance;
                }
            }
            else
            {
                if (tempDistance < minDistance)
                {
                    if (troopCommanders[i].IsGeneral)
                    {
                        generalResult = troopCommanders[i];
                    }
                    else
                    {
                        result = troopCommanders[i];
                        minDistance = tempDistance;
                    }
                }
            }
        }
        if (result == null && generalResult != null)
        {
            return generalResult;
        }
        return result;
    }

    public CommandUnit GetCommandInRange(Vector3 center, float range, System.Func<CommandUnit, bool> condition)
    {
        CommandUnit result = null;
        float minDistance = 0f;
        float tempDistance;
        for (int i = 0; i < troopCommanders.Count; i++)
        {
            if (troopCommanders[i].troopState == TroopState.DESTROYED || !condition.Invoke(troopCommanders[i])) continue;
            tempDistance = Vector3.Distance(troopCommanders[i].lastPosition, center);
            if (tempDistance > range) continue;//区块筛选
            if (result == null)
            {
                result = troopCommanders[i];
                minDistance = tempDistance;
            }
            else
            {
                if (tempDistance < minDistance)
                {
                    result = troopCommanders[i];
                    minDistance = tempDistance;
                }
            }
        }
        return result;
    }
    public void GetCommandEnermy(Commander commander)
    {
        commander.nowEnermy = commanders.Find((c) => c.belong != commander.belong);
    }

    public void UpdateTroopCommand(float time)
    {

        for (int i = 0; i < commanders.Count; i++)
        {
            commanders[i].UpdateCommand(time);
            //if (BattleManager.autoBattle && troopCommanders[i].belong != BattleManager.instance.controlBelong)
            //{
            //    troopCommanders[i].TriggerCommandToAggressive();
            //}
        }
        //for (int i = 0; i < troopCommanders.Count; i++)
        //{
        //    troopCommanders[i].UpdateCommand(time);
        //    if (BattleManager.autoBattle && troopCommanders[i].belong != BattleManager.instance.controlBelong)
        //    {
        //        troopCommanders[i].TriggerCommandToAggressive();
        //    }
        //}
    }
    public List<GameObject> MoveArrowS = new List<GameObject>();

    ///// <summary>
    ///// 获得兵模控制
    ///// </summary>
    public void ChooseToMove(Vector3 targetPosition, bool isFocus = false)
    {
        List<CommandUnit> tempCommand = GetNowSelectedCommand();

        if (tempCommand.Count == 0) return;
        //生成移动的
        if (rightMouseCenter != default)
        {
            //可走另一分支
            if (commandWidthDic.Count > 0)
            {
                for (int i = 0; i < tempCommand.Count; i++)
                {
                    tempCommand[i].nowWidth = commandWidthDic[tempCommand[i]];
                    CommandMove(commandCenterV3[tempCommand[i]], tempCommand[i], faceTo, isFocus);
                    //  tempCommand[i].DesinateTargetTroop(desTargetDic[tempCommand[i]], faceTo);
                }
                commandWidthDic.Clear();
            }
            else
            {
                for (int i = 0; i < tempCommand.Count; i++)
                {
                    CommandMove(commandCenterV3[tempCommand[i]], tempCommand[i], faceTo, isFocus);
                    // tempCommand[i].DesinateTargetTroop(desTargetDic[tempCommand[i]], faceTo);
                }
            }
            //moveResult
            //for (int i = 0; i < tempCommand.Count; i++) {

            //     var command = tempCommand[resultFlagArray[i]];
            //    CommandMove(moveResult[i], command, faceTo);
            //}

            foreach (var item in guidesList)
            {
                item.GetComponent<GUIDEControl>().Action();
            }
            guidesList.Clear();

            rightMouseCenter = default;
            return;
        }
        Vector3 oldCenter = Vector3.zero;
        //进行位置排序，方向整合
        for (int i = 0; i < tempCommand.Count; i++)
        {
            oldCenter += tempCommand[i].lastPosition;
        }
        oldCenter /= tempCommand.Count;
        Vector3 face = targetPosition - oldCenter;
        face.y = 0;

        (Vector3[], int[]) resultTuple = GetCommandPosition(tempCommand, face);
        var result = resultTuple.Item1;
        var resultFlagArray = resultTuple.Item2;

        for (int i = 0; i < resultFlagArray.Length; i++)
        {
            result[i] += targetPosition;
            var command = tempCommand[resultFlagArray[i]];
            CommandMove(result[i], command, face, isFocus);
        }
    }

    /// <summary>
    /// 正常的点击鼠标右键 生成的是移动指示标志
    /// </summary>
    /// <param name="position"></param>
    /// <param name="command"></param>
    /// <param name="face"></param>
    public Dictionary<CommandUnit, GameObject> moveArrowDic = new Dictionary<CommandUnit, GameObject>();
    public void CommandMove(Vector3 position, CommandUnit command, Vector3 face, bool isFocus = false)
    {
        GameObject MoveArrow;
        if (!moveArrowDic.TryGetValue(command, out MoveArrow))
        {
            MoveArrow = GameObjectPoolManager.Instance.Spawn(DataBaseManager.Instance.configMap["MoveArrow"], position);
            moveArrowDic[command] = MoveArrow;
        }
        MoveArrow.SetActive(true);
        MoveArrow.GetComponent<MoveArrowControl>().StartActive(command, position);

        command.attackTarget = null;
        command.TroopState = TroopState.MOVING;
        if (isFocus)
            command.SetFocus();
        else
            command.SetFocus(3f);
        TroopMoveToTargetPosition(position, command, face);
    }

    /// <summary>
    /// 右键长按不移动显示对应 方向的光标显示
    /// </summary>
    /// <param name="targetPosition"></param>
    public void PreviewMoveGUIDE(Vector3 targetPosition)
    {
        List<CommandUnit> tempCommand = GetNowSelectedCommand();
        if (tempCommand.Count == 0) return;

        //确定rightFirstV在地图上的
        SetRightMouseCenter();

        desTargetDic.Clear();

        //确定当前控制方阵的模组数
        int totalNum = 0;
        for (int i = 0; i < tempCommand.Count; i++)
        {
            totalNum += tempCommand[i].aliveCount;
        }

        //获取当前所有控制队伍的模组数量动态设置GUIDES池子 
        SetGUIDES(totalNum);

        #region 计算所有方阵的中心点
        //计算所有方阵的中心点
        Vector3 tempCenter = Vector3.zero;
        for (int i = 0; i < tempCommand.Count; i++)
        {
            tempCenter += tempCommand[i].lastPosition;
        }
        tempCenter /= tempCommand.Count;
        #endregion

        //部队的队形朝向是从方阵中心点指向当前鼠标按下的点
        faceTo = rightMouseCenter - tempCenter;
        faceTo.y = 0;

        //运算当前朝向与位置下的单位各自的位置

        (Vector3[], int[]) resultTuple = GetCommandPosition(tempCommand, faceTo);
        var result = resultTuple.Item1;
        var resultFlagArray = resultTuple.Item2;

        int flag = 0;
        commandCenterV3.Clear();
        for (int i = 0; i < result.Length; i++)
        {
            var command = tempCommand[resultFlagArray[i]];
            flag = SetGUIDESPos(command, result[i] + targetPosition, flag, faceTo);
        }

    }

    Dictionary<CommandUnit, Vector3> commandCenterV3 = new Dictionary<CommandUnit, Vector3>();
    /// <summary>
    /// 长按右键拖拽鼠标之后队伍的队形调整
    /// </summary>
    /// <param name="targetPosition"></param>
    public void PreviewChangeMoveGUIDE(Vector3 targetPosition)
    {
        List<CommandUnit> tempCommand = GetNowSelectedCommand();

        if (tempCommand.Count == 0) return;

        SetRightMouseCenter();
        commandCenterV3.Clear();
        desTargetDic.Clear();
        commandWidthDic.Clear();
        int totalNum = 0;
        for (int i = 0; i < tempCommand.Count; i++)
        {
            totalNum += tempCommand[i].aliveCount;
        }

        SetGUIDES(totalNum);

        faceTo = targetPosition - rightMouseCenter;
        faceTo = new Vector3(faceTo.z, faceTo.y, -faceTo.x);

        var formation = FormationUtil.GetSquardFormation(tempCommand.Count, tempCommand.Count);
        var resultFlagArray = FormationUtil.FitTargetPositions(tempCommand, formation, faceTo);
        float distance = Vector3.Distance(rightMouseCenter, targetPosition);
        var moveResult = FormationUtil.GetFormationsWithLowerRight(tempCommand, distance, resultFlagArray, ref commandWidthDic);
        moveResult = FormationUtil.FormationChangeFace(moveResult, faceTo);


        int flag = 0;
        for (int i = 0; i < moveResult.Length; i++)
        {
            var command = tempCommand[resultFlagArray[i]];
            moveResult[i] += rightMouseCenter;
            var face = -rightMouseCenter + targetPosition;
            face = new Vector3(-face.z, face.y, face.x);


            if (command.NowWidth == -1)
            {
                command.NowWidth = (int)Mathf.Sqrt(command.aliveCount);
            }

            var resultPos = FormationUtil.GetFormationsWithFirstLine(command.aliveCount, command.moduleSize, command.spaceSize, commandWidthDic[command]);
            resultPos = FormationUtil.FormationChangeFace(resultPos, face);
            for (int j = 0; j < resultPos.Length; j++)
            {
                resultPos[j] += moveResult[i];
            }
            desTargetDic.Add(command, resultPos);
            Vector3 center = Vector3.zero;
            foreach (var item in resultPos)
            {
                center += item;
                guidesList[flag].transform.position = item;
                flag++;
            }
            commandCenterV3[command] = center / command.aliveCount;
        }
        faceTo = -faceTo;
    }

    /// <summary>
    /// 获取当前朝向下各个方阵各自的位置 以及对应的排序编号
    /// </summary>
    /// <param name="commands"></param>
    /// <returns></returns>
    public (Vector3[], int[]) GetCommandPosition(List<CommandUnit> commands, Vector3 faceTo)
    {
        var formation = FormationUtil.GetSquardFormation(commands.Count, commands.Count);
        int[] resultFlagArray = FormationUtil.FitTargetPositions(commands, formation, faceTo);
        float[] space = FormationUtil.GetSizeAndSpaceWithLineCenter(commands, resultFlagArray);
        var result = FormationUtil.GetFormationsWithFirstLineCenter(commands.Count, space);
        return (FormationUtil.FormationChangeFace(result, faceTo), resultFlagArray);
    }

    /// <summary>
    /// 获取当前所有控制队伍的模组数量动态设置GUIDES池子
    /// </summary>
    /// <param name="totalNum"></param>
    public void SetGUIDES(int totalNum)
    {
        for (int i = guidesList.Count; i < totalNum; i++)
        {
            GameObject MoveArrow = GameObjectPoolManager.Instance.Spawn(DataBaseManager.Instance.configMap["Guide"], Vector3.zero);
            MoveArrow.SetActive(true);
            guidesList.Add(MoveArrow);
        }
        for (int i = guidesList.Count - 1; i > totalNum; i--)
        {
            GameObjectPoolManager.Instance.Recycle(guidesList[i], DataBaseManager.Instance.configMap["Guide"]);
            guidesList.Remove(guidesList[i]);
        }
    }

    /// <summary>
    /// 记录右键按下的第一帧的地图内的位置 
    /// </summary>
    public void SetRightMouseCenter()
    {
        if (rightMouseCenter == default)
        {
            // rightMouseCenter = InputManager.Instance.mouseWorldPos;
            ray = Camera.main.ScreenPointToRay(InputManager.Instance.rightFirstV);
            if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
            {
                rightMouseCenter = m_HitInfo.point;
            }
            if (rightMouseCenter == InputManager.Instance.mouseWorldPos) Debug.LogError("IsTrue");
        }
    }

    /// <summary>
    /// 设置每个GUIDES的位置
    /// </summary>
    /// <param name="command"></param>
    /// <param name="originPos"></param>
    /// <param name="guidesIndex"></param>
    /// <returns></returns>
    public int SetGUIDESPos(CommandUnit command, Vector3 originPos, int guidesIndex, Vector3 face)
    {

        if (command.NowWidth == -1)
        {
            command.NowWidth = (int)Mathf.Sqrt(command.aliveCount);
        }

        var resultPos = FormationUtil.GetFormationsWithFirstLineCenter(command.aliveCount, command.moduleSize, command.spaceSize, command.NowWidth);
        resultPos = FormationUtil.FormationChangeFace(resultPos, face);
        for (int j = 0; j < resultPos.Length; j++)
        {
            resultPos[j] += originPos;
        }
        desTargetDic.Add(command, resultPos);
        Vector3 center = Vector3.zero;
        foreach (var item in resultPos)
        {
            guidesList[guidesIndex].transform.position = item;
            center += item;
            guidesIndex++;
        }
        commandCenterV3[command] = center / command.aliveCount;
        return guidesIndex;
    }

    /// <summary>
    /// 获取当前正在控制的command数组
    /// </summary>
    /// <returns></returns>
    public List<CommandUnit> GetNowSelectedCommand()
    {

        List<CommandUnit> tempCommand = new List<CommandUnit>();
        foreach (var i in controls)
        {
            if (troopCommanders[i].TroopState == TroopState.DESTROYED) continue;
            tempCommand.Add(troopCommanders[i]);
        }
        return tempCommand;
    }

    public CommandUnit IsNowChooseUnitCastAble(out string[] spellArray)
    {
        spellArray = null;

        //射线检测 当前鼠标位置的SoldierModel的

        foreach (var i in controls)
        {
            if (troopCommanders[i].TroopState == TroopState.DESTROYED) continue;//TODO变更成移除controlFlag;
            spellArray = troopCommanders[i].GetCastableSpells();
            if (spellArray != null && spellArray.Length > 0) return troopCommanders[i];
        }
        return null;
    }

    public bool CommandIsInCommanders(CommandUnit command)
    {
        foreach (var item in controls)
        {
            if (troopCommanders[item].Equals(command))
            {
                return true;
            }
        }
        return false;
    }

    public void ChooseAllAble()
    {
        nowFormationFlag = -1;
        for (int i = 0; i < this.troopCommanders.Count; i++)
        {
            if (troopCommanders[i].controlAble)
            {
                controls.Add(i);
            }
        }
        foreach (var i in controls)
        {
            SetOnChoose(troopCommanders[i], true);
        }
    }
    /// <summary>
    /// 点击了盾徽
    /// </summary>
    /// <param name="target"></param>
    public bool ChooseToAttack(CommandUnit target)
    {
        bool triggerAttack = false;
        foreach (var i in controls)
        {
            if (troopCommanders[i].TroopState == TroopState.DESTROYED) continue;//TODO变更成移除controlFlag;
            if (troopCommanders[i].SetTarget(target))
            {
                triggerAttack = true;
            }
        }
        if (triggerAttack)
        {
            var c = controlFlagList.Find((f) => f.commander == target);
            if (c != null)
            {
                c.TriggerShinyAnim();
            }
        }
        return triggerAttack;
    }

    //TODO:将下列内容作为扩展方法来进行附加
    public GameObject temp;
    public void TroopMoveToTargetPosition(Vector3 position, CommandUnit troop, Vector3 faceTo = default)
    {
        if (faceTo == default)
        {
            Vector3 face = position - troop.lastPosition;//两点的差异
            if (troop.moveList.Count > 0)
            {
                face = position - troop.moveList[troop.moveList.Count - 1];
            }
            troop.troopfaceTo = face;
        }
        else
        {
            troop.troopfaceTo = faceTo;
        }
        if (temp != null) temp.transform.position = position;
        DesinateTargetTroop(troop, position);
        //if(troop.aliveCount>=1&& Random.Range(0, 1f)>0.98f)
        //DialogManager.instance.SetBubbleTragetTransFrom(troop.troopsData[0].model.transform);
    }

    public void TroopReformation(CommandUnit troop)
    {
        //if (troop.moveList.Count > 0)
        //    DesinateTargetTroop(troop, troop.moveList[0]);
        //else
        DesinateTargetTroop(troop, troop.lastPosition);
    }

    //添加进移动列表里
    void DesinateTargetTroop(CommandUnit command, Vector3 centerPosition)
    {
        if (InputManager.Instance.isShiftMulAble)
        {
            command.moveList.Add(centerPosition);
        }
        else
        {
            command.moveList.Clear();
            command.moveList.Add(centerPosition);
        }


        command.DesinateTargetTroop();

    }


    public void AttackTarget(CommandUnit target)
    {
        foreach (var control in controls)
        {
            troopCommanders[control].SetTarget(target);
        }
    }
    /// <summary>
    /// 能攻击就攻击 不能攻击就移动到对应点
    /// </summary>
    /// <param name="target"></param>
    public void MoveAttackTarget(CommandUnit target, Vector3 targetPosition, bool focus = false)
    {
        bool triggerAttack = false;

        foreach (var control in controls)
        {
            if (troopCommanders[control].TroopState == TroopState.DESTROYED) continue;
            if (!troopCommanders[control].SetTarget(target))
            {
                ChooseToMove(targetPosition, focus);
                break;
            }
            else
            {
                if (focus)
                    troopCommanders[control].SetFocus();
                else
                {
                    troopCommanders[control].SetFocus(0f);
                }
                triggerAttack = true;
            }
        }
        if (triggerAttack)
        {
            var c = controlFlagList.Find((f) => f.commander == target);
            if (c != null)
            {
                c.TriggerShinyAnim();
            }
        }
    }


    public bool IsTargetCommandOnChoose(CommandUnit command)
    {
        foreach (var value in controls)
        {
            if (troopCommanders[value] == command) return true;
        }
        return false;
    }

    public void ResetControl()
    {
        if (!InputManager.Instance.isShiftMulAble)
        {
            foreach (var controlFlag in controls)
            {
                if (!controlFlagList[controlFlag].isCommand)
                    SetOnChoose(troopCommanders[controlFlag], false);
            }
            controls.Clear();
        }
    }
    public List<ControlFlag> AddMulCommands(HashSet<CommandUnit> commanders)
    {
        nowFormationFlag = -1;
        if (!InputManager.Instance.isShiftMulAble)
        {
            //ResetControl();
            foreach (var controlFlag in controls)
            {
                if (!commanders.Contains(this.troopCommanders[controlFlag]))
                    SetOnChoose(this.troopCommanders[controlFlag], false);
            }
            controls.Clear();
        }
        foreach (var commander in commanders)
        {
            for (int i = 0; i < this.troopCommanders.Count; i++)
            {
                if (this.troopCommanders[i].Equals(commander) && commander.controlAble)
                {
                    //增加是否多重控制
                    controls.Add(i);
                    continue;
                }
            }
        }
        List<ControlFlag> t = new List<ControlFlag>();
        foreach (var i in controls)
        {
            t.Add(controlFlagList[i]);
        }
        return t;
    }

    /// <summary>
    /// 获取目标的ControlFlag
    /// </summary>
    /// <param name="commander"></param>
    /// <returns></returns>
    public ControlFlag GetControlFlag(CommandUnit commander)
    {
        return controlFlagList.Find((c) => c.commander == commander);
    }

    public void ShowFlagSetOnChoose()
    {
        foreach (var i in controls)
        {
            if (controlFlagList[i].isCommand)
                SetOnChoose(troopCommanders[i], true);
        }
    }
    public void SetOnChoose(int[] controls, bool value)
    {
        foreach (var i in controls)
        {
            if (controlFlagList[i].isCommand)
                SetOnChoose(troopCommanders[i], value);
        }
    }
    public void TargetCommondModelClick(CommandUnit commander)
    {
        nowFormationFlag = -1;
        //刷新Control
        for (int i = 0; i < troopCommanders.Count; i++)
        {
            if (troopCommanders[i].Equals(commander))
            {
                if (!commander.controlAble) continue;
                //增加是否多重控制
                if (!InputManager.Instance.isShiftMulAble)
                {
                    foreach (var controlFlag in controls)
                    {
                        SetOnChoose(troopCommanders[controlFlag], false);
                    }
                    controls.Clear();
                    controls.Add(i);
                }
                else
                {
                    if (controls.Contains(i))
                    {
                        controls.Remove(i);
                        SetOnChoose(troopCommanders[i], false);
                    }
                    else
                    {
                        controls.Add(i);
                    }
                }
                break;
            }
        }
        //刷新Control
        foreach (var i in controls)
        {
            SetOnChoose(troopCommanders[i], true);
        }
    }


    void SetOnChoose(CommandUnit commander, bool value)
    {
        commander.SetOnChoose(value);
        controlFlagList.Find((f) => f.commander == commander)?.SetOnChoose(value);
    }

    public bool IsAttackAble(SoldierStatus from, SoldierStatus to)
    {
        if (to.nowState == 1) return false;
        float distance = GetSoldierSpaceDistance(from.model, to.model);
        if (distance < minblockDistance)
        {
            return true;
        }
        return false;
    }

    //TODO：需要进行抽象剥离方位逻辑
    public float GetSoldierSpaceDistance(SoldierModel from, SoldierModel to)
    {
        return Vector3.Distance(from.transform.position, to.transform.position) - from.nowStatus.EntityData.originData.ocupySize / 2 - to.nowStatus.EntityData.originData.ocupySize / 2;
    }

    public void DestroyAllEnermy()
    {
        foreach (var command in troopCommanders)
        {
            if (command.belong != BattleManager.instance.controlBelong)
            {
                TroopDestroyed(command);
            }
        }
    }

    /// <summary>
    /// 部队被摧毁
    /// </summary>
    /// <param name="target"></param>
    public void TroopDestroyed(CommandUnit target)
    {
        target.SetOnChoose(false);
        //摧毁指挥标志
        var flag = controlFlagList.FindIndex((f) => f.commander != null && f.commander.Equals(target));
        if (flag != -1)
            controlFlagList[flag].gameObject.SetActive(false);
        //controlFlagList[flag].commander = null;
        //TODO:改成回收
        for (int i = 0; i < troopCommanders.Count; i++)
        {
            troopCommanders[i].EngagedTroopRemove(target);
        }
        target.TroopState = TroopState.DESTROYED;
        target.eventModel.FireEvent(Constant_Event.TROOP_REMOVE, "Troop", this);
        //移除控制
        controls.Remove(troopCommanders.IndexOf(target));
        //隐藏指挥标志

    }

    /// <summary>
    /// 部队复生
    /// </summary>
    public void TroopReverse(CommandUnit target)
    {
        var flag = controlFlagList.FindIndex((f) => f.commander != null && f.commander.Equals(target));
        controlFlagList[flag].gameObject.SetActive(true);
    }

    /// <summary>
    /// 玩家部队全阵亡
    /// </summary>
    public bool IsPlayerAllDie()
    {
        for (int i = 0; i < troopCommanders.Count; i++)
        {
            if (troopCommanders[i].belong == BattleManager.instance.controlBelong && troopCommanders[i].TroopState != TroopState.DESTROYED)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 敌对部队全阵亡
    /// </summary>
    public bool IsEnermyAllDie()
    {
        for (int i = 0; i < troopCommanders.Count; i++)
        {
            if (troopCommanders[i].belong != BattleManager.instance.controlBelong && troopCommanders[i].aliveCount != 0)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 布置部队
    /// </summary>
    public CommandUnit DeployTroop(Vector3 position, Vector3 forward, General general, int belong, int equipBelong)
    {
        //尝试获取阵型
        UnitData data = DataBaseManager.Instance.GetIdNameDataFromList<UnitData>(general.sourceData.unitType);
        TroopEntity entity = new TroopEntity(data);
        var result = FormationUtil.GetFormationsWithCenter(1, data.ocupySize, 0.1f);
        result = FormationUtil.FormationChangeFace(result, forward);
        NavMeshHit hit;
        CommandUnit commander = new CommandUnit();
        commander.IsGeneral = true;
        commander.belong = belong;
        commander.entityData = entity;
        commander.realityTroop = null;
        int count = -1;//从下标0开始
        //EquipData weapon = GetEquipSetWeapon(data.armourEquipSetId);
        string speciesType = general.sourceData.speciesType;
        count++;
        result[count] += position;
        string modelName = DataBaseManager.Instance.GetModelName(data.armourEquipSetId, data.weaponEquipSetId, DataBaseManager.Instance.GetSpeciesName(speciesType));
        GameObject g = default;
        g = GameObjectPoolManager.Instance.Spawn("Prefab/Model_A_HumanMelee");
        var model = g.GetComponent<SoldierModel>();
        SoldierStatus status = new SoldierStatus();
        //TODO:把GeneralData里的源数值付给SoldierStatus
        status.commander = commander;
        status.EntityData = entity;
        status.model = model;
        status.nowHp = general.life;
        status.onLifeChange += () => general.life = status.nowHp;
        int flag = 0;
        model.actionModel.control.attackMotionTakeOver = (t) =>
        {
            if (flag == 3) flag = 0;
            flag++;
            switch (flag)
            {
                case 0:
                    return "Attack";
                case 2:
                    return "Attack_Stab";
                case 1:
                    return "Attack_Slash";
            }
            return "Attack";
        };
        model.actionModel.SetMotionPriority(0);
        //使用的是单独的
        g.gameObject.SetActive(true);
        g.transform.parent = BattleManager.instance.parent.transform;
        g.transform.position = result[0];
        g.transform.localScale = Vector3.one;
        if (!string.IsNullOrEmpty(data.armourEquipSetId))
        {
            EquipSwitchXML switchModel = model.EquipSwitch;
            switchModel.targetModelName = modelName;//使用模型装备文件
            switchModel.AddEquipSet(data.armourEquipSetId);
            switchModel.AddEquipSet(data.weaponEquipSetId);
            switchModel.speciesType = speciesType;
            switchModel.subSpeciesType = "";
            switchModel.equipBelong = equipBelong;
            switchModel.OnInit();
        }
        model.modelName = modelName;
        model.Init(status);
        commander.AddSoldier(model.nowStatus);//支配内容
        ARPGControl.AttachARPGControl(g, general);
        CharacterManager.Instance.RegistCharacter(status, $"General_{general.sourceData.idName}");
        //根据地形进行非合理点的位置调整
        if (NavMesh.SamplePosition(result[0], out hit, 30f, 1))
        {
            g.GetComponent<NavMeshAgent>().Warp(hit.position);//传送
            g.transform.forward = forward;
        }
        else if (NavMesh.SamplePosition(result[0], out hit, 50f, 1))
        {
            g.GetComponent<NavMeshAgent>().Warp(hit.position);
            g.transform.forward = forward;
        }
        if (commander != null)
        {
            RegistCommander(commander);
            RegistGeneralCommandFlag(commander, general);
            BattleManager.instance.miniMap.RegistMiniMapCommandFlag(commander);
        }
        return commander;
    }

    public SoldierStatus DeployUnit(Vector3 position, string unitIdName, int belong)
    {
        //尝试获取阵型
        UnitData data = DataBaseManager.Instance.GetIdNameDataFromList<UnitData>(unitIdName);
        TroopEntity entity = new TroopEntity(data);
        string speciesType = data.speciesType;
        NavMeshHit hit;
        var weapon = DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>(data.weaponEquipSetId);
        string actionMotion = weapon.TargetActionModel;
        if (string.IsNullOrEmpty(speciesType))
            speciesType = data.speciesType;
        //获取对应的物种数据
        string modelName = DataBaseManager.Instance.GetModelName(data.armourEquipSetId, data.weaponEquipSetId, DataBaseManager.Instance.GetSpeciesName(speciesType));
        var g = GameObjectPoolManager.Instance.Spawn("Prefab/" + modelName);
        g.gameObject.SetActive(true);
        g.transform.SetParent(BattleManager.instance.parent.transform);
        g.transform.position = position;
        g.transform.localScale = Vector3.one;
        var model = g.GetComponent<SoldierModel>();
        SoldierStatus status = new SoldierStatus();
        status.EntityData = entity;
        status.model = model;
        if (!string.IsNullOrEmpty(data.weaponEquipSetId))
        {
            //model.nowStatus.weapon = weapon;
            EquipSwitchXML switchModel = model.EquipSwitch;
            switchModel.targetModelName = modelName;//使用模型装备文件
            switchModel.AddEquipSet(data.armourEquipSetId);
            switchModel.AddEquipSet(data.weaponEquipSetId);
            switchModel.speciesType = speciesType;
            switchModel.subSpeciesType = data.subSpeciesType;
            switchModel.equipBelong = belong;
            switchModel.OnInit();
        }
        model.Init(status);
        model.modelName = modelName;
        //根据地形进行非合理点的位置调整
        if (NavMesh.SamplePosition(position, out hit, 30f, 1))
        {
            g.GetComponent<NavMeshAgent>().Warp(hit.position);//传送

        }
        else if (NavMesh.SamplePosition(position, out hit, 50f, 1))
        {
            g.GetComponent<NavMeshAgent>().Warp(hit.position);

        }
        return status;
    }

    public Dictionary<string, CommandUnit> targetCommandDic = new Dictionary<string, CommandUnit>();
    public CommandUnit DeployUnitToTargetCommand(Vector3 position, Vector3 forward, string unitIdName, string speciesType, string subSpeciesType, int belong, string targetCommand)
    {
        //尝试获取阵型
        UnitData data = DataBaseManager.Instance.GetSpeciesTypeUnitData(unitIdName, speciesType, subSpeciesType);
        TroopEntity entity = new TroopEntity(data);
        NavMeshHit hit;
        CommandUnit commander = null;
        if (!targetCommandDic.ContainsKey(targetCommand))
        {
            commander = new CommandUnit();
            var weapon = DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>(data.weaponEquipSetId);
            string actionMotion = weapon.TargetActionModel;
            if (System.Enum.TryParse<ModelMotionType>(actionMotion, out ModelMotionType motionType))
            {
                switch (motionType)
                {
                    case ModelMotionType.RANGE:
                        commander.isRangeAttackIntension = true;
                        commander.attackRange = weapon.AttackRange;
                        break;
                    default:
                        commander.isRangeAttackIntension = false;
                        break;
                }
            }
            commander.belong = belong;
            commander.entityData = entity;
            RegistCommander(commander);
            RegistTroopCommandFlag(commander, entity, belong);
            BattleManager.instance.miniMap.RegistMiniMapCommandFlag(commander);
            targetCommandDic[targetCommand] = commander;
        }
        else
        {
            commander = targetCommandDic[targetCommand];
        }
        if (string.IsNullOrEmpty(speciesType))
            speciesType = data.speciesType;
        //获取对应的物种数据
        string modelName = DataBaseManager.Instance.GetModelName(data.armourEquipSetId, data.weaponEquipSetId, DataBaseManager.Instance.GetSpeciesName(speciesType));
        string targetModel = DataBaseManager.Instance.GetIdNameDataFromList<EquipAbleModelData>(modelName).targetModelName;
        if (string.IsNullOrEmpty(targetModel))
        {
            targetModel = modelName;
        }
        var g = GameObjectPoolManager.Instance.Spawn("Prefab/" + targetModel);
        g.gameObject.SetActive(true);
        g.transform.SetParent(BattleManager.instance.parent.transform);
        g.transform.position = position;
        g.transform.localScale = Vector3.one;
        var model = g.GetComponent<SoldierModel>();
        SoldierStatus status = new SoldierStatus();
        status.commander = commander;
        status.EntityData = entity;
        status.model = model;
        commander.AddSoldier(status);//支配内容
        if (!string.IsNullOrEmpty(data.weaponEquipSetId))
        {
            //model.nowStatus.weapon = weapon;
            EquipSwitchXML switchModel = model.EquipSwitch;
            switchModel.targetModelName = modelName;//使用模型装备文件
            switchModel.AddEquipSet(data.armourEquipSetId);
            switchModel.AddEquipSet(data.weaponEquipSetId);
            switchModel.speciesType = speciesType;
            switchModel.subSpeciesType = subSpeciesType;
            switchModel.equipBelong = belong;
            switchModel.OnInit();
        }
        model.Init(status);
        model.modelName = modelName;
        //根据地形进行非合理点的位置调整
        if (NavMesh.SamplePosition(position, out hit, 30f, 1))
        {
            g.GetComponent<NavMeshAgent>().Warp(hit.position);//传送
            g.transform.forward = forward;
        }
        else if (NavMesh.SamplePosition(position, out hit, 50f, 1))
        {
            g.GetComponent<NavMeshAgent>().Warp(hit.position);
            g.transform.forward = forward;
        }
        return commander;
    }

    public CommandUnit DeployTroop(Vector3 position, Vector3 forward, string unitIdName, string speciesType, string subSpeciesType, int belong, int num)
    {
        //尝试获取阵型
        UnitData data = DataBaseManager.Instance.GetSpeciesTypeUnitData(unitIdName, speciesType, subSpeciesType);
        TroopEntity entity = new TroopEntity(data);
        entity.nowNum = num;
        return DeployTroop(position, forward, entity, belong, belong);
    }

    public CommandUnit RaiseToTempCommand(List<SoldierStatus> soldierList, int belong)
    {
        var result = FormationUtil.GetFormationsWithCenter(soldierList.Count, 1f, 0.1f);
        Vector3 centorPos = default;
        foreach (var soldier in soldierList)
        {
            centorPos += soldier.model.lastPosition;
        }
        centorPos /= soldierList.Count;
        Vector3 targetPos = centorPos;
        Vector3 forward = targetPos - soldierList[0].model.lastPosition;
        result = FormationUtil.FormationChangeFace(result, forward);
        bool isNewUpdate = false;
        CommandUnit tempRaised = GetCommandInRange(centorPos, 10f, ((c) => c.idName == "RaisedCommand"));
        if (tempRaised == null)
        {
            isNewUpdate = true;
            tempRaised = new CommandUnit();
            tempRaised.idName = "RaisedCommand";
            tempRaised.SetBoolValue("DisableRemote", true);
            //重设选中光圈
            tempRaised.belong = belong;
        }
        tempRaised.entityData = new TroopEntity(DataBaseManager.Instance.GetSpeciesTypeUnitData(soldierList[0].EntityData.originData, "Zombie"));
        int count = -1;//从下标0开始
        //获取对应的物种数据
        for (int i = 0; i < soldierList.Count; i++)
        {
            count++;
            result[count] += targetPos;
            SoldierStatus status = soldierList[i];
            //执行父级的分离
            status.commander.SeperateUnit(status);
            status.commander = tempRaised;
            tempRaised.AddSoldier(status);
            //根据地形进行非合理点的位置调整
            status.SetMoveTargetPos(NavmeshAblePos(result[i]));
        }
        if (isNewUpdate)
        {
            RegistCommander(tempRaised);
            RegistTroopCommandFlag(tempRaised, UnitType.RAISED, belong);
            BattleManager.instance.miniMap.RegistMiniMapCommandFlag(tempRaised);
            tempRaised.SetOnChoose(false);
        }
        return tempRaised;
    }

    NavMeshHit hit;
    public Vector3 NavmeshAblePos(Vector3 input)
    {
        if (NavMesh.SamplePosition(input, out hit, 30f, 1))
        {
            return hit.position;
        }
        else if (NavMesh.SamplePosition(input, out hit, 50f, 1))
        {
            return hit.position;
        }
        return input;
    }

    public CommandUnit SeperateTroop(Vector3 targetPos, CommandUnit origin, int belong, int num)
    {
        //尝试获取阵型
        var result = FormationUtil.GetFormationsWithCenter(num, origin.moduleSize, origin.spaceSize);
        Vector3 forward = targetPos - origin.lastPosition;
        result = FormationUtil.FormationChangeFace(result, forward);
        NavMeshHit hit;
        CommandUnit commander = new CommandUnit();
        bool needResetGuide = IsTargetCommandOnChoose(origin);
        //重设选中光圈
        if (needResetGuide)
        {
            origin.SetOnChoose(false);
        }
        commander.belong = belong;
        commander.entityData = origin.entityData;
        int count = -1;//从下标0开始
        UnitData unitData = null;
        //获取对应的物种数据
        for (int i = 0; i < num; i++)
        {
            count++;
            result[count] += targetPos;
            SoldierStatus status = origin.SeperateUnit(targetPos);
            status.commander = commander;
            commander.AddSoldier(status);//支配内容
            if (unitData == null)
            {
                unitData = status.EntityData.originData;
            }
            //根据地形进行非合理点的位置调整
            if (NavMesh.SamplePosition(result[i], out hit, 30f, 1))
            {
                status.SetMoveTargetPos(hit.position);
            }
            else if (NavMesh.SamplePosition(result[i], out hit, 50f, 1))
            {
                status.SetMoveTargetPos(hit.position);
            }
        }
        RegistCommander(commander);
        RegistTroopCommandFlag(commander, commander.entityData, belong);
        BattleManager.instance.miniMap.RegistMiniMapCommandFlag(commander);
        if (needResetGuide)
            origin.SetOnChoose(true);
        return commander;
    }

    public CommandUnit DeployTroop(Vector3 position, Vector3 forward, TroopControl troop, int belong, int equipBelong)
    {
        return DeployTroop(position, forward, troop.troopEntity, belong, equipBelong);
    }


    CommandUnit DeployTroop(Vector3 position, Vector3 forward, TroopEntity entity, int belong, int equipBelong)
    {
        //尝试获取阵型
        EquipSetData weapon = entity.weaponEquipSet.data;
        EquipSetData armour = entity.armourEquipSet.data;
        int num = entity.nowNum;
        int width = num < 10 ? num : 10;
        var result = FormationUtil.GetFormationsWithCenter(num, entity.originData.ocupySize, 0.1f, width);
        result = FormationUtil.FormationChangeFace(result, forward);
        NavMeshHit hit;
        CommandUnit commander = new CommandUnit();
        commander.nowWidth = width;
        string actionMotion = weapon.TargetActionModel;
        if (System.Enum.TryParse<ModelMotionType>(actionMotion, out ModelMotionType motionType))
        {
            switch (motionType)
            {
                case ModelMotionType.RANGE:
                    commander.isRangeAttackIntension = true;
                    commander.attackRange = weapon.AttackRange;
                    break;
                default:
                    commander.isRangeAttackIntension = false;
                    break;
            }
        }
        commander.belong = belong;
        commander.entityData = entity;
        int count = -1;//从下标0开始
                       //EquipData weapon = GetEquipSetWeapon(data.armourEquipSetId);
        string speciesType = entity.speciesType;
        string modelName = DataBaseManager.Instance.GetModelName(armour.idName, weapon.idName, DataBaseManager.Instance.GetSpeciesName(speciesType));
        string targetModel = DataBaseManager.Instance.GetIdNameDataFromList<EquipAbleModelData>(modelName).targetModelName;
        if (string.IsNullOrEmpty(targetModel))
        {
            targetModel = modelName;
        }
        //获取对应的物种数据
        for (int i = 0; i < entity.nowNum; i++)
        {
            count++;
            result[count] += position;
            var g = GameObjectPoolManager.Instance.Spawn("Prefab/" + targetModel);
            g.SetActive(true);
            g.transform.SetParent(BattleManager.instance.parent.transform);
            g.transform.position = result[i];
            g.transform.localScale = Vector3.one;
            var model = g.GetComponent<SoldierModel>();
            SoldierStatus status = new SoldierStatus();
            status.commander = commander;
            status.EntityData = entity;
            status.model = model;
            commander.AddSoldier(status);//支配内容
            if (!string.IsNullOrEmpty(entity.weaponEquipSet.data.idName))
            {
                EquipSwitchXML switchModel = model.EquipSwitch;
                switchModel.targetModelName = modelName;//使用模型装备文件
                switchModel.AddEquipSet(entity.armourEquipSet.data.idName);
                switchModel.AddEquipSet(entity.weaponEquipSet.data.idName);
                switchModel.speciesType = speciesType;
                switchModel.subSpeciesType = entity.subSpeciesType;
                switchModel.equipBelong = equipBelong;
                switchModel.OnInit();
            }
            model.Init(status);
            model.modelName = modelName;
            //根据地形进行非合理点的位置调整
            if (NavMesh.SamplePosition(result[i], out hit, 30f, 1))
            {
                g.GetComponent<NavMeshAgent>().Warp(hit.position);//传送
                g.transform.forward = forward;
            }
            else if (NavMesh.SamplePosition(result[i], out hit, 50f, 1))
            {
                g.GetComponent<NavMeshAgent>().Warp(hit.position);
                g.transform.forward = forward;
            }

        }
        if (commander != null)
        {
            RegistCommander(commander);
            RegistTroopCommandFlag(commander, entity, belong);
            BattleManager.instance.miniMap.RegistMiniMapCommandFlag(commander);
            if (entity.originData.statusAttachs != null)
            {
                for (int i = 0; i < entity.originData.statusAttachs.Length; i++)
                {
                    var standardStatus = StatusManager.Instance.GetStatus(entity.originData.statusAttachs[i]);
                    if (standardStatus.type == StatusLayerType.COMMAND_STATUS)
                    {
                        commander.AddStatus(standardStatus);
                        //Debug.LogError("CommandStatusNeedAttach");
                    }
                    //status.Add(StatusManager.Instance.RequestStatus(standardStatus, this, commander.belong));
                }
            }
        }
        return commander;
    }



    public CommandUnit CharactersGroupUp(int belong, Character[] characters)
    {
        CommandUnit commander = new CommandUnit();
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i] != null)
            {
                commander.AddSoldier(characters[i].refData);
                characters[i].refData.commander = commander;
                //characters[i].refData.
            }
        }
        RegistCommander(commander);
        //commander.TriggerCommandToAggressive();
        return commander;
    }
}
