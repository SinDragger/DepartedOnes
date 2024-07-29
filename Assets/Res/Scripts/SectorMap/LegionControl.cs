using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 存储数据-区块地图级别
/// </summary>
public class LegionControl : AggregationEntity
{
    public string name;
    public int Id { get; private set; }
    /// <summary>
    /// 当前位于的区块
    /// </summary>
    public Color nowSectorReg;
    /// <summary>
    /// 当前位置
    /// </summary>
    public Vector2 position;
    /// <summary>
    /// 势力所属
    /// </summary>
    public int belong;

    public bool isExposed;
    /// <summary>
    /// 主将Id
    /// </summary>
    public General mainGeneral;

    public LegionState LastState;
    public LegionState State
    {
        get { return state; }
        set
        {
            if (state != null) state.LeaveState();
            state = value;
            if (state != null)
                state.EnterState();
        }
    }

    LegionState state = new LegionWaitState();
    //————————————根据部队决定的属性
    /// <summary>
    /// 资源携带
    /// </summary>
    public ResourcePool resourcePool = new ResourcePool();
    //分类的部队存储————
    public Dictionary<int, List<TroopControl>> stackTroops = new Dictionary<int, List<TroopControl>>();
    /// <summary>
    /// 部下
    /// </summary>
    public List<TroopControl> troops = new List<TroopControl>();
    /// <summary>
    /// 携带装备
    /// </summary>
    public List<TroopControl> troopEquips = new List<TroopControl>();
    public float moveSpeed;

    /// <summary>
    /// 携带装备
    /// </summary>
    public Dictionary<EquipSetData, int> equipWeaponStore = new Dictionary<EquipSetData, int>();
    public Dictionary<EquipSetData, int> equipArmourStore = new Dictionary<EquipSetData, int>();

    public int equipsTotalWeight;
    public int equipsTotalNumber;

    public bool uiDataChanged;
    public bool unitChanged;
    public int MaxNum
    {
        get
        {
            int result = 0;
            for (int i = 0; i < troops.Count; i++)
            {
                result += troops[i].maxNum;
            }
            if (mainGeneral != null)
            {
                result += 1;
            }
            return result;
        }
    }
    public int TotalNum
    {
        get
        {
            int result = 0;
            for (int i = 0; i < troops.Count; i++)
            {
                result += troops[i].nowNum;
            }
            if (mainGeneral != null)
            {
                result += 1;
            }
            return result;
        }
    }

    public int ResCarryNum
    {
        get
        {
            int result = 0;
            foreach (var res in resourcePool.resourceCarry)
            {
                result += res.Value;
            }
            foreach (var equips in troopEquips)
            {
                //result += equips.equipNum;
            }
            //TODO:增加携带的装备套组的数量
            return result;
        }
    }

    public int ControlPower => troops.Count + (mainGeneral == null ? 0 : 10);

    public float nowDetectRange;
    public float BaseDetectRange => 5;
    public LegionData data;
    public static int maxSize = 8;

    public bool isOnFriendLand => SectorBlockManager.Instance.GetBlock(nowSectorReg).belong == belong;

