using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 
/// </summary>
public class ChargeImpact : IImpactEffect
{
    //敌人离开技能范围剔除 HashSet<Transform>没有做
    HashSet<SoldierModel> underAttack = new HashSet<SoldierModel>();
    //不用把底层运算逻辑写在这个执行方法里 
    //需要把底层运算逻辑写在各个逻辑的单独方法中
    //data.attackTargets[] 所有的攻击敌人

    ARPGControl control;
    SkillDeployer deployer;
    Vector2 targetMoveTo;
    Vector3 targetMovePos;

    GameObject specialEffects;
    float nowTime;
    public void Execute(SkillDeployer deployer)
    {
        this.deployer = deployer;
        //设置移动的方向点
        SetTargetMovePos();
        //不可释放技能 且不可进行攻击
        control.characterSkillManager.canCastSkill = false;
        //control.canAttack = false;
        control.isItAFastMove = true;
        //把自生剔除
        underAttack.Add(control.m_SoldierModel);
      
        //生成粒子特效prefab 并且执行
        specialEffects = GameObjectPoolManager.Instance.Spawn(DataBaseManager.Instance.configMap["ChargeSpecialEffects"], control.transform);
        Quaternion rotation = Quaternion.FromToRotation(-specialEffects.transform.forward, CameraControl.Instance.CameraToward(targetMoveTo));
        //// 将旋转应用到物体的朝向
        specialEffects.transform.rotation = rotation * specialEffects.transform.rotation;
        specialEffects.transform.position = control.transform.position + Vector3.up / 1.5f;
        specialEffects.GetComponent<SpecialEffects>().Execute();
        //不一定会执行到看退出的情况但是还是要写上 防止没有退出的情况
        deployer.exitEvent += ResetCanSetDestantion;
        deployer.updateEvent += DeployerUpdate;
        deployer.fixedUpdateEvent += DeployerFixedUpdate;
        nowTime = 0f;
        lastPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    }
    void DeployerFixedUpdate()
    {



    }
    Vector2 controlXZ { get { return new Vector2(control.transform.position.x, control.transform.position.z); } }
    Vector2 targetMovePosXZ;
    float velocity = 15f;

    Vector3 lastPosition;
    void DeployerUpdate()
    {
        //需要将Prefab的位置设置成玩家的位置在节能持续的期间
        deployer.transform.position = control.transform.position;
        if (lastPosition == control.transform.position)
        {
            ResetCanSetDestantion();
        }
        lastPosition = control.transform.position;
        nowTime += Time.deltaTime;
        if (deployer.SkillData.durationTime < nowTime + 0.1f)
        {
            ResetCanSetDestantion();
        }
        specialEffects.transform.position = control.transform.position + Vector3.up / 1.5f;
        if (Vector2.Distance(controlXZ, targetMovePosXZ) < 0.5f)
        {
            //停止移动
            ResetCanSetDestantion();
        }
        if (control.MoveToPosition(targetMoveTo, velocity).Equals(Vector3.zero))
        {
            //停止移动
            ResetCanSetDestantion();
        }
        else
        {
            deployer.CalculateTargets(deployer.SkillData.owner.transform);
            PushAwayTheEnemy();
        }
    }

    /// <summary>
    /// 设置冲锋的目的地
    /// </summary>
    /// <param name="deployer"></param>
    public void SetTargetMovePos()
    {
        //先设获取 人物移动的方向向量
        control = deployer.SkillData.owner.GetComponent<ARPGControl>();
        targetMoveTo = control.targetMoveTo;
        //当方向向量为Vector2.zero 需要通过人物朝向获得方向向量
        if (targetMoveTo == Vector2.zero)
        {
            //targetMoveTo=当前面朝方向正前方 也就是 左或者右
            targetMoveTo = control.GetComponent<ClickToMove>().IsRight ? Vector2.right : Vector2.left;
        }
        //单位向量化防止之后引用出问题

        //冲锋的距离
        float distance = 10;
        velocity = distance / deployer.SkillData.durationTime;
        //deployer.SkillData.durationTime = distance / velocity;
        //在将释放技能的人物移动设置为不可移动
        control.CannotSetDestantion();
        targetMovePos = deployer.SkillData.owner.transform.position + new Vector3(targetMoveTo.x, 0, targetMoveTo.y).normalized * distance;
        targetMovePosXZ = new Vector2(targetMovePos.x, targetMovePos.z);

    }

    //推开的敌人 
    //在hashset中判断这个transfrom是被推过的没有 
    //被推过 不管
    //没被推过 将敌人推开 并且加入hashset
    void PushAwayTheEnemy()
    {
        foreach (var sModel in deployer.SkillData.attackTargets)
        {
            //如果trs在hashset中不做操作
            if (!underAttack.Contains(sModel))
            {
                float distance = UnitControlManager.instance.GetSoldierSpaceDistance(sModel, control.m_SoldierModel);
                if (UnitControlManager.instance.GetSoldierSpaceDistance(sModel, control.m_SoldierModel) > deployer.SkillData.atackDistance)
                {
                    continue;
                }
                //如果trs不在hashset中将transfrom于control.transfrom.position作差值向量 方向取control到trs的方向
                Vector3 pushDirection = sModel.transform.position - control.transform.position;
                //TODO 推开敌人效果目前理想 但可能还需更改
                //通过pushDirection.normalized与 targetMoveTo 做判断
                sModel.transform.position += FilterTargetMoveDirection(pushDirection.normalized);
                //获取敌人数据
                if (sModel.nowStatus.Belong != control.m_SoldierModel.nowStatus.Belong)
                    sModel.nowStatus.GetDamage((int)deployer.SkillData.atkRatio);
                underAttack.Add(sModel);
            }
        }


    }
    /// <summary>
    /// 传入角色减去敌人位置的向量 
    /// 将其转换成二维向量 
    /// 根据向量的叉乘确定目标在冲锋方向的左右 
    /// 再根据左右获取到冲锋方向两个垂直单位向量 
    /// 在将其转换回三维向量传回
    /// </summary>
    /// <param name="pushDirection"></param>
    /// <returns></returns>
    Vector3 FilterTargetMoveDirection(Vector3 pushDirection)
    {
        Vector2 pushDirectionV2 = new Vector2(pushDirection.normalized.x, pushDirection.normalized.z);

        if (GetRelativeDirectionIsLeft(targetMoveTo, pushDirectionV2))
        {
            return new Vector3(-targetMoveTo.y, 0, targetMoveTo.x);
        }
        else
        {
            return new Vector3(targetMoveTo.y, 0, -targetMoveTo.x);
        }

    }

    bool GetRelativeDirectionIsLeft(Vector2 vector1, Vector2 vector2)
    {
        float crossProduct = vector1.x * vector2.y - vector1.y * vector2.x;

        if (crossProduct > 0)
        {
            return true;
        }
        else if (crossProduct < 0)
        {
            return false;
        }
        else
        {
            return false;
        }


    }

    /// <summary>
    /// 到达目标点或者是冲锋节能事件到了之后要将角色的控制权返还给玩家
    /// </summary>
    void ResetCanSetDestantion()
    {
      
        control.transform.DOKill();
        control.CanSetDestantion();
        control.characterSkillManager.canCastSkill = true;
        //control.canAttack = true;
        control.isItAFastMove = false;
        CameraControl.Instance.smoothSpeed = 10;
        //播放冲锋停止动画
        deployer.SkillData.coolRemain = deployer.SkillData.coolTime;
        deployer.fixedUpdateEvent -= DeployerFixedUpdate;
        deployer.updateEvent -= DeployerUpdate;
        deployer.exitEvent -= ResetCanSetDestantion;
        underAttack.Clear();
        underAttack.Add(control.m_SoldierModel);
        specialEffects.GetComponent<SpecialEffects>().Recycle();
        deployer.DestorySkillDeployer();
    }

    public bool Releasable()
    {
        return true;
    }
}
