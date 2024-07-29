using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// 指挥模块
/// </summary>
public class CommandUnit : AggregationEntity
{
    /// <summary>
    /// 部队所属
    /// </summary>
    public int belong;
    /// <summary>
    /// 实际存在的部队
    /// </summary>
    public TroopControl realityTroop;
    public TroopEntity entityData;
    /// <summary>
    /// 部队朝向
    /// </summary>
    public Vector3 troopfaceTo;
    /// <summary>
    /// 当前位置
    /// </summary>
    public Vector3 lastPosition;
    /// <summary>
    /// 进攻性激活触发
    /// </summary>
    public CommandTrigger aggressiveTrigger = new CommandTriggerAll();
    /// <summary>
    /// 是否被触发
    /// </summary>
    public bool hasBeenTriggerAggressive
    {
        get;
        private set;
    }
    public float aspectRatio;
    /// <summary>
    /// 强制执行目标
    /// </summary>
    float focusOnTarget;
    /// <summary>
    /// 是否是远程攻击意图（使用LastPosition中心点相互计算攻击范围）
    /// </summary>
    public bool isRangeAttackIntension;

    public bool IsRangeAttackAble => isRangeAttackIntension && !GetBoolValue("DisableRemote");


    public bool IsGeneral;
    /// <summary>
    /// 攻击范围
    /// </summary>
    public float attackRange;

    public const int MAX_COMMAND_COUNT = 10;
    public const float MAX_RECORD_TIME = 10;

    public SoldierStatus centerSoldier;
    public bool canSetTarget = true;

    /// <summary>
    /// 被包围
    /// </summary>
    public bool isBeFlank;
    /// <summary>
    /// 包围的生效时间——1s进入 1s移出
    /// </summary>
    public float flankCountDown;
    int commandCount;
    //0-剧情动画中 1-待机 2-移动 3-交战中 4-攻击移动
    public TroopState troopState;
    public TroopState TroopState
    {
        get
        {
            return troopState;
        }
        set
        {
            if (troopState != value)//阶段切换
            {
                switch (value)
                {
                    case TroopState.MOVING:
                        //移除所有攻击关系进行撤离
                        moveStartAliveCount = aliveCount;
                        TroopEscapeBattle();
                        engageSet.Clear();
                        if (attackTarget != null)
                        {
                            attackTarget = null;
                        }
                        break;
                    case TroopState.ENGAGING:
                        if (IsRangeAttackAble)
                        {
                            ForEachAlive((s) =>
                            {
                                s.nowAttackRange = 1f;
                            });
                        }
                        break;
                    default:
                        break;
                }
                if (value != TroopState.ENGAGING && IsRangeAttackAble)
                {
                    ForEachAlive((s) =>
                    {
                        s.nowAttackRange = s.weapon.AttackRange;
                    });
                }
            }
            focusOnTarget = 0f;
            troopState = value;
        }
    }
    const float flankDegree = 75f;
    /// <summary>
    /// 进行移动初始时的人员数量
    /// </summary>
    int moveStartAliveCount;

    public List<(CommandUnit, int, float)> damageRecordList = new List<(CommandUnit, int, float)>();

    /// <summary>
    /// 单位数据集合
    /// </summary>
    public List<SoldierStatus> troopsData = new List<SoldierStatus>();
    /// <summary>
    /// 进攻指向目标
    /// </summary>
    public CommandUnit attackTargettemp;
    /// <summary>
    /// 进攻指向目标
    /// </summary>
    public CommandUnit attackTarget
    {
        get
        {
            return attackTargettemp;
        }
        set
        {
            //if (value != null)
            //    Debug.LogError(value.entityData.originData.idName);
            attackTargettemp = value;
        }
    }
    /// <summary>
    /// 交战关联列表
    /// </summary>
    public HashSet<CommandUnit> engageSet = new HashSet<CommandUnit>();
    /// <summary>
    /// 事件模块
    /// </summary>
    public EventProcessModel eventModel = new EventProcessModel();
    /// <summary>
    /// 单位之间的间隔距离
    /// </summary>
    public float spaceSize = 0.2f;

    public float moduleSize = 1f;
    /// <summary>
    /// 基础攻击增加值
    /// </summary>
    public int basicAttackBonus;
    /// <summary>
    /// 破甲增加值
    /// </summary>
    public int breakAttackBonus;
    public int NowWidth
    {
        get
        {
            if (nowWidth == -1)
            {
                nowWidth = (int)Mathf.Sqrt(aliveCount) + 1;
            }
            if (aliveCount < nowWidth) nowWidth = aliveCount;
            return nowWidth;
        }
        set
        {
            if (value >= aliveCount)
            {
                nowWidth = MaxWidth;
            }
            else if (value <= MinWidth)
            {
                nowWidth = MinWidth;
            }
            else if (value <= MaxWidth)
            {
                nowWidth = value;
            }
            else if (value > MaxWidth)
            {
                nowWidth = MaxWidth;
            }

        }
    }

    public int nowWidth = -1;

    public int MinWidth
    {
        get
        {
            return aliveCount > 4 ? 4 : aliveCount;
        }
    }

    public int MaxWidth
    {
        get
        {
            return aliveCount > 15 ? 15 : aliveCount;
        }
    }

    /// <summary>
    /// 控制
    /// </summary>
    public bool controlAble => belong == BattleManager.instance.controlBelong;
    /// <summary>
    /// 可用单位计数
    /// </summary>
    public int aliveCount = 0;

    public int ammoCount;

    public List<EntityStatus> status = new List<EntityStatus>();
    public List<Vector3> moveList = new List<Vector3>();
    public float reachPercentage = 0.1f;
    public event System.Action onChoose;
    public event System.Action onUnChoose;
    int reachNumber = 0;

