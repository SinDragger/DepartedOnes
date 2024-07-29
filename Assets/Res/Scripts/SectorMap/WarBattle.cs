using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战争战场
/// </summary>
public class WarBattle
{
    public int winnerBelongTo = -1;
    public string targetWarBattleMap;
    public List<LegionControl> combatLegions = new List<LegionControl>();
    public Dictionary<LegionControl, int> originNumber = new Dictionary<LegionControl, int>();
    public Dictionary<TroopControl, int> originTroopNumber = new Dictionary<TroopControl, int>();
    public Vector2 centorPos;
    public WarBattle(params LegionControl[] legions)
    {
        combatLegions.AddRange(legions);
        centorPos = Vector2.zero;
        for (int i = 0; i < legions.Length; i++)
        {
            centorPos += legions[i].position;
            originNumber[legions[i]] = legions[i].TotalNum;
            for (int j = 0; j < legions[i].troops.Count; j++)
            {
                originTroopNumber[legions[i].troops[j]] = legions[i].troops[j].nowNum;
            }
        }
        centorPos /= legions.Length;
    }

    public List<LegionControl> GetPlayerLegions()
    {
        List<LegionControl> result = null;
        for (int i = 0; i < combatLegions.Count; i++)
        {
            if (combatLegions[i].belong == BattleManager.instance.controlBelong)
            {
                if (result == null) result = new List<LegionControl>();
                result.Add(combatLegions[i]);
            }
        }
        return result;
    }
    public List<LegionControl> GetEnermyLegions()
    {
        List<LegionControl> result = null;
        for (int i = 0; i < combatLegions.Count; i++)
        {
            if (combatLegions[i].belong != BattleManager.instance.controlBelong)
            {
                if (result == null) result = new List<LegionControl>();
                result.Add(combatLegions[i]);
            }
        }
        return result;
    }

    public void InitAutoWarBattle()
    {
        var leftLegion = combatLegions.Find((l) => l.belong == GameManager.instance.belong);
        if (leftLegion == null) leftLegion = combatLegions[0];
        var rightLegion = combatLegions.Find((l) => l != leftLegion);
        behaviourChain = new BehaviourChain();
        if (battleTroopsList == null)
        {
            battleTroopsList = new List<List<AutoBattleTroop>>(8);
            for (int i = 0; i < 8; i++)
            {
                battleTroopsList.Add(new List<AutoBattleTroop>(8));
            }
            //将两方的全塞入
            int flag = 3;
            for (int i = 0; i < 4; i++)
            {
                if (leftLegion.stackTroops.ContainsKey(i))
                {
                    if (InitAutoTroop(flag, leftLegion.stackTroops[i], leftLegion.belong))
                    {
                        flag--;
                    }
                }
            }
            flag = 4;
            for (int i = 0; i < 4; i++)
            {
                if (rightLegion.stackTroops.ContainsKey(i))
                {
                    if (InitAutoTroop(flag, rightLegion.stackTroops[i], rightLegion.belong))
                    {
                        flag++;
                    }
                }
            }
        }
        RefreshBattleTroopsPos();
    }

    private bool InitAutoTroop(int flag, List<TroopControl> troopList, int belong)
    {
        bool result = false;
        battleTroopsList[flag] = new List<AutoBattleTroop>();
        for (int i = 0; i < troopList.Count; i++)
        {
            if (troopList[i].nowNum > 0)
            {
                result = true;
                var newTroop = new AutoBattleTroop(this, troopList[i], belong);
                battleTroopsList[flag].Add(newTroop);
                newTroop.lineFlag = flag;
            }
        }
        return result;
    }

    /// <summary>
    /// 战时两方的距离
    /// </summary>
    public float nowDistance => Mathf.Clamp(100f - behaviourChain.nowTime, 0, 100f);
    public List<List<AutoBattleTroop>> battleTroopsList;
    /// <summary>
    /// 行为链
    /// </summary>
    public BehaviourChain behaviourChain;
    /// <summary>
    /// 自动模式下的状态更新
    /// </summary>
    public void UpdateAutoResult(float deltaTime)
    {
        //速度放缓
        deltaTime /= 10f;
        behaviourChain.CheckFutureBehaviours(deltaTime);
        foreach(var legion in combatLegions)
        {
            //维持UI刷新可能
            legion.uiDataChanged = true;
        }
    }