    public LegionControl()
    {
        Id = LegionManager.Instance.RegistLegionId();
    }
    public LegionControl(string idName, int belong)
    {
        Id = LegionManager.Instance.RegistLegionId();
        data = DataBaseManager.Instance.GetIdNameDataFromList<LegionData>(idName);
        if (data.generalIdName != null)
        {
            mainGeneral = new General(DataBaseManager.Instance.GetIdNameDataFromList<GeneralData>(data.generalIdName));
            mainGeneral.belong = belong;
            //将将领的全局效果进行注册
        }
        idName = data.idName;
        name = data.name;
        this.belong = belong;
        isExposed = belong == GameManager.instance.belong;
        if (data.troops != null)
        {
            for (int i = 0; i < data.troops.Length; i++)
            {
                string unitName = data.troops[i].unitIdName;
                var troop = new TroopControl(unitName, data.troops[i].speciesType, data.troops[i].subSpeciesType, data.troops[i].number, belong);
                if (!string.IsNullOrEmpty(data.troops[i].useWeaponSet))
                {
                    troop.troopEntity.ChangeWeapon(data.troops[i].useWeaponSet);
                }
                if (!string.IsNullOrEmpty(data.troops[i].useArmourSet))
                {
                    troop.troopEntity.ChangeWeapon(data.troops[i].useArmourSet);
                }
                AddTroop(troop);
            }
        }
        if (data.resCarry != null)
        {
            for (int i = 0; i < data.resCarry.Length; i++)
            {
                resourcePool.ChangeResource(data.resCarry[i].idName, data.resCarry[i].num);
            }
        }
    }
    public LegionControl(LegionDeployData deployData)
    {
        Id = LegionManager.Instance.RegistLegionId();
        data = DataBaseManager.Instance.GetIdNameDataFromList<LegionData>(deployData.legionIdName);
        if (data.generalIdName != null)
        {
            mainGeneral = new General(DataBaseManager.Instance.GetIdNameDataFromList<GeneralData>(data.generalIdName));
            mainGeneral.belong = deployData.belong;
            //将将领的全局效果进行注册
        }
        idName = data.idName;
        name = data.name;
        belong = deployData.belong;
        isExposed = belong == GameManager.instance.belong;
        if (data.troops != null)
        {
            for (int i = 0; i < data.troops.Length; i++)
            {
                string unitName = data.troops[i].unitIdName;
                var troop = new TroopControl(unitName, data.troops[i].speciesType, data.troops[i].subSpeciesType, data.troops[i].number, belong);
                if (!string.IsNullOrEmpty(data.troops[i].useWeaponSet))
                {
                    troop.troopEntity.ChangeWeapon(data.troops[i].useWeaponSet);
                }
                if (!string.IsNullOrEmpty(data.troops[i].useArmourSet))
                {
                    troop.troopEntity.ChangeWeapon(data.troops[i].useArmourSet);
                }
                AddTroop(troop);
            }
        }
        //资源内容
        position = new Vector2(deployData.posX, deployData.posY);
        if (deployData.stateParams != null && deployData.stateParams.Length > 0)
        {
            ActiveLegionStateParamState(0, deployData.stateParams);
        }
        if (data.resCarry != null)
        {
            for (int i = 0; i < data.resCarry.Length; i++)
            {
                resourcePool.ChangeResource(data.resCarry[i].idName, data.resCarry[i].num);
            }
        }
    }

    public LegionControl(string legionIdNameData, int belong, bool isEmpty)
    {
        Id = LegionManager.Instance.RegistLegionId();
        data = DataBaseManager.Instance.GetTargetAggregationData<LegionData>(legionIdNameData);
        this.belong = belong;
        if (data.generalIdName != null)
        {
            mainGeneral = new General(DataBaseManager.Instance.GetIdNameDataFromList<GeneralData>(data.generalIdName));
            mainGeneral.belong = belong;
        }
        idName = data.idName;
        name = data.name;
        isExposed = belong == GameManager.instance.belong;
        if (!isEmpty && data.troops != null)
        {
            for (int i = 0; i < data.troops.Length; i++)
            {
                string unitName = data.troops[i].unitIdName;
                var troop = new TroopControl(unitName, data.troops[i].speciesType, data.troops[i].subSpeciesType, data.troops[i].number, belong);
                AddTroop(troop);
            }
            if (data.resCarry != null)
            {
                for (int i = 0; i < data.resCarry.Length; i++)
                {
                    resourcePool.ChangeResource(data.resCarry[i].idName, data.resCarry[i].num);
                }
            }
        }
    }

    // <summary>
    // 直接的分离-从前向后拨款部队
    // </summary>
    public LegionControl SperateLegion(int num)
    {
        LegionControl result = new LegionControl(data.idName, belong, true);
        result.mainGeneral = null;
        TransportUnitTo(result, num);
        return result;
    }

    /// <summary>
    /// 激活Legion的State参数
    /// </summary>
    void ActiveLegionStateParamState(int flag, string[] stateParams)
    {
        if (stateParams[flag] == "LegionDefendState")
        {
            State = new LegionDefendState(this);
        }
        else if (stateParams[flag] == "LegionMoveState")
        {
            Vector2 targetPos = default;
            if (float.TryParse(stateParams[flag + 1], out float result))
            {
                targetPos.x = result;
            }
            if (float.TryParse(stateParams[flag + 2], out result))
            {
                targetPos.y = result;
            }
            State = new LegionMoveState(this, targetPos, (distance) =>
                {
                    if (distance == 0)
                    {
                        if (stateParams.Length > flag + 3)
                        {
                            ActiveLegionStateParamState(flag + 3, stateParams);
                        }
                        else
                        {
                            State = new LegionWaitState();
                        }
                    }
                });
        }
        else if (stateParams[flag] == "LegionOccupyState")
        {
            var c = ConstructionManager.Instance.GetClosestConstruction(position);
            State = new LegionOccupyState(this, c);
        }
    }