    public float overallWidth
    {
        get
        {
            if (aliveCount == 1)
            {
                NowWidth = 1;
            }
            return NowWidth * (moduleSize + spaceSize) - spaceSize;
        }
    }

    public string ammoName = default;

    public float nowArea
    {
        get
        {
            return aliveCount * (moduleSize + spaceSize);
        }

    }
    public SquardFormation formation { get { return FormationUtil.GetSquardFormation(aliveCount, NowWidth); } }

    /// <summary>
    /// 最大血量数值
    /// </summary>
    public int CommandUnit_TotalHP
    {
        get
        {
            return troopsData.Count * entityData.maxLife * (int)SoldierStatus.lifeDouble;
        }
    }

    int nowhp = 0;
    /// <summary>
    /// 当前军队血量数值
    /// </summary>
    public int CommandUnit_NowHP
    {

        get
        {
            nowhp = 0;
            for (int i = 0; i < aliveCount; i++)
            {
                nowhp += troopsData[i].nowHp;
            }
            return nowhp;
        }
    }
    public int nowMorale;
    /// <summary>
    /// 当前士气
    /// </summary>
    public int NowMorale
    {
        get
        {
            nowMorale = CalculateCurrentMoraleValue();
            return nowMorale;
        }
    }

    int CalculateCurrentMoraleValue()
    {
        return (int)((GameManager.instance.moraleCurve.Evaluate(CommandUnit_NowHP / (float)CommandUnit_TotalHP)) * entityData.moraleBenchmark);
    }

    public float GetAttackCoolDown()
    {
        float result = Random.Range(0.6f, 1f) / (1 + nowWeaponSpeedDelta);
        if (belong != BattleManager.instance.controlBelong)
        {
            result = (result + (1 - BattleManager.instance.enermyBattleWill) * 2 * result);
        }
        return result;
    }
    public float nowWeaponSpeedDelta;
    public float GetWeaponSpeedDelta()
    {
        return nowWeaponSpeedDelta;
    }

    public void ReachNumberIncrease()
    {
        reachNumber++;
        if ((reachNumber / (float)aliveCount) > reachPercentage)
        {
            //进入下一个位置
            NextMovePoint();
        }
    }

    public void NextMovePoint()
    {
        if (moveList.Count == 0 || moveList.Count == 1)
        {

            return;
        }
        moveList.Remove(moveList[0]);
        DesinateTargetTroop();
    }

    public void AutoDie()
    {
        var t = aliveCount;
        for (int i = 0; i < t; i++)
        {
            if (troopsData[i].nowHp != 0)
            {
                troopsData[i].nowHp = 0;
                troopsData[i].TriggerDying(true);
            }
        }
    }

    public void SetHoldMode(bool value)
    {
        if (value)
        {
            ForEachAlive((s) =>
            {
                s.model.actionModel.SetMotionPriority(40);
            });
        }
        else
        {
            ForEachAlive((s) =>
            {
                s.model.actionModel.SetMotionPriority(50);
            });
        }
    }

    /// <summary>
    /// 根据被动技能决定
    /// </summary>
    public string[] GetCastableSpells()
    {
        if (entityData != null)
            return entityData.originData.castableSpells;
        return null;
    }

    public void DesinateTargetTroop()
    {
        reachNumber = 0;
        if (aliveCount == 0) return;
        //correctedValue = lastPosition - centerSoldier.model.transform.position;
        oldLastPos = lastPosition;
        var result = FormationUtil.GetFormationsWithCenter(aliveCount, moduleSize, spaceSize, NowWidth);
        result = FormationUtil.FormationChangeFace(result, troopfaceTo);//怀疑没根据中心点修正绝对位置
        //解除model的攻击目标
        //重新排列位置再进行移动
        int[] resultFlagArray = FormationUtil.ResetPosition(this, formation, troopfaceTo);
        NavMeshHit hit;
        for (int i = 0; i < resultFlagArray.Length; i++)
        {
            //result[i] += lastPosition;
            result[i] += moveList[0];
            SoldierStatus soldier = GetSoldier(resultFlagArray[i]);
            if (attackTarget != null && soldier.aimUnits.Count > 0) continue;

            //攻击状态且已经出现阻挡关系的单位不影响
            if (NavMesh.SamplePosition(result[i], out hit, 30f, 1))
            {
                soldier.SetMoveTargetPos(hit.position);//传送
            }
            else if (NavMesh.SamplePosition(result[i], out hit, 50f, 1))
            {
                soldier.SetMoveTargetPos(hit.position);
            }
        }
    }

    #region 遗弃的方法
    public void DesinateTargetTroop(Vector3[] result, Vector3 troopfaceTo)
    {
        reachNumber = 0;
        moveList.Clear();
        attackTarget = null;
        TroopState = TroopState.MOVING;
        this.troopfaceTo = troopfaceTo;
        //解除model的攻击目标
        //重新排列位置在进行
        int[] resultFlagArray = FormationUtil.ResetPosition(this, formation, troopfaceTo);
        //NavMeshHit hit;
        for (int i = 0; i < result.Length; i++)
        {
            SoldierStatus soldier = GetSoldier(resultFlagArray[i]);
            if (attackTarget != null && soldier.aimUnits.Count > 0) continue;
            ////攻击状态且已经出现阻挡关系的单位不影响
            soldier.SetMoveTargetPos(result[i]);
        }
    }
    #endregion
    public void AddSoldier(SoldierStatus newSoldier)
    {
        if (newSoldier.nowState >= 0)
        {
            troopsData.Add(newSoldier);
            aliveCount++;
            if (troopsData.Count != aliveCount)
            {
                troopsData.Swap(aliveCount - 1, troopsData.Count - 1);
            }
            //判断是否需要换位
        }
        else
        {

            troopsData.Add(newSoldier);
        }
    }