    /// <summary>
    /// 刷新战场位置（全部）
    /// </summary>
    void RefreshBattleTroopsPos()
    {
        int delta = 0;
        for (int i = 0; i < battleTroopsList.Count; i++)
        {
            int max = battleTroopsList[i].Count;
            if (max % 2 == 0)
            {
                delta = (max - 1);
            }
            else
            {
                delta = ((max / 2) * 2);
            }
            for (int j = 0; j < battleTroopsList[i].Count; j++)
            {

                battleTroopsList[i][j].rowFlag = j * 2 - delta;
            }
        }
    }

    /// <summary>
    /// 刷新战场位置（某行）
    /// </summary>
    void RefreshBattleTroopsPos(int line)
    {
        int delta = 0;
        int max = battleTroopsList[line].Count;
        if (max % 2 == 0)
        {
            delta = (max - 1);
        }
        else
        {
            delta = ((max / 2) * 2);
        }
        for (int j = 0; j < battleTroopsList[line].Count; j++)
        {

            battleTroopsList[line][j].rowFlag = j * 2 - delta;
        }
    }

    /// <summary>
    /// 脱离战斗
    /// </summary>
    /// <param name="troop"></param>
    public void LeaveBattle(AutoBattleTroop troop)
    {
        battleTroopsList[troop.lineFlag].Remove(troop);
        RefreshBattleTroopsPos(troop.lineFlag);
        if (battleTroopsList[troop.lineFlag].Count == 0)
        {
            LineClear(troop.lineFlag);
        }
    }

    /// <summary>
    /// 部队进行了移动
    /// </summary>
    public event System.Action<AutoBattleTroop, int, int> troopMoved;

    /// <summary>
    /// 行补位
    /// </summary>
    /// <param name="flag"></param>
    void LineClear(int flag)
    {
        //行清空
        if (flag == 3)
        {
            for (int i = 2; i >= 0; i--)
            {
                for (int j = 0; j < battleTroopsList[i].Count; j++)
                {
                    battleTroopsList[i][j].SetAction(1);
                }
            }
            CheckGameWin(true);
        }
        else if (flag == 4)
        {
            for (int i = 5; i < 8; i++)
            {
                for (int j = 0; j < battleTroopsList[i].Count; j++)
                {
                    battleTroopsList[i][j].SetAction(1);
                }
            }
            CheckGameWin(false);
        }
    }

    void CheckGameWin(bool isleft)
    {
        if (isleft)
        {
            for (int i = 0; i < 4; i++)
            {
                if (battleTroopsList[i].Count != 0) return;
            }
            GameComplete(true);
        }
        else
        {
            for (int i = 4; i < 8; i++)
            {
                if (battleTroopsList[i].Count != 0) return;
            }
            GameComplete(false);
        }
    }

    void GameComplete(bool rightWin)
    {
        if (rightWin)
        {
            winnerBelongTo = combatLegions[0].belong;
        }
        else
        {
            winnerBelongTo = combatLegions[1].belong;
        }
        LegionManager.Instance.WarComplete(this);

        //自动战斗后结果，没有玩家干涉，直接计算资源后放入地块
        var block = SectorBlockManager.Instance.GetBlock(centorPos);
        int bodyRes = 0;
        foreach (var legion in combatLegions)
        {
            bodyRes += originNumber[legion] - legion.TotalNum;
        }
        //block.pool.ChangeResource(Constant_Resource.Born, bodyRes);
    }

    public void MoveForward(AutoBattleTroop troop)
    {
        int targetIndex = -1;
        List<AutoBattleTroop> targetLine = null;
        int targetLineIndex = 0;
        if (troop.lineFlag < 3)
        {
            targetLineIndex = troop.lineFlag + 1;
        }
        else if (troop.lineFlag > 4)
        {
            targetLineIndex = troop.lineFlag - 1;
        }
        targetLine = battleTroopsList[targetLineIndex];
        targetIndex = targetLine.FindIndex((t) => t.rowFlag == troop.rowFlag - 1);
        if (targetIndex == -1)
        {
            targetIndex = targetLine.FindIndex((t) => t.rowFlag == troop.rowFlag - 2);
        }
        if (targetIndex == -1)
        {
            targetIndex = targetLine.FindIndex((t) => t.rowFlag == troop.rowFlag);
        }
        battleTroopsList[troop.lineFlag].Remove(troop);
        if (targetIndex == -1)
        {
            targetLine.Add(troop);
        }
        else
        {
            targetLine.Insert(targetIndex + 1, troop);
        }
        troopMoved?.Invoke(troop, troop.lineFlag, targetLineIndex);
        troop.lineFlag = targetLineIndex;
        RefreshBattleTroopsPos(targetLineIndex);
    }