    /// <summary>
    /// 增援单位
    /// </summary>
    public void ReinforceUnits(string unitType, string speciesName, int num)
    {
        uiDataChanged = true;
        if (speciesName == Constant_AttributeString.SPECIES_EQUIP)
        {
            for (int i = 0; i < troopEquips.Count; i++)
            {
                if (troopEquips[i].idName == unitType)
                {
                    int emptyAble = troopEquips[i].maxNum - troopEquips[i].nowNum;
                    if (emptyAble == 0) continue;
                    if (emptyAble >= num)
                    {
                        troopEquips[i].ReinforceNumber(num);
                        return;
                    }
                    else
                    {
                        troopEquips[i].ReinforceNumber(emptyAble);
                        num -= emptyAble;
                    }
                }
            }
            if (num > 0)
            {
                AddTroop(new TroopControl(unitType, speciesName, num, belong));
            }
            return;
        }
        //遍历下方所有troop，补足人数
        for (int i = 0; i < troops.Count; i++)
        {
            if (troops[i].idName == unitType && troops[i].troopEntity.speciesType == speciesName)
            {
                int emptyAble = troops[i].maxNum - troops[i].nowNum;
                if (emptyAble == 0) continue;
                if (emptyAble >= num)
                {
                    troops[i].ReinforceNumber(num);
                    return;
                }
                else
                {
                    troops[i].ReinforceNumber(emptyAble);
                    num -= emptyAble;
                }
            }
        }
        if (num > 0)
        {
            AddTroop(new TroopControl(unitType, speciesName, num, belong));
        }
    }

    /// <summary>
    /// 部队合并
    /// </summary>
    public void MergeTo(LegionControl legion)
    {
        for (int i = troops.Count - 1; i >= 0; i--)
        {
            var nowTroop = troops[i];
            legion.AddTroop(nowTroop);
            this.RemoveTroop(nowTroop);
        }
    }

    /// <summary>
    /// 单位移动给目标
    /// </summary>
    public void TransportUnitTo(LegionControl legion, int num)
    {
        string nowUnitIdName = "";
        for (int i = troops.Count - 1; i >= 0; i--)
        {
            var nowTroop = troops[i];
            nowUnitIdName = nowTroop.idName;
            int ableNumber = nowTroop.nowNum;
            if (num > ableNumber)
            {
                num -= ableNumber;
                legion.AddTroop(nowTroop);
                this.RemoveTroop(nowTroop);
            }
            else
            {
                nowTroop.LostNum(num);
                break;
            }
        }
        if (num != -0)
            legion.ReinforceUnits(nowUnitIdName, num);
    }

    /// <summary>
    /// 增援单位
    /// </summary>
    public void ReinforceUnits(string unitIdName, int num)
    {
        uiDataChanged = true;
        //遍历下方所有troop，补足人数
        for (int i = 0; i < troops.Count; i++)
        {
            if (troops[i].idName == unitIdName)
            {
                int emptyAble = troops[i].maxNum - troops[i].nowNum;
                if (emptyAble == 0) continue;
                if (emptyAble >= num)
                {
                    troops[i].ReinforceNumber(num);
                    return;
                }
                else
                {
                    troops[i].ReinforceNumber(emptyAble);
                    num -= emptyAble;
                }
            }
        }
        if (num > 0)
        {
            AddTroop(new TroopControl(unitIdName, num, belong));
        }
    }


    /// <summary>
    /// 创建新的部队
    /// TODO:控制调整Work位置
    /// </summary>
    /// <param name="troop"></param>
    public void AddTroop(TroopControl troop, int targetStack = -1)
    {
        uiDataChanged = true;
        if (troop.IsEquipSet())
        {
            troopEquips.Add(troop);
            return;
        }
        troops.Add(troop);//方便遍历
        if (AddTroopToTargetStack(troop, targetStack)) return;
        //前列放临时部队
        //战士填充到前列
        //远程填充到中列
        //施法单位填充到选锋精锐
        //辅兵填充到后排格
        //其余填充前线
        if (troop.troopEntity.unitType == UnitType.SUMMONOBJECT)
        {
            if (AddTroopToTargetStack(troop, 0)) return;
        }
        if (troop.troopEntity.unitType == UnitType.MONSTER)
        {
            if (AddTroopToTargetStack(troop, 0)) return;
        }
        if (troop.troopEntity.unitType == UnitType.FIGHTER)
        {
            if (DataBaseManager.Instance.GetModelMotionType(troop.troopEntity.originData) == ModelMotionType.RANGE)
            {
                if (AddTroopToTargetStack(troop, 1)) return;
            }
            else
            {
                if (AddTroopToTargetStack(troop, 0)) return;
            }
        }
        if (troop.troopEntity.unitType == UnitType.CASTER)
        {
            if (AddTroopToTargetStack(troop, 3)) return;
        }
        if (troop.troopEntity.unitType == UnitType.WORKER)
        {
            if (AddTroopToTargetStack(troop, 0)) return;
        }
        for (int i = 0; i < GameConfig.MAX_STACK - 1; i++)
        {
            if (!stackTroops.ContainsKey(i))
            {
                stackTroops[i] = new List<TroopControl>();
            }
            if (stackTroops[i].Count >= 8) continue;
            foreach (var t in stackTroops[i])
            {
                //增加合并
                if (t.troopEntity.originData.Equals(troop.troopEntity.originData))
                {
                    stackTroops[i].Add(troop);
                    return;
                }
            }
            if (stackTroops[i].Count == 0)
            {
                stackTroops[i].Add(troop);
                return;
            }
        }
        for (int i = 0; i < GameConfig.MAX_STACK - 1; i++)
        {
            if (stackTroops[i].Count == 1)
            {
                stackTroops[i].Add(troop);
                return;
            }
        }
        stackTroops[GameConfig.MAX_STACK - 1].Add(troop);
        //同类型合并
    }