    /// <summary>
    /// 获取目标下标士兵
    /// </summary>
    public SoldierStatus GetSoldier(int index)
    {
        return troopsData[index];
    }

    /// <summary>
    /// 在State赋值完毕后进行Target的赋值
    /// </summary>
    public void SetFocus(float value = 99999f)
    {
        focusOnTarget = value;
    }

    public bool isOnChoose;
    public void SetOnChoose(bool value)
    {

        isOnChoose = value;
        if (value)
        {
            ForEachAlive((s) => s.model.ShowGuide());
            onChoose?.Invoke();
        }
        else
        {
            ForEachAlive((s) => s.model.HideGuide());
            onUnChoose?.Invoke();
        }
    }

    /// <summary>
    /// 移入Grid
    /// </summary>
    public void OcupyGrid(BaseGrid grid)
    {
        if (grid.belong > 0 && grid.belong != belong)
        {
            foreach (SoldierModel soldier in grid.GetGridContain<SoldierModel>())
            {
                //判断是否是敌人
                if (soldier != null && soldier.nowStatus != null && soldier.nowStatus.commander.belong != belong)
                {
                    //接触，开战
                    AddEngage(soldier.nowStatus.commander);
                }
            }
            grid.belong = 0;
        }
        else
        {
            grid.belong = belong;
        }
    }

    /// <summary>
    /// 增加交战
    /// </summary>
    void AddEngage(CommandUnit enermy)
    {
        if (TroopState == TroopState.FLEE) return;
        engageSet.Add(enermy);
        if (focusOnTarget > 0f && enermy != attackTarget) return;
        if (TroopState != TroopState.ENGAGING)
        {
            if (IsRangeAttackAble)
            {
                //远程与跟自己交战的反击 被迫近战
                if (enermy.engageSet.Contains(this))
                {
                    TroopState = TroopState.ENGAGING;
                }
            }
            else
            {
                if (TroopState != TroopState.ENGAGING && attackTarget == null)
                {
                    attackTarget = enermy;
                    UnitControlManager.instance.TroopMoveToTargetPosition(attackTarget.lastPosition, this);
                }
                TroopState = TroopState.ENGAGING;
                if (enermy.IsRangeAttackAble)
                {
                    enermy.AddEngage(this);
                }
            }
        }
        if (false)
            TriggerCommandToAggressive();
        if (attackTarget == null)
        {
            attackTarget = enermy;
        }
        else
        {
            //目标单位不是在战斗而是在撤离
            if (attackTarget.TroopState == TroopState.MOVING)
            {
                attackTarget = enermy;
            }
        }
        if (!enermy.engageSet.Contains(this))
        {
            enermy.AddEngage(this);
        }
    }

    /// <summary>
    /// 获得状态补正
    /// </summary>
    /// <returns></returns>
    public int GetCommandStateHitFix()
    {
        int result = 0;
        if (isBeFlank)
        {
            if (!GetBoolValue(Constant_AttributeString.STATUS_RESIST_FLANK))
                result += -8;
            if (GetBoolValue(Constant_AttributeString.STATUS_BATTLE_FUROR))
                result += 4;
        }
        if (GetBoolValue(Constant_AttributeString.STATUS_REVENGE_ANGER))
        {
            float livePercent = (float)aliveCount / (float)entityData.maxNum;
            livePercent = Mathf.Clamp(1f - (livePercent - 0.3f) / 0.7f, 0f, 1f);
            result += (int)(8 * livePercent);
        }
        if (GetBoolValue(Constant_AttributeString.STATUS_INTERSPERSED_TROUNCE))
        {
            if (attackTarget != null && attackTarget.isBeFlank)
            {
                result += 4;
            }
        }
        return result;
    }

    public int GetCommandStateDefendFix()
    {
        int result = 0;
        if (isBeFlank)
        {
            if (!GetBoolValue(Constant_AttributeString.STATUS_RESIST_FLANK))
                result += -8;
            if (GetBoolValue(Constant_AttributeString.STATUS_BATTLE_FUROR))
                result += 4;
        }
        //战意低迷时被杀
        if (belong != BattleManager.instance.controlBelong)
        {
            if (BattleManager.instance.enermyBattleWill < 0.4f)
            {
                result += -6;//战意低迷易受攻击
            }
        }

        return result;
    }

    void UpdateRecordDamage(float deltaTime)
    {
        float newTime = 0f;
        for (int i = damageRecordList.Count - 1; i >= 0; i--)
        {
            newTime = damageRecordList[i].Item3 - deltaTime;
            if (newTime <= 0f)
            {
                damageRecordList.RemoveAt(i);
            }
            else
            {
                var data = damageRecordList[i];
                data.Item3 = newTime;
                damageRecordList[i] = data;
            }
        }
        //判定当前状态有无被‘侧袭’‘夹击’
        //同时出现在EngageList与伤害列表里，则判定有
        if (TroopState == TroopState.ENGAGING && engageSet.Count > 1)
        {
            if (attackTarget != null)
            {
                troopfaceTo = (attackTarget.lastPosition - lastPosition);
            }
            int hasAttacked = 0;
            bool isBySide = false;
            foreach (var engageTarget in engageSet)
            {
                if (HasBeenDamaged(engageTarget))
                {
                    hasAttacked++;
                    float attackFrom = GraphicUtil.GetPointDegree(lastPosition.XZ(), GetTroopFaceTo(), engageTarget.lastPosition.XZ());
                    if (attackFrom > flankDegree || attackFrom < -flankDegree)
                    {
                        isBySide = true;
                    }
                }
            }
            if (isBySide && hasAttacked >= 2)
            {
                isBeFlank = true;
            }
            else
            {
                isBeFlank = false;
            }
        }
        else
        {
            isBeFlank = false;
        }
    }