    public void DealDamageToOtherSide(AutoBattleTroop attackTroop)
    {
        bool isOdd = battleTroopsList[attackTroop.lineFlag].Count % 2 == 0;
        //定位当前位置
        if (attackTroop.lineFlag <= 3)
        {
            if (AttackTargetLinePos(attackTroop, 4, attackTroop.rowFlag, isOdd))
            {
            }
            else if (attackTroop.isRangeAttack && AttackTargetLinePos(attackTroop, 5, attackTroop.rowFlag, isOdd))
            {
            }
        }
        else
        {
            if (AttackTargetLinePos(attackTroop, 3, attackTroop.rowFlag, isOdd))
            {
            }
            else if (attackTroop.isRangeAttack && AttackTargetLinePos(attackTroop, 2, attackTroop.rowFlag, isOdd))
            {
            }
        }
    }


    //战场宽度限制部队投入发挥
    public bool IsAbleToAttack(AutoBattleTroop attackTroop)
    {
        //定位当前位置
        bool isOdd = battleTroopsList[attackTroop.lineFlag].Count % 2 == 0;
        if (attackTroop.lineFlag <= 3)
        {
            if (TargetLinePosCanAttack(4, attackTroop.rowFlag, isOdd))
            {
                return true;
            }
            else if (attackTroop.isRangeAttack && TargetLinePosCanAttack(5, attackTroop.rowFlag, isOdd))
            {
                return true;
            }
        }
        else
        {
            if (TargetLinePosCanAttack(3, attackTroop.rowFlag, isOdd))
            {
                return true;
            }
            else if (attackTroop.isRangeAttack && TargetLinePosCanAttack(2, attackTroop.rowFlag, isOdd))
            {
                return true;
            }
        }
        return false;
    }

    public bool TargetLinePosCanAttack(int line, int row, bool isOdd)
    {
        if (battleTroopsList[line].Find((troop) => troop.rowFlag == row) != null)
        {
            return true;
        }
        else//形成侧击或
        {
            if (battleTroopsList[line].Count % 2 == 0 == isOdd)//同等
            {
                if (battleTroopsList[line].Find((troop) => troop.rowFlag == row + 2) != null) return true;
                if (battleTroopsList[line].Find((troop) => troop.rowFlag == row - 2) != null) return true;
            }
            else
            {
                if (battleTroopsList[line].Find((troop) => troop.rowFlag == row + 1) != null) return true;
                if (battleTroopsList[line].Find((troop) => troop.rowFlag == row - 1) != null) return true;
            }
            return false;
        }
    }

    public bool AttackTargetLinePos(AutoBattleTroop attackTroop, int line, int row, bool isOdd)
    {
        AutoBattleTroop faceTo = battleTroopsList[line].Find((troop) => troop.rowFlag == row);
        if (faceTo != null)
        {
            attackTroop.AttackAnotherTroop(faceTo);
            return true;
        }
        else//形成侧击或
        {
            AutoBattleTroop up;
            AutoBattleTroop down;
            if (battleTroopsList[line].Count % 2 == 0 == isOdd)//同等
            {
                up = battleTroopsList[line].Find((troop) => troop.rowFlag == row + 2);
                down = battleTroopsList[line].Find((troop) => troop.rowFlag == row - 2);
            }
            else
            {
                up = battleTroopsList[line].Find((troop) => troop.rowFlag == row + 1);
                down = battleTroopsList[line].Find((troop) => troop.rowFlag == row - 1);
            }
            if (up != null && down != null)//同时向两侧进行攻击
            {
                attackTroop.AttackAnotherTroop(up, down);
                return true;
            }
            else if (up != null)//攻击一侧
            {
                attackTroop.AttackAnotherTroop(up);
                return true;
            }
            else if (down != null)//攻击一侧
            {
                attackTroop.AttackAnotherTroop(down);
                return true;
            }
        }
        return false;
    }
}

