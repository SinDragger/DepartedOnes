using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAffectEntity : BaseAffectEntity
{
    public int breakNum;
    public int damageNum;


    public AttackAffectEntity() : base() { }

    public AttackAffectEntity(params object[] param) : base(param)
    {

        SoldierStatus attackSide = (param[0] as SoldierStatus);
        var weapon = attackSide.weapon;
        damageNum = weapon.Damage + attackSide.commander.basicAttackBonus;//TODO:更改值的获取方式
        breakNum = weapon.Damage + attackSide.commander.breakAttackBonus;
    }

    protected override void TriggerAffect(float timeDelta)
    {
        if (isEnd) return;
        SoldierStatus attackSide = (param[0] as SoldierStatus);
        SoldierStatus getHitSide = (param[1] as SoldierStatus);
        EquipSetData weapon = (param[2] as EquipSetData);
        //判断是否命中————无弹道近战武器情况
        if (param.Length == 3)
        {
            isEnd = true;

            if (attackSide != null && getHitSide != null && !getHitSide.isDead)
            {
                float distance = UnitControlManager.instance.GetSoldierSpaceDistance((attackSide as SoldierStatus).model, getHitSide.model);
                if (distance < weapon.AttackRange && !getHitSide.CheckPercentFloatValue("MeleeAttackProtectPercent"))
                {
                    int delta = attackSide.nowHitRate - getHitSide.nowDefendLevel;
                    if (UnityEngine.Random.Range(0, 1f) < 0.5f * Mathf.Pow(1.05f, delta))
                    {
                        int damage = getHitSide.BeenHit(damageNum, breakNum);
                        if (damage > 0)
                        {
                            getHitSide.OnBeenDamaged(attackSide);
                            getHitSide.BeenMeleeAttack(attackSide, damage);
                            if (getHitSide.isDead)
                            {
                                //TODO:杀死了
                                attackSide.OnKill(getHitSide);
                            }
                        }
                    }
                }
            }
        }
        else if (param.Length > 3)
        {
            DrivableObject drivableObject = (param[3] as DrivableObject);
            drivableObject.UpdateMovement(timeDelta);
            if (drivableObject.IsEnd())
            {
                isEnd = true;

                bool isRangeWeapon = weapon.TargetActionModel == ModelMotionType.RANGE.ToString();
                bool targetGetHit = false;
                if (getHitSide != null)//attackSide != null && 
                {
                    float distance = getHitSide.model.Distance(drivableObject.nowObject.transform.position);
                    if (distance < 0.3f)
                    {
                        if (isRangeWeapon && getHitSide.CheckPercentFloatValue("RangeAttackProtectPercent"))//不受伤害的Check情况 
                        {
                            targetGetHit = false;
                        }
                        else
                        {
                            targetGetHit = true;
                            int delta = attackSide.nowHitRate - getHitSide.nowRangeDefendLevel;
                            if (UnityEngine.Random.Range(0, 1f) > 0.5f * Mathf.Pow(1.05f, delta))
                            {
                                getHitSide.BeenHit(damageNum, breakNum);
                                getHitSide.commander.BeenShoot(attackSide.commander);
                            }
                        }
                    }
                    else
                    {
                        //Debug.LogError("FailHitTarget");
                    }
                }

                if (!targetGetHit)
                {
                    var set = GridMapManager.instance.gridMap.GetTargetGridContain<SoldierModel>(drivableObject.nowObject.transform.position);
                    if (set.Count > 0)
                    {
                        foreach (SoldierModel model in set)
                        {
                            if (model.Distance(drivableObject.nowObject.transform.position) < 0.3f)
                            {
                                if (isRangeWeapon && getHitSide.CheckPercentFloatValue("RangeAttackProtectPercent"))//不受伤害的Check情况
                                {
                                    targetGetHit = false;
                                }
                                else
                                {
                                    targetGetHit = true;
                                    int delta = attackSide.nowHitRate - model.nowStatus.nowRangeDefendLevel;
                                    if (UnityEngine.Random.Range(0, 1f) > 0.5f * Mathf.Pow(1.05f, delta))
                                        model.nowStatus.BeenHit(damageNum, breakNum);
                                    break;
                                }
                            }
                        }
                        //距离近且最近的(飞行单位怎么处理？)
                    }
                    //获取格子里离最近的敌人进行
                }
                drivableObject.TiggerEnd(targetGetHit ? 0 : 1);
                if (!targetGetHit)
                {
                    //Debug.LogError("FailHit");
                }
            }
            //对飞行弹药进行是否命中的计算等待
        }
    }
    public override void EndEffect(bool instantly)
    {
        if (isEnd) return;
        if (param.Length > 3)
        {
            DrivableObject drivableObject = (param[3] as DrivableObject);
            drivableObject.TiggerEnd(instantly ? 0 : 1);
        }
    }
}