    bool HasBeenDamaged(CommandUnit otherCommand)
    {
        return damageRecordList.Find((record) => record.Item1 == otherCommand) != default;
    }

    Vector2 GetTroopFaceTo()
    {
        return new Vector2(troopfaceTo.x, troopfaceTo.z);
    }

    public void RecordDamageFrom(CommandUnit command, int damage)
    {
        damageRecordList.Add((command, damage, MAX_RECORD_TIME));
    }

    public void BeenShoot(CommandUnit command)
    {
        if (TroopState != TroopState.DESTROYED)
        {
            if (attackTarget != null && attackTarget.TroopState != TroopState.ENGAGING)
            {
                if (command.IsRangeAttackAble && (!attackTarget.IsRangeAttackAble || attackTarget.TroopState != TroopState.SHOOTING))
                {
                    attackTarget = command;
                }
            }
            else if (attackTarget == null)
            {
                attackTarget = command;
            }
        }
    }

    /// <summary>
    /// 触发开始主动进攻
    /// </summary>
    public void TriggerCommandToAggressive()
    {
        if (hasBeenTriggerAggressive) return;
        hasBeenTriggerAggressive = true;
        aggressiveTrigger?.Trigger(this);
    }
    /// <summary>
    /// 对每个存活单位进行操作
    /// </summary>
    /// <param name="action"></param>
    public void ForEachAlive(System.Action<SoldierStatus> action)
    {
        for (int i = 0; i < aliveCount; i++)
        {
            action?.Invoke(troopsData[i]);
        }
    }
    /// <summary>
    /// 脱战行为
    /// </summary>
    void TroopEscapeBattle()
    {
        foreach (var engageTarget in engageSet)
        {
            if (engageTarget.IsRangeAttackAble)
            {
                engageTarget.engageSet.Remove(this);
                if (engageTarget.engageSet.Count == 0)
                {
                    engageTarget.TroopState = TroopState.WAITING;
                }
            }
        }
        foreach (var unit in troopsData)
        {
            unit.EscapeBattle();
        }
    }
    /// <summary>
    /// 结束移动
    /// </summary>
    bool IsEndMoving()
    {
        int movingEndNum = 0;
        for (int i = 0; i < aliveCount; i++)
        {
            if (!troopsData[i].model.IsMoving)
            {
                movingEndNum++;
            }
        }
        if ((float)movingEndNum / (float)aliveCount > 0.8f && moveList.Count > 1) return true;
        if (movingEndNum == aliveCount) return true;
        return false;
    }
    /// <summary>
    /// 失去交战者
    /// </summary>
    public void EngagedTroopRemove(CommandUnit commandUnit)
    {
        if (TroopState == TroopState.DESTROYED) return;
        bool needCheck = false;
        if (attackTarget == commandUnit)
        {
            attackTarget = null;
            needCheck = true;
        }
        if (engageSet.Contains(commandUnit))
        {
            engageSet.Remove(commandUnit);
            needCheck = true;
        }
        if (needCheck && attackTarget == null && canSetTarget)
        {
            if (engageSet.Count > 0)
            {
                foreach (var e in engageSet)
                {
                    if (e.troopState != TroopState.DESTROYED)
                        attackTarget = e;
                    break;
                }
            }
            else
            {
                //部队重整
                if (aliveCount > 0)
                {
                    TroopState = TroopState.WAITING;
                    UnitControlManager.instance.TroopReformation(this);
                }
            }
        }
    }