public class AutoBattleTroop : AggregationEntity
{
    /// <summary>
    /// 源头的部队
    /// </summary>
    public TroopControl originTroop;
    /// <summary>
    /// 所在的战场
    /// </summary>
    WarBattle warBattle;
    //所属
    public int belong;
    //行位置
    public int lineFlag;
    //列位置
    public int rowFlag;
    //血量
    public int basicHp;
    //命中
    public int hitRate;
    //防御率
    public int defendLevel;
    //名称
    public string soldierName;
    //攻击范围
    float attackRange;
    //攻击间隔
    float weaponSpeed;

    /// <summary>
    /// 0:攻击 1:移动(向前)
    /// </summary>
    public int actionState;
    //武器
    EquipSetData weapon;
    //护甲
    EquipSetData armour;
    //行列数量情况
    public List<int> unitHPs;
    float weaponCoolDown;
    const float weaponBasicAttackTime = 0.9f;
    const float moveUseTime = 2f;
    const float lineDistance = 10f;
    //个体持有的状态
    public List<EntityStatus> status = new List<EntityStatus>();
    public float actionStartTime;
    public float actionMaxTime;

    public int aliveNumber;
    /// <summary>
    /// 远程攻击者
    /// </summary>
    public bool isRangeAttack;
    public AutoBattleTroop(WarBattle battle, TroopControl troop, int belong)
    {
        warBattle = battle;
        this.belong = belong;
        originTroop = troop;
        ApplyData(troop.troopEntity.originData);
        aliveNumber = troop.nowNum;
        unitHPs = new List<int>(aliveNumber);
        for (int i = 0; i < aliveNumber; i++)
        {
            unitHPs.Add(basicHp);
        }
        battle.behaviourChain.PushDelayAction(weaponCoolDown, BattleTroopActionProcess);
    }

    /// <summary>
    /// 攻击另一个Troop
    /// </summary>
    public void AttackAnotherTroop(AutoBattleTroop anotherTroop, int number)
    {
        if (anotherTroop.aliveNumber <= 0) return;

        int damageHurtPower = weapon.Damage;
        int armourBreakPower = weapon.Break;
        int coverage = anotherTroop.armour.Coverage;
        int actualHurt = 0;
        for (int i = 0; i < number; i++)
        {
            //命中计算



            bool isHitArmour = Random.Range(0, 100) < coverage;
            if (isHitArmour)
            {
                int hardness = anotherTroop.armour.Hardness;
                if (armourBreakPower >= hardness)
                {
                    armourBreakPower -= hardness;
                    hardness = 0;
                }
                else
                {
                    hardness -= armourBreakPower;
                    armourBreakPower = 0;
                }
                damageHurtPower -= 2 * hardness;
                //护甲防御开始生效
            }
            actualHurt = damageHurtPower + armourBreakPower / 2;
            if (isRangeAttack)
            {
                //随机受害者
                anotherTroop.BeenHit(Random.Range(0, anotherTroop.aliveNumber), actualHurt);
            }
            else
            {
                anotherTroop.BeenHit(i, actualHurt);
            }

            if (anotherTroop.aliveNumber <= 0)
            {
                anotherTroop.BeenEliminated();
                break;
            }
        }
    }

    void BeenHit(int flag, int actualDamage)
    {
        if (aliveNumber == 0) return;
        flag = Mathf.Clamp(flag, 0, aliveNumber);
        if (actualDamage > 0)
        {
            unitHPs[flag] -= actualDamage;
            //Debug.LogError($"{originTroop.idName} GetDamage {actualDamage} Left {unitHPs[flag]}");
            if (unitHPs[flag] < 0)
            {
                unitHPs.Swap(flag, aliveNumber - 1);
                aliveNumber -= 1;
                originTroop.LostNum(1);
            }
        }
    }

    void BeenEliminated()
    {
        warBattle.behaviourChain.RemoveBehaviour(BattleTroopActionProcess);
        warBattle.LeaveBattle(this);
    }

    /// <summary>
    /// 攻击另一个Troop
    /// </summary>
    public void AttackAnotherTroop(AutoBattleTroop anotherTroop)
    {
        if (isRangeAttack)
        {
            int ableNumber = aliveNumber;
            AttackAnotherTroop(anotherTroop, ableNumber);
        }
        else
        {
            int ableNumber = Mathf.Clamp(aliveNumber, 0, 10);
            AttackAnotherTroop(anotherTroop, ableNumber);
        }
    }