    bool AddTroopToTargetStack(TroopControl troop, int targetStack)
    {
        if (targetStack >= 0 && targetStack < GameConfig.MAX_STACK)
        {
            if (!stackTroops.ContainsKey(targetStack))
            {
                stackTroops[targetStack] = new List<TroopControl>();
            }
            if (stackTroops[targetStack].Count >= 8) return false;
            stackTroops[targetStack].Add(troop);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 暂时不由销毁导致部队的销毁，转移至编制撤销
    /// </summary>
    /// <param name="troop"></param>
    public void RemoveTroop(TroopControl troop)
    {
        uiDataChanged = true;
        if (troop.IsEquipSet())
        {
            troopEquips.Remove(troop);
            return;
        }
        troops.Remove(troop);
        foreach (var stack in stackTroops)
        {
            stack.Value.Remove(troop);
            if (stack.Value.Count == 0)
            {

            }
        }
    }

    /// <summary>
    /// UI移动接管
    /// </summary>
    public int MoveTroop(TroopControl troop, int to, int flag)
    {
        //uiDataChanged = true;
        if (flag < 0) flag = 0;
        int fromId = 0;
        foreach (var stack in stackTroops)
        {
            if (stack.Value != null && stack.Value.Contains(troop))
            {
                stack.Value.Remove(troop);
                fromId = stack.Key;
                break;
            }
        }
        if (!stackTroops.ContainsKey(to))
        {
            stackTroops[to] = new List<TroopControl>();
        }
        if (stackTroops[to].Count < flag)
        {
            stackTroops[to].Add(troop);
            //return stackTroops[to].Count;
        }
        else
        {
            stackTroops[to].Insert(flag, troop);

            //return flag;
        }
        return fromId;
    }

    public bool IsEliminate()
    {
        List<TroopControl> needRemoveTroops = new List<TroopControl>();
        bool result = true;
        if (mainGeneral != null && mainGeneral.life > 0)
        {
            return false;
        }
        for (int i = 0; i < troops.Count; i++)
        {
            if (troops[i].nowNum != 0)
            {
                result = false;
            }
        }
        if (result)
        {
            EventManager.Instance.DispatchEvent(new EventData(Constant_Event.LEGION_REMOVE, "Legion", this));
        }
        return result;
    }

    public void CheckRemove()
    {
        List<TroopControl> needRemoveTroops = new List<TroopControl>();
        for (int i = 0; i < troops.Count; i++)
        {
            if (troops[i].nowNum == 0)
            {
                needRemoveTroops.Add(troops[i]);
            }
        }
        foreach (var t in needRemoveTroops)
        {
            RemoveTroop(t);
        }
    }


    public void ReceiveEquip(Dictionary<EquipSetData, int> equipDic, bool isWeapon)
    {
        foreach (var pair in equipDic)
        {
            equipsTotalWeight += pair.Key.Weight * pair.Value;
            equipsTotalNumber += pair.Value;
        }
        if (isWeapon)
        {
            equipWeaponStore.DictionaryAppend(equipDic);
        }
        else
        {
            equipArmourStore.DictionaryAppend(equipDic);
        }
    }


    public void ChangeEquip(EquipSetData equipData, int equipNum, bool isWeapon)
    {
        if (equipData == null || equipData.Name == "无") return;
        if (isWeapon)
        {
            if (equipWeaponStore.ContainsKey(equipData))
            {
                equipWeaponStore[equipData] += equipNum;
            }
            else
            {
                equipWeaponStore[equipData] = equipNum;
            }
            //equipsTotalWeight += pair.Key.Weight * pair.Value;
            //equipsTotalNumber += pair.Value;
        }
        else
        {
            if (equipArmourStore.ContainsKey(equipData))
            {
                equipArmourStore[equipData] += equipNum;
            }
            else
            {
                equipArmourStore[equipData] = equipNum;
            }
        }
    }
}