    /// <summary>
    /// 外部调用 更新判断部队进攻关系的建立
    /// </summary>
    public void UpdateAttackRelation(bool forceRenew = false)
    {
        if (attackTarget == null)
        {
            if (engageSet.Count == 0)
                return;
            else if (focusOnTarget <= 0f)
            {
                attackTarget = ArrayUtil.GetHashSetFirst(engageSet);
            }
            else
            {
                return;
            }
        }
        else
        {
            if (attackTarget.aliveCount == 0)
            {
                attackTarget = null;
                TroopEscapeBattle();
            }
        }
    }
    /// <summary>
    /// 更新单位强制进攻目标
    /// </summary>
    void UpdateSoldierFocusTarget()
    {
        for (int i = 0; i < aliveCount; i++)
        {
            if (troopsData[i].focusTarget != null)//如果存在敌人 但敌人不在可攻击范围+追踪范围内——则尝试转火。//当前追踪范围设定为0f
            {
                if (troopsData[i].isActionAttacking) continue;
                if (troopsData[i].focusTarget.model == null || !troopsData[i].IsTargetInAttackRange())
                {
                    troopsData[i].focusTarget = null;
                }
            }
            if (!IsRangeAttackAble)
            {
                if (troopsData[i].focusTarget == null)
                {
                    float minDistance = attackRange + 0.5f; //切换成最小攻击距离//float.MaxValue;
                    float temp;
                    SoldierStatus target = null;

                    foreach (var enermy in GetAllEngageEnermyList())
                    {
                        temp = UnitControlManager.instance.GetSoldierSpaceDistance(troopsData[i].model, enermy.model);
                        if (temp < minDistance)
                        {
                            minDistance = temp;
                            target = enermy;
                        }
                    }
                    if (target != null)
                        troopsData[i].AddAttackTarget(target);
                    else
                    {
                        //依旧没有攻击目标——尝试队形重整
                        //return;//不进行主动冲锋
                        //minDistance = attackRange + 0.1f; //切换成最小攻击距离//float.MaxValue;
                        minDistance = float.MaxValue;
                        foreach (var enermy in GetFocusAttackEnermyList())
                        {
                            temp = UnitControlManager.instance.GetSoldierSpaceDistance(troopsData[i].model, enermy.model);
                            if (temp < minDistance)
                            {
                                minDistance = temp;
                                target = enermy;
                            }
                        }
                        if (target != null)
                            troopsData[i].AddAttackTarget(target);
                    }
                    //TODO:修改降低focus级别
                }
            }
            else
            {
                float minDistance = troopsData[i].nowAttackRange;//切换成最小攻击距离
                var list = GetAllEngageEnermyList();
                int flag = 1;
                SoldierStatus target = null;
                for (int j = 0; j < list.Count; j++)
                {
                    var distance = UnitControlManager.instance.GetSoldierSpaceDistance(troopsData[i].model, list[j].model);
                    if (distance <= minDistance)
                    {
                        if (Random.Range(0, flag) == 0)
                        {
                            target = list[j];
                        }
                        flag++;
                    }
                }
                if (target != null)
                {
                    troopsData[i].AddAttackTarget(target, true);
                }
                else
                {
                    if (list.Count > 0)
                        troopsData[i].AddAttackTarget(list[Random.Range(0, list.Count)], true);
                }
                //从攻击范围中选取可攻击到的目标。没有就随机
                //进行距离判定/格子从属判断：小规模离散与大规模互殴
            }
        }
        tempList = null;
    }
    List<SoldierStatus> tempList;
    public List<SoldierStatus> GetAllEngageEnermyList()
    {
        if (tempList != null) return tempList;
        tempList = new List<SoldierStatus>();

        foreach (var engageTarget in engageSet)
        {
            for (int i = 0; i < engageTarget.aliveCount; i++)
            {
                tempList.Add(engageTarget.troopsData[i]);
            }
        }
        return tempList;
    }
    public List<SoldierStatus> GetFocusAttackEnermyList()
    {
        var result = new List<SoldierStatus>();
        if (attackTarget == null)
        {
            return result;
        }
        for (int i = 0; i < attackTarget.aliveCount; i++)
        {
            result.Add(attackTarget.troopsData[i]);
        }
        return result;
    }


    void CentorDrag()
    {
        ForEachAlive((s) =>
        {
            if (s.nowState == 1) return;
            Vector3 target = Vector3.zero;
            //不控制在进攻逻辑中的单位
            if (s.isActionAttacking || s.focusTarget != null)
            {
                return;
            }
            //只控制移动的单位
            if (s.controlDelta > 0)
            {
                s.controlDelta -= Time.deltaTime;
            }
            else
            {
                s.controlDelta = Random.Range(0.2f, 0.3f);
            }
            target = lastPosition;
            if (attackTarget != null)
            {
                target = Vector3.MoveTowards(target, attackTarget.lastPosition, 0.4f);
            }
            Vector3 now = s.model.actionModel.transform.position;
            s.model.actionModel.m_Agent.Move((target - now).normalized * 0.01f);
            //if ((s.model.actionModel.m_Agent.destination - s.model.lastPosition).magnitude <)
        });
    }

    void UpdateCentorSoldier(Vector3 pos)
    {
        SoldierStatus result = null;
        float minDistance = float.MaxValue;
        for (int i = 0; i < aliveCount; i++)
        {
            float distance = Vector3.Distance(troopsData[i].model.transform.position, lastPosition);
            if (distance < minDistance)
            {
                result = troopsData[i];
                minDistance = distance;
            }
        }
        if (result != null)
        {
            centerSoldier = result;
        }
        //centerSoldier.model.transform.localScale = Vector3.one * 3f;
    }
    float timm = 0;
    public void UpdateCenterSoldier()
    {
        if (centerSoldier == null && timm > 0.1f)
        {
            UpdateCentorSoldier(lastPosition);
            //更替队形时调整中间者的Update
        }
        else if (centerSoldier != null && centerSoldier.isDead)
        {
            UpdateCentorSoldier(lastPosition);
        }
        timm += Time.deltaTime;


    }