    /// <summary>
    /// 攻击另一个Troop
    /// </summary>
    public void AttackAnotherTroop(params AutoBattleTroop[] anotherTroops)
    {
        int ableNumber = aliveNumber;
        if (!isRangeAttack)
        {
            ableNumber = Mathf.Clamp(ableNumber, 0, 10);
        }
        int leftAttackNumber = ableNumber / anotherTroops.Length;
        for (int i = 1; i < anotherTroops.Length; i++)
        {
            AttackAnotherTroop(anotherTroops[i], leftAttackNumber);
        }
        AttackAnotherTroop(anotherTroops[0], ableNumber - leftAttackNumber * (anotherTroops.Length - 1));

    }

    public void SetAction(int state)
    {
        actionState = state;
        warBattle.behaviourChain.RemoveBehaviour(BattleTroopActionProcess);
        actionStartTime = 0;
        actionMaxTime = 0;
        if (state == 0)
        {

        }
        else if (state == 1)
        {

        }
        BattleTroopActionProcess();
    }

    void BattleTroopActionProcess()
    {
        if (actionState == 0)
        {
            int distanceFlag = 0;
            if (lineFlag <= 3)
            {
                distanceFlag = 3 - lineFlag;
            }
            else
            {
                distanceFlag = lineFlag - 4;
            }

            if (warBattle.nowDistance + distanceFlag * lineDistance < attackRange && warBattle.IsAbleToAttack(this))
            {
                warBattle.behaviourChain.PushDelayAction(0.95f * weaponCoolDown, DealDamageToOtherSide);
                actionStartTime = warBattle.behaviourChain.nowTime;
                actionMaxTime = actionStartTime + weaponCoolDown;
                //没有可执行的内容
                warBattle.behaviourChain.PushDelayAction(weaponCoolDown, BattleTroopActionProcess);
            }
            else
            {
                actionStartTime = 0f;
                actionMaxTime = 0f;
                warBattle.behaviourChain.PushDelayAction(0.5f, BattleTroopActionProcess);
            }
        }
        else if (actionState == 1)
        {
            if (actionStartTime != 0)
            {
                actionState = 0;
            }
            actionStartTime = warBattle.behaviourChain.nowTime;
            actionMaxTime = actionStartTime + moveUseTime;
            warBattle.behaviourChain.PushDelayAction(moveUseTime, MoveToFrontLine);
        }
    }

    void MoveToFrontLine()
    {
        warBattle.MoveForward(this);
        SetAction(0);
        //切换逻辑
    }

    /// <summary>
    /// 对另一侧造成伤害
    /// </summary>
    void DealDamageToOtherSide()
    {
        warBattle.DealDamageToOtherSide(this);
    }

    public float GetMotionPercent(float time)
    {
        if (actionMaxTime == 0f) return 0f;
        float percent = Mathf.Clamp((time - actionStartTime) / (actionMaxTime - actionStartTime), 0, 1f);
        return percent;
    }


    void ApplyData(UnitData originData)
    {
        basicHp = originData.maxLife;
        hitRate = originData.hitRate;
        defendLevel = originData.defendLevel;
        soldierName = originData.name;
        //获取武器信息
        weapon = DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>(originData.weaponEquipSetId);
        armour = DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>(originData.armourEquipSetId);
        attackRange = weapon.AttackRange;
        //初始化UnitData中的statusList 
        //向StatusManager.Instance.RequestStatus()申请 添加进status当中
        if (originData.statusAttachs != null)
        {
            for (int i = 0; i < originData.statusAttachs.Length; i++)
            {
                status.Add(StatusManager.Instance.RequestStatus(originData.statusAttachs[i], this, belong));
            }
        }
        weaponSpeed = weapon.GetFloatValue("WeaponSpeed");
        weaponCoolDown = (weaponBasicAttackTime / weaponSpeed + 1f) * 2;
        string actionMotion = weapon.TargetActionModel;
        if (System.Enum.TryParse<ModelMotionType>(actionMotion, out ModelMotionType motionType))
        {
            switch (motionType)
            {
                case ModelMotionType.RANGE:
                    isRangeAttack = true;
                    break;
                default:
                    isRangeAttack = false;
                    break;
            }
        }
    }
}