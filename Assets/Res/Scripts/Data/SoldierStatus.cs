using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 单位的数据情况·实际战场数据
/// </summary>

public class SoldierStatus : AggregationEntity, IBelongable
{
    CommandUnit c;//上级指挥
    /// <summary>
    /// TODO:修改字符大小写
    /// </summary>
    public CommandUnit commander
    {
        get { return c; }
        set
        {
            SetIntValue(Constant_AttributeString.BELONG, value.belong);
            c = value;
        }
    }
    public int Belong => commander.belong;
    public SoldierModel model;//对应的模型
    //个体持有的状态
    public List<EntityStatus> status = new List<EntityStatus>();

    TroopEntity entityData;
    public TroopEntity EntityData
    {
        get { return entityData; }
        set
        {
            entityData = value;
            ApplyData(value);
        }
    }

    /// <summary>
    /// 0为默认状态。1为中间状态 -1为死亡 2为复生中
    /// </summary>
    public int nowState = 0;

    public string soldierName;//单位名字，没有就默认单位名
    public int nowHp
    {
        get { return GetIntValue(Constant_AttributeString.UNIT_HP); }
        set { SetIntValue(Constant_AttributeString.UNIT_HP, value); }
    }
    public int nowHitRate
    {
        get { return GetIntValue(Constant_AttributeString.UNIT_HITRATE) + commander.GetCommandStateHitFix(); }
        set { SetIntValue(Constant_AttributeString.UNIT_HITRATE, value); }
    }
    public int nowDefendLevel
    {
        get { return GetIntValue(Constant_AttributeString.UNIT_DEFENDLEVEL) + commander.GetCommandStateDefendFix(); }
        set { SetIntValue(Constant_AttributeString.UNIT_DEFENDLEVEL, value); }
    }

    public int nowRangeDefendLevel
    {
        get { return 10; }
    }

    public int nowShield { get; set; }
    //位移改变频率
    public float controlDelta;
    /// <summary>
    /// 当前攻击范围
    /// </summary>
    public float nowAttackRange
    {
        get { return GetFloatValue(Constant_AttributeString.UNIT_ATKRANGE); }
        set { SetFloatValue(Constant_AttributeString.UNIT_ATKRANGE, value); }
    }

    public float nowSpeed;
    public float nowSpeedPercent = 1f;

    //攻击间隔中的冷却
    public float attackCoolDown = 0f;
    public List<SoldierStatus> aimUnits = new List<SoldierStatus>();//当前想要攻击的单位
    public List<SoldierStatus> aimSelfUnits = new List<SoldierStatus>();//当前攻击自己的单位

    //发生死亡发生

    public event System.Action<SoldierStatus> onDie;
    public event System.Action onReverse;
    public event System.Action onHit;
    public event System.Action onLifeChange;
    /// <summary>
    /// 把人杀了的时候
    /// </summary>
    public event System.Action<SoldierStatus> onKill;
    public event System.Action<SoldierStatus> onBeenDamaged;
    //ic event System.Action onLifeChange;

    public bool isActionAttacking => model.actionModel.IsAttacking;

    public SoldierStatus focusTarget;
    public EquipSetData weapon;
    public EquipSetData armour;
    public bool isDead;
    public float nowWeaponSpeed;
    public SoldierStatus()
    {

    }

    public const float lifeDouble = 1f;

    void ApplyData(TroopEntity entityData)
    {
        nowHp = (int)(entityData.maxLife * lifeDouble);
        nowHitRate = entityData.hitRate;
        nowDefendLevel = entityData.defendLevel;
        soldierName = entityData.originData.idName;
        //获取武器信息
        weapon = entityData.weaponEquipSet.data;
        armour = entityData.armourEquipSet.data;
        nowAttackRange = weapon.AttackRange;
        //切换进weapon的通用近战
        nowShield = 0;
        nowSpeed = entityData.speed;
        //初始化UnitData中的statusList 
        //向StatusManager.Instance.RequestStatus()申请 添加进status当中
        if (entityData.originData.statusAttachs != null && commander != null)
        {
            for (int i = 0; i < entityData.originData.statusAttachs.Length; i++)
            {
                var standardStatus = StatusManager.Instance.GetStatus(entityData.originData.statusAttachs[i]);
                if (standardStatus.type == StatusLayerType.COMMAND_STATUS)
                {
                    continue;
                }
                status.Add(StatusManager.Instance.RequestStatus(standardStatus, this, commander.belong));
            }
        }
        nowWeaponSpeed = weapon.GetFloatValue("WeaponSpeed");
        isDead = false;
    }

    public void ChangeWeaponSpeed(float value)
    {
        nowWeaponSpeed += value;
    }

    public float GetWeaponSpeed()
    {
        return nowWeaponSpeed + commander.GetWeaponSpeedDelta();
    }

    public void ChangeNowSpeed(float value)
    {
        nowSpeedPercent += value;
        nowSpeedPercent = Mathf.Clamp(nowSpeedPercent, 0f, 3.5f);
        model.SetSpeed(nowSpeed * nowSpeedPercent);
        model.SetSpeedPercent(nowSpeedPercent);
    }
    public void ChangeNowSpeedPercent(float value)
    {

        nowSpeedPercent = value;
        nowSpeedPercent = Mathf.Clamp(nowSpeedPercent, 0f, 3.5f);
        model.SetSpeed(nowSpeed * nowSpeedPercent);
        model.SetSpeedPercent(nowSpeedPercent);
    }
    public void UpdateNow(float deltaTime)
    {
        if (deathProof > 0)
        {
            deathProof -= deltaTime;
            if (deathProof <= 0)
            {
                TriggerDying(true);
                deathProof = 0;
            }
        }
    }

    public EntityStatus ApplyOutSideStatus(string idName, int belong = -1)
    {
        var sameStatus = status.Find((s) => s.originStatus.idName.Equals(idName));
        if (sameStatus != null)
        {
            sameStatus.heapNum++;
            return sameStatus;
        }
        else
        {
            var targetStatus = StatusManager.Instance.RequestStatus(idName, this, belong);
            targetStatus.heapNum = 1;
            status.Add(targetStatus);
            return targetStatus;
        }
    }

    public void ActiveStatus(string statusIdName, int belong = -1)
    {
        status.Add(StatusManager.Instance.RequestStatus(statusIdName, this, belong == -1 ? commander.belong : belong));
    }

    public void EndStatus(string statusIdName, int belong = -1)
    {
        //增加取消注册
        //在statusManager中注销EntityStatus
        EntityStatus target = status.Find((s) => s.originStatus.idName == statusIdName);
        if (status.Contains(target))
        {
            //多光环重复生效的移除1层
            target.heapNum--;
            if (target.heapNum <= 0)
            {
                target.StatusActiveEffectReverseExecution();
                StatusManager.Instance.RemoveStatus(target);
                status.Remove(target);
            }
        }
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

    public bool IsTargetInAttackRange()//TODO:需要校验
    {
        return IsTargetInAttackRange(focusTarget);
    }

    public bool IsTargetInAttackRange(SoldierStatus soldierStatus)//TODO:需要校验
    {
        float distance = UnitControlManager.instance.GetSoldierSpaceDistance(model, soldierStatus.model);
        float attackRange = nowAttackRange;
        if (GetBoolValue("DisableRemote"))
        {
            attackRange = 1.0f;
        }
        if (distance < attackRange)
            return true;
        return false;
    }
    /// <summary>
    /// 被近战攻击
    /// </summary>
    /// <param name="attackSide"></param>
    public void BeenMeleeAttack(SoldierStatus attackSide, int damage)
    {
        if (!string.IsNullOrEmpty(attackSide.weapon.damageEffectPrefab))
        {
            var g = GameObjectPoolManager.Instance.Spawn("Prefab/" + attackSide.weapon.damageEffectPrefab, model.lastPosition + new Vector3(0, 0.15f, 0), model.transform);
            CoroutineManager.DelayedCoroutine(1f, () =>
            {
                GameObjectPoolManager.Instance.Recycle(g, "Prefab/" + attackSide.weapon.damageEffectPrefab);
            });
        }
        if (damage > 0)
        {
            commander.RecordDamageFrom(attackSide.commander, damage);
        }
    }

    public void OnKill(SoldierStatus soldierStatus)
    {
        onKill?.Invoke(soldierStatus);
    }

    public void OnBeenDamaged(SoldierStatus soldierStatus)
    {
        onBeenDamaged?.Invoke(soldierStatus);
    }

    //元素损伤独立计算
    public int BeenHit(int damageHurtPower, int armourBreakPower)
    {
        int coverage = armour.Coverage;
        if (false)
            commander.TriggerCommandToAggressive();
        onHit?.Invoke();
        bool isHitArmour = Random.Range(0, 100) < coverage;
        if (isHitArmour)
        {
            int hardness = armour.Hardness;
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
            return GetDamage(damageHurtPower + armourBreakPower / 2);
        }
        else
        {
            return GetDamage(damageHurtPower + armourBreakPower / 2);
        }
    }

    /// <summary>
    /// 穿透防御的实际伤害
    /// </summary>
    public int GetDamage(int actualDamage)
    {
        if (GameManager.instance.troopInvincible) return 0;
        if (commander.belong == BattleManager.instance.controlBelong && GameManager.instance.playerInvincible)
        {
            return 0;
        }

        if (!CanBeHurt() || actualDamage <= 0) return 0;
        //判断是否死亡/转切阶段触发技能
        if (nowShield > 0)
        {
            if (nowShield - actualDamage > 0)
            {
                nowShield -= actualDamage;
                actualDamage = 0;
            }
            else
            {
                actualDamage -= nowShield;
                nowShield = 0;
            }
        }
        nowHp -= actualDamage;
        onLifeChange?.Invoke();
        if (nowHp <= 0) TriggerDying();//状态标记为Dying?
        else
        {
            model.actionModel.BeenHit();
        }
        model.GetHitted(actualDamage);
        return actualDamage;
    }

    /// <summary>
    /// 表现层
    /// </summary>
    public void OnAttackEnermy(SoldierStatus oldTarget)
    {
        if (oldTarget != null && oldTarget.nowHp <= 0)
        {
            oldTarget = null;
        }
        //缺失攻击目标？——尝试设立新目标
        if (oldTarget == null || oldTarget.model == null) return;
        //TODO:根据真实情况设置冷却值
        if (commander != null)
        {
            attackCoolDown = commander.GetAttackCoolDown();
            //Debug.LogError(attackCoolDown);
        }

        //创建攻击的弹道（暂时不考虑远程——攻击携带此次攻击的特效，并复制）
        //根据攻击模式创建特效与攻击——以及之后受击特效？
        //攻击拥有初始速度，并根据速度命中
        if (weapon.TargetActionModel == ModelMotionType.RANGE.ToString() && !GetBoolValue("DisableRemote"))
        {
            if (focusTarget != null && focusTarget.nowHp <= 0)
            {
                focusTarget = null;
                return;
            }
            float arrowSpeed = 50f;//箭速
            GameObject arrow = GameObjectPoolManager.Instance.Spawn(GetArrowName());
            arrow.transform.parent = BattleManager.instance.transform;
            arrow.gameObject.SetActive(true);
            var arrowC = arrow.GetComponent<AmmoArrow>();
            //远程攻击————位移扩散
            Vector3 targetPos = GetTargetPos();
            DrivableObject drivableObject = arrowC.Init(model.transform.position, targetPos, GetArrowName());
            //TODO:增加弹道设置
            drivableObject.Init(arrow, targetPos, arrowSpeed);
            BaseAffectEntity attackIntension = new AttackAffectEntity(this, focusTarget, weapon, drivableObject);
            BattleManager.instance.RegistAffect(attackIntension);
        }
        else
        {
            if (oldTarget != focusTarget) return;//转换攻击目标则丢失
            //近战攻击
            BaseAffectEntity attackIntension = new AttackAffectEntity(this, oldTarget, weapon);
            BattleManager.instance.RegistAffect(attackIntension);
        }
    }

    public string GetArrowName()
    {
        if (!string.IsNullOrEmpty(commander.ammoName))
        {

            return DataBaseManager.Instance.configMap[commander.ammoName];
        }
        return DataBaseManager.Instance.configMap["Ammo"];

    }

    Vector3 GetTargetPos()
    {
        Vector3 targetPos = focusTarget.model.transform.position + new Vector3(0, GameConfig.RangeWeaponYFix, 0);
        float distance = Vector3.Distance(model.transform.position, targetPos);
        //半箭1身位偏斜
        Vector2 random = UnityEngine.Random.insideUnitCircle * distance * 2 / weapon.AttackRange * EntityData.originData.ocupySize;
        targetPos.x += random.x;
        targetPos.z += random.y;
        return targetPos;
    }

    /// <summary>
    /// 变形
    /// </summary>
    public void ShapeShift(string targetSpecies)
    {
        //新旧Origin数据差值变化
        UnitData newUnitData = DataBaseManager.Instance.GetSpeciesTypeUnitData(EntityData.originData, targetSpecies);
        if (EntityData.originData.statusAttachs != null)
        {
            for (int i = 0; i < EntityData.originData.statusAttachs.Length; i++)
            {
                EndStatus(EntityData.originData.statusAttachs[i]);
            }
        }
        EntityData = new TroopEntity(newUnitData);
        if (EntityData.originData.statusAttachs != null)
        {
            for (int i = 0; i < EntityData.originData.statusAttachs.Length; i++)
            {
                ActiveStatus(EntityData.originData.statusAttachs[i]);
            }
        }
        model.Init(this);
        var switchModel = model.EquipSwitch;
        switchModel.speciesType = targetSpecies;
        switchModel.subSpeciesType = "";
        switchModel.OnInit();
    }

    /// <summary>
    /// 脱战/隐身/死亡时移除攻防关系
    /// </summary>
    public void RemoveAttackRelation()
    {
        for (int i = 0; i < aimSelfUnits.Count; i++)
        {
            aimSelfUnits[i].AttackTargetLost(this);
        }
    }

    public void AddAttackTarget(SoldierStatus target, bool isForceAttack = false)
    {
        aimUnits.Add(target);
        target.aimSelfUnits.Add(this);
        if (isForceAttack)
        {
            focusTarget = target;
        }
        else
        {
            if (focusTarget == null)//TODO:改成当前目标如果不是在blocking列表里则进行剔除
            {
                focusTarget = target;
            }
        }
    }

    public void SetMoveTargetPos(Vector3 pos)
    {
        if (nowState != 0) return;
        model.GetComponent<ClickToMove>().SetPositionMoveTo(pos);
        //根据地形进行非合理点的位置调整
    }

    public void AttackTargetLost(SoldierStatus target)
    {
        aimUnits.Remove(target);
        if (focusTarget == this) focusTarget = null;
        if (focusTarget == null && aimUnits.Count > 0)
        {
            focusTarget = aimUnits[0];
        }
        //TODO:重新选定新的目标
    }
    /// <summary>
    /// 脱战
    /// </summary>
    public void EscapeBattle()
    {
        aimUnits.Clear();
        focusTarget = null;
    }

    public Vector3 GetSourroundForce()
    {
        Vector3 sumForce = default;
        float separateDistance = 1.5f;
        Vector3 separateForce = default;
        float separateWeight = 0.4f;
        float cohesionDistance = 3f;
        float cohesionWeight = 0.6f;
        float clostIn = 3f;
        var models = GridMapManager.instance.gridMap.GetCircleGridContain<SoldierModel>(model.lastPosition, separateDistance);

        //排斥力
        foreach (SoldierModel neighbourModel in models)
        {
            if (neighbourModel == model || neighbourModel.nowStatus.Belong != Belong) continue;
            Vector3 dir = model.lastPosition - neighbourModel.lastPosition;
            sumForce += dir.normalized / dir.magnitude;
        }
        models = GridMapManager.instance.gridMap.GetCircleGridContain<SoldierModel>(model.lastPosition, cohesionDistance);

        //聚合力
        Vector3 center = Vector3.zero;
        foreach (SoldierModel neighbourModel in models)
        {
            if (neighbourModel == model) continue;
            center += neighbourModel.lastPosition;
        }
        if (models.Count > 1)
        {
            center /= models.Count - 1;
            //得到移动的力  center 越大  得到的力就越大 刚好符合
            Vector3 dirToCenter = center - this.model.lastPosition;
            //如果这种聚集力不合适的话就使用下面的
            // 如果上面的力不行的话就是用这种方法
            sumForce += dirToCenter * cohesionWeight;
        }
        return sumForce;
    }

    float deathProof;

    public bool CanBeHurt()
    {
        if (deathProof > 0) return false;
        if (nowHp <= 0) return false;
        return true;
    }

    //触发死亡
    public void TriggerDying(bool force = false)
    {
        if (!force && GetBoolValue(Constant_AttributeString.STATUS_DELAY_DEATH) && deathProof == 0f)
        {
            isDead = false;
            nowHp = 1;
            deathProof = 3f;
            return;
        }
        nowHp = 0;
        onDie?.Invoke(this);
        //移除所有状态
        EndStatus(status);
        //判断是否真正死亡
        RemoveAttackRelation();
        GameManager.instance.playerData.soulPoint += 1;
        if (Belong == GameManager.instance.belong)//并且是玩家单位
        {
            GameManager.instance.playerData.soulPoint += 1;
        }
        aimUnits.Clear();
        deathProof = 0f;
        commander.LostUnit(this);
        model.Die();
        isDead = true;
    }

    public int raiseTime = 0;
    /// <summary>
    /// 逆转死亡状态
    /// </summary>
    public void ReverseDeath()
    {
        raiseTime++;
        int leftHp = EntityData.maxLife;
        for (int i = 0; i < raiseTime; i++)
        {
            leftHp /= 2;
        }
        if (leftHp == 0) leftHp = 1;
        nowHp = leftHp;
        nowState = 0;
        model.actionModel.OnInit();
        model.actionModel.enabled = true;
        //显示光圈
        isDead = false;
        onReverse?.Invoke();
        //TODO:增加是否是当前操控的序列询问
        model.ReverseDeath();
        if (commander.isOnChoose)
            model.ShowGuide();
        if (entityData.originData.statusAttachs != null && commander != null)
        {
            for (int i = 0; i < entityData.originData.statusAttachs.Length; i++)
            {
                var standardStatus = StatusManager.Instance.GetStatus(entityData.originData.statusAttachs[i]);
                if (standardStatus.type == StatusLayerType.COMMAND_STATUS)
                {
                    continue;
                }
                status.Add(StatusManager.Instance.RequestStatus(standardStatus, this, commander.belong));
            }
        }
    }

    /// <summary>
    /// 离开战场的回收
    /// </summary>
    public void OnReset()
    {
        EndStatus(status);
        nowState = 0;
        aimUnits.Clear();
        focusTarget = null;
        //TODO:移除所有状态
        model.OnReset();
    }

    public void OnUpdate(float deltaTime)
    {
        //时间经过
        if (attackCoolDown > 0f)
            attackCoolDown -= deltaTime;
    }

}