    /// <summary>
    /// 轮询时间进行指令的执行
    /// </summary>
    public void UpdateCommand(float deltaTime)
    {
        UpdateCenterSoldier();

        if (TroopState == TroopState.ENGAGING)
        {
            CentorDrag();
        }
        if (TroopState == TroopState.MOVING)
        {
            CorrectCurrentPosition();
        }
        //if (TroopState == TroopState.WAITING)
        //{
        //    UnitControlManager.instance.TroopReformation(this);
        //}
        ForEachAlive((s) =>
        {
            s.UpdateNow(deltaTime);
        });
        UpdateRecordDamage(deltaTime);
        focusOnTarget -= deltaTime;
        lastPosition = GetMiddlePosition();
        if (TroopState == TroopState.DESTROYED || aliveCount == 0)
        {
            return;
        }
        else if (troopState != TroopState.FLEE && ((float)CommandUnit_NowHP / (float)CommandUnit_TotalHP <= 0.3f))
        {
            //测试
            //troopState = TroopState.FLEE;
            //focusOnTarget = true;
            //UnitControlManager.instance.TroopMoveToTargetPosition(BattleManager.instance.nowBattleMap.enemyFleePoint.position, this);
            //return;
        }

        ForEachAlive((s) =>
        {
            s.OnUpdate(deltaTime);
        });

        if (!canSetTarget) return;
        commandCount++;
        if (commandCount == MAX_COMMAND_COUNT)
        {
            commandCount = 0;
        }
        else
        {
            return;
        }
        bool isAutoCommand = !controlAble && focusOnTarget <= 0f;
        if (!isAutoCommand)
        {
            //if (UnitControlManager.instance.GetCommand(belong).autoChargeSet.Contains(entityData))
            if (BattleManager.instance.autoCharge)
            {
                isAutoCommand = true;
            }
        }
        if (TroopState == TroopState.FLEE)
        {
            if (IsEndMoving())
            {
                Debug.LogError("FLEE_OVER");
            }
            return;
        }
        if (isAutoCommand)
        {
            if (entityData.unitType == UnitType.CASTER && (TroopState == TroopState.TAKE_OVER || TroopState == TroopState.WAITING || troopState == TroopState.MOVING))
            {
                var command = UnitControlManager.instance.GetEnermyInRange(this, 20f);
                if (command != null)
                {
                    //计算方向 改变部队的移动方向
                    Vector3 escapeDirection = (lastPosition - command.lastPosition).normalized;
                    var target = lastPosition + escapeDirection * 3;
                    //target.x = Mathf.Clamp(target.x, -75f, 75f);
                    //target.z = Mathf.Clamp(target.x, -75f, 75f);
                    DesinateTargetTroop(target);
                }

            }
            else if (TroopState == TroopState.TAKE_OVER || TroopState == TroopState.WAITING)
                SetTarget(UnitControlManager.instance.GetEnermyInRange(this, hasBeenTriggerAggressive ? 150f : 20f), false);
            //暂时索敌玩家部队并部署攻击 
        }

        if (isRangeAttackIntension && (TroopState == TroopState.TAKE_OVER || TroopState == TroopState.WAITING))//isAutoCommand && 
        {
            if (attackTarget == null)
                SetTarget(UnitControlManager.instance.GetEnermyInRange(this, attackRange), false);
            //暂时索敌玩家部队并部署攻击 
        }

        if (TroopState == TroopState.ATTACK_MOVING)
        {
            //一定周身索敌范围——————索敌范围
            if (attackTarget == null)
            {
                SetTarget(UnitControlManager.instance.GetEnermyInRange(this, 100f));
            }
        }
        if (TroopState == TroopState.MOVING)
        {
            needntCorrectCurrentPosition = false;
            if (IsEndMoving())
            {
                if (focusOnTarget > 0f) engageSet.Clear();
                if (engageSet.Count == 0)
                {
                    TroopState = TroopState.WAITING;
                    //if (moveStartAliveCount != aliveCount)
                    //{
                    //}
                    UnitControlManager.instance.TroopReformation(this);
                }
                else
                {
                    TroopState = TroopState.ENGAGING;
                    attackTarget = ArrayUtil.GetHashSetFirst(engageSet);
                    UnitControlManager.instance.TroopMoveToTargetPosition(attackTarget.lastPosition, this);
                }
                needntCorrectCurrentPosition = true;


            }
        }
        //战斗关系建立，进攻防守自行进行
        if (TroopState != TroopState.ENGAGING && TroopState != TroopState.SHOOTING)
        {
            if (attackTarget != null)
            {
                UnitControlManager.instance.TroopMoveToTargetPosition(attackTarget.lastPosition, this);
                if (IsRangeAttackAble)
                {
                    //如果在两者之间的攻击范围内则进行覆盖射击
                    float distance = Vector3.Distance(lastPosition, attackTarget.lastPosition);
                    if (distance < attackRange)
                    {
                        UnitControlManager.instance.TroopReformation(this);
                        TroopState = TroopState.SHOOTING;
                    }
                }
                //被动遭受攻击时进入的交战逻辑
            }
            if (focusOnTarget < 0f && engageSet.Count > 0 && entityData.unitType != UnitType.CASTER)
            {
                TroopState = TroopState.ENGAGING;
                UpdateAttackRelation();
            }
        }
        else if (TroopState == TroopState.ENGAGING)
        {
            if (attackTarget == null)
            {
                if (engageSet.Count > 0)
                {
                    foreach (var e in engageSet)
                    {
                        if (e.troopState != TroopState.DESTROYED)
                            attackTarget = e;
                        break;
                    }
                }
                else
                {
                    if (attackTarget == null)
                    {
                        TroopState = TroopState.WAITING;
                        UnitControlManager.instance.TroopReformation(this);
                    }
                }
            }
            else
            {
                if (!attackTarget.engageSet.Contains(this))
                {
                    attackTarget.AddEngage(this);
                }
                engageSet.RemoveWhere((command) => command.aliveCount == 0);
                if (engageSet.Count == 0)
                {
                    TroopState = TroopState.WAITING;
                }
                UpdateAttackRelation();
            }
        }
        else if (TroopState == TroopState.SHOOTING)
        {
            if (attackTarget == null)
            {
                TroopState = TroopState.WAITING;
                UnitControlManager.instance.TroopReformation(this);
            }
            else
            {
                //判断转火与否
                lastAttackTarget = attackTarget;
                engageSet.Add(attackTarget);
            }
        }
        //每个单位将设立向最临近的战斗方目标。除非是移动脱离
        if (TroopState != TroopState.MOVING)
        {
            UpdateSoldierFocusTarget();
        }
    }

    void DesinateTargetTroop(Vector3 centerPosition)
    {
        TroopState = TroopState.MOVING;
        if (InputManager.Instance.isShiftMulAble)
        {
            moveList.Add(centerPosition);
        }
        else
        {
            moveList.Clear();
            moveList.Add(centerPosition);
        }
        DesinateTargetTroop();

    }
    CommandUnit lastAttackTarget;
    Vector3 tempResult;
    public Vector3 GetMiddlePosition()
    {
        if (tempResult == Vector3.zero)
        {
            int count = 0;
            for (int i = 0; i < aliveCount; i++)
            {
                count++;
                tempResult += (troopsData[i].model.transform.position - tempResult) / count;

            }
            CoroutineManager.StartFrameDelayedCoroutine(() =>
            {
                tempResult = Vector3.zero;
            });
        }
        return tempResult;
    }

    //高级AI可以有效进行火力分配
    public bool SetTarget(CommandUnit target, bool force = true)
    {
        if (attackTarget == target) return true;
        if (attackTarget != null && force)
        {
            engageSet.Remove(attackTarget);
            attackTarget = null;
            TroopEscapeBattle();
        }
        if (CanAttackTargetTroop(target))
        {
            attackTarget = target;
            troopfaceTo = (target.GetMiddlePosition() - GetMiddlePosition()).normalized;
            return true;
        }
        return false;
    }

    public void RemoveAttackTarget()
    {
        attackTarget = null;
        engageSet.Clear();
    }

    private bool CanAttackTargetTroop(CommandUnit unit)
    {
        if (unit == null) return false;
        if (belong != unit.belong)//TODO额外增加逻辑判定
        {
            return true;
        }
        return false;
    }

    public void AddStatus(StandardStatus standardStatus)
    {
        status.Add(StatusManager.Instance.RequestStatus(standardStatus, this, belong));
    }

    public void EndStatus(List<EntityStatus> status, int belong = -1)
    {
        foreach (var target in status)
        {
            target.StatusActiveEffectReverseExecution();
            StatusManager.Instance.RemoveStatus(target);
        }
        status.Clear();
    }

    #region QueueMove

    public Vector3 oldLastPos;//最开始移动时的lastpos与moveList[0]一起做出一个直线

    public Vector3 correctedValue;//点击移动时lastpos和centersolider之间的v3偏移值 再计算行进路程当中当前应该在的位置时需要加上偏移值

    public float moveTime;
    public bool needntCorrectCurrentPosition;
    public void CorrectCurrentPosition()
    {
        if (needntCorrectCurrentPosition)
            return;

        var result = FormationUtil.GetFormationsWithCenter(aliveCount, moduleSize, spaceSize, NowWidth);
        result = FormationUtil.FormationChangeFace(result, troopfaceTo);
        //CalculateCorrectedValue();
        int[] resultFlagArray = FormationUtil.ResetPosition(this, formation, troopfaceTo);
        NavMeshHit hit;
        moveTime += Time.deltaTime;
        moveTime = Mathf.Clamp(moveTime, 0, Vector3.Distance(oldLastPos, moveList[0]) / centerSoldier.model.actionModel.m_Agent.speed);
        for (int i = 0; i < resultFlagArray.Length; i++)
        {

            SoldierStatus soldier = GetSoldier(resultFlagArray[i]);
            if (!soldier.model.IsMoving || soldier.nowState != 0)
            {
                continue;
            }
            result[i] += oldLastPos + (centerSoldier.model.actionModel.m_Agent.speed * moveTime) * (moveList[0] - oldLastPos).normalized;
            //攻击状态且已经出现阻挡关系的单位不影响
            if (NavMesh.SamplePosition(result[i], out hit, 30f, 1))
            {
                Vector3 targetPos = (hit.position - soldier.model.transform.position);
                targetPos = targetPos.normalized * 0.01f;
                soldier.model.actionModel.m_Agent.Move(targetPos);
                //基于当前位置做一个朝向当前应该在的位置的力 力的大小基于当前位置与应该在的位置的距离而定
            }
        }
    }





    public void CorrectCurrentPositionWithAlignLeftTop()
    {
        if (needntCorrectCurrentPosition)
            return;

        //var result = FormationUtil.GetFormationsWithFirstLine(aliveCount, moduleSize, spaceSize, NowWidth);
        var result = FormationUtil.GetFormationsWithCenter(aliveCount, moduleSize, spaceSize, NowWidth);
        result = FormationUtil.FormationChangeFace(result, troopfaceTo);
        //CalculateCorrectedValue();
        int[] resultFlagArray = FormationUtil.ResetPosition(this, formation, troopfaceTo);
        NavMeshHit hit;
        moveTime += Time.deltaTime;
        moveTime = Mathf.Clamp(moveTime, 0, Vector3.Distance(oldLastPos, moveList[0]) / centerSoldier.model.actionModel.m_Agent.speed);
        for (int i = 0; i < resultFlagArray.Length; i++)
        {

            SoldierStatus soldier = GetSoldier(resultFlagArray[i]);
            if (!soldier.model.IsMoving)
            {
                continue;
            }
            result[i] += oldLastPos + (centerSoldier.model.actionModel.m_Agent.speed * moveTime) * (moveList[0] - oldLastPos).normalized;

            //攻击状态且已经出现阻挡关系的单位不影响
            if (NavMesh.SamplePosition(result[i], out hit, 30f, 1))
            {

                Vector3 targetPos = (hit.position - soldier.model.transform.position);
                targetPos = targetPos.normalized * 0.01f;
                soldier.model.actionModel.m_Agent.Move(targetPos);
            }

        }

    }


    public void CalculateCorrectedValue()
    {

        float t = Vector3.Dot(centerSoldier.model.transform.position - oldLastPos, moveList[0] - oldLastPos) / (moveList[0] - oldLastPos).sqrMagnitude;
        correctedValue = oldLastPos + t * (moveList[0] - oldLastPos);
    }




    #endregion
    public void ChangeFaceTo(Vector3 vector)
    {
        for (int i = 0; i < aliveCount; i++)
        {
            troopsData[i].model.ChangeFaceTo(vector);
        }
    }
    public int[] GetAllTroopFlagList()
    {
        int[] result = new int[aliveCount];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = i;
        }
        return result;
    }
    public Vector3[] GetAllTroopPositionList()
    {
        Vector3[] result = new Vector3[aliveCount];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = troopsData[i].model.transform.position;
        }
        return result;
    }

    public void LostUnit(SoldierStatus soldier)
    {
        soldier.nowState = -1;
        aliveCount--;
        //活人 位置交换——死人分区
        troopsData.Swap(aliveCount, troopsData.IndexOf(soldier));
        //向上反馈
        //eventModel.FireEvent(GameEventType.SOLDIER_DIE, "SoldierStatus", soldier, "Num", 1);
        if (realityTroop != null && !soldier.isDead)
        {
            realityTroop.LostNum(1);
        }
        //断掉与其之间的联系
        if (aliveCount == 0 && TroopState != TroopState.DESTROYED)
        {
            OnDestroy();
        }
    }

    void OnDestroy()
    {
        foreach (var target in status)
        {
            target.StatusActiveEffectReverseExecution();
            StatusManager.Instance.RemoveStatus(target);
        }
        status.Clear();
        UnitControlManager.instance.TroopDestroyed(this);
        aggressiveTrigger?.Trigger(this);
    }

    public void RaiseUnit(SoldierStatus targetSoldier, string targetSpeciesType = null)
    {
        if (aliveCount == 0)
        {
            troopState = TroopState.WAITING;
            UnitControlManager.instance.TroopReverse(this);
        }
        if (targetSoldier.nowState != 1)
        {
            if (troopsData[aliveCount] != targetSoldier)
            {
                int index = troopsData.IndexOf(targetSoldier);
                ArrayUtil.Swap(troopsData, aliveCount, index);
            }
            targetSoldier.nowState = 1;
            targetSoldier.model.ActivateAllEffect();
            CoroutineManager.instance.StartCoroutine(RaiseUnitCor(targetSoldier, (s) =>
            {
                if (!string.IsNullOrEmpty(targetSpeciesType))
                {
                    s.ShapeShift(targetSpeciesType);
                }
                if (GameManager.instance.CheckTechAble("RebirthFrenzy"))
                {
                    var standardStatus = StatusManager.Instance.GetStatus("RebirthFrenzy");
                    s.status.Add(StatusManager.Instance.RequestStatus(standardStatus, s, s.commander.belong));
                }
                if (GameManager.instance.CheckTechAble("RebirthSpeedy"))
                {
                    var standardStatus = StatusManager.Instance.GetStatus("RebirthSpeedy");
                    s.status.Add(StatusManager.Instance.RequestStatus(standardStatus, s, s.commander.belong));
                }
            }));
            aliveCount++;
        }
    }

    public void RaiseAndSeperateSoldier(SoldierStatus targetSoldier, string targetSpeciesType = null)
    {
        if (targetSoldier.nowState != 1)
        {
            if (troopsData[aliveCount] != targetSoldier)
            {
                int index = troopsData.IndexOf(targetSoldier);
                ArrayUtil.Swap(troopsData, aliveCount, index);
            }
            targetSoldier.nowState = 1;
            CoroutineManager.instance.StartCoroutine(RaiseUnitCor(targetSoldier, (s) =>
            {
                if (!string.IsNullOrEmpty(targetSpeciesType))
                {
                    s.ShapeShift(targetSpeciesType);
                }
            }));
        }
    }

    //public void RaiseUnit(int number)
    //{
    //    while (number > 0 && aliveCount < troopsData.Count)
    //    {
    //        if (troopsData[aliveCount].nowState != 1)
    //        {
    //            troopsData[aliveCount].nowState = 1;
    //            CoroutineManager.instance.StartCoroutine(RaiseUnitCor(troopsData[aliveCount]));
    //            aliveCount++;
    //            number--;
    //        }
    //    }
    //}

    IEnumerator RaiseUnitCor(SoldierStatus soldier, System.Action<SoldierStatus> onRise = null)
    {
        if (realityTroop != null)
        {
            realityTroop.ReinforceNumber(1);
        }
        var g = GameObjectPoolManager.Instance.Spawn("Prefab/DeadRaiseEffect", BattleManager.instance.transform);
        g.transform.position = soldier.model.transform.position;
        g.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        //只临时复生
        soldier.ReverseDeath();
        onRise?.Invoke(soldier);
        soldier.SetMoveTargetPos(lastPosition);//归队移动
        yield return new WaitForSeconds(1f);
        g.SetActive(false);
        GameObjectPoolManager.Instance.Recycle(g, "Prefab/DeadRaiseEffect");
    }

    public SoldierStatus SeperateUnit(Vector3 closedToPos)
    {
        SoldierStatus result = troopsData[0];
        float minDistance = Vector3.Distance(result.model.lastPosition, closedToPos);
        ForEachAlive((s) =>
        {
            float distance = Vector3.Distance(s.model.lastPosition, closedToPos);
            if (distance < minDistance)
            {
                result = s;
                minDistance = distance;
            }
        });
        aliveCount--;
        troopsData.Remove(result);
        return result;
    }

    public SoldierStatus SeperateUnit(SoldierStatus target)
    {
        if (target.nowState != 1)
            aliveCount--;
        troopsData.Remove(target);
        return target;
    }

    public void ActionToAllUnit(System.Action<SoldierStatus> action)
    {
        foreach (var s in troopsData)
        {
            action?.Invoke(s);
        }
    }
}
public enum TroopState
{
    TAKE_OVER,//接管
    WAITING,//等待
    MOVING,//移动
    ATTACK_MOVING,//搜寻攻击移动中
    ENGAGING,//战斗
    SHOOTING,//战斗
    FLEE,//撤离
    CASTING,//施法
    DESTROYED
    //施法？
}