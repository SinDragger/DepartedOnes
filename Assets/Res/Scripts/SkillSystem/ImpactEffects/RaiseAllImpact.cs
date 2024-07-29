using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class RaiseAllImpact : IImpactEffect
{
    ARPGControl control;
    SkillDeployer deployer;
    float countDown;
    GameObject specialEffects;
    public void Execute(SkillDeployer deployer)
    {
        countDown = deployer.SkillData.durationTime;
        this.deployer = deployer;
        control = deployer.SkillData.owner.GetComponent<ARPGControl>();
        control.characterSkillManager.canCastSkill = false;
        control.isItAFastMove = true;


        specialEffects = GameObjectPoolManager.Instance.Spawn("Prefab/" + deployer.SkillData.relatedEffectName, control.transform);
        specialEffects.transform.position = control.transform.position;
        specialEffects.GetComponent<SpecialEffects>().Execute();
        //剔除自生
        //判断落脚点 通过角色的头顶距离做射线判断 是否可以跳过去如果可以 就直接飞过去 
        //如果有障碍 还是起跳但是需要一直通过NavMesh.SamplePosition来判断跳跃方向的前进是否可以移动
        //角色先有一个上升的表现 后又一个下降的表现
        //当遇见不可移动的点时 应该直接落地
        //不可释放技能 且不可进行攻击
        //不一定会执行到看退出的情况但是还是要写上 防止没有退出的情况
        deployer.updateEvent += DeployerUpdate;
        deployer.fixedUpdateEvent += DeployerFixedUpdate;
    }

    private void DeployerFixedUpdate()
    {
    }

    /// <summary>
    /// 进行蓄力爆发
    /// </summary>
    private void DeployerUpdate()
    {
        if (countDown > 0)
        {
            countDown -= Time.deltaTime;
            if (countDown < 0)
            {
                BreakEffect();
                ResetCanSetDestantion();
            }
        }
    }

    private void BreakEffect()
    {
        int maxNumber = 100;
        int soulPoint = GameManager.instance.playerData.soulPoint;
        HashSet<GridMapCorpse> array = GridMapManager.instance.gridMap.GetCircleGridContainType<GridMapCorpse>(control.m_SoldierModel.lastPosition, 10f);
        var willRaiseCorpse = array.GetHashSetItems(maxNumber, (c) =>
        {
            return c.soldier.EntityData.speciesType != "Beast";
        });
        ObjectPoolManager.Instance.Recycle(array);
        List<SoldierStatus> willSeperateList = new List<SoldierStatus>();
        //构筑一个虚拟的新的Command
        foreach (var corpse in willRaiseCorpse)
        {
            if (corpse.soldier.commander.belong == BattleManager.instance.controlBelong)
            {
                if (soulPoint >= 1)
                {
                    GameManager.instance.playerForce.ChangeLimitedRes("SoulPoint", -1);
                    corpse.soldier.commander.RaiseUnit(corpse.soldier);
                }
                else
                {
                    break;
                }
            }
            else
            {
                //不复活敌军-在获得强化之前
                continue;
                corpse.soldier.commander.RaiseAndSeperateSoldier(corpse.soldier, "Zombie");
                willSeperateList.Add(corpse.soldier);
            }
        }
        if (willSeperateList.Count > 0)
        {
            UnitControlManager.instance.RaiseToTempCommand(willSeperateList, BattleManager.instance.controlBelong);
        }
    }

    private void ResetCanSetDestantion()
    {
        control.CanSetDestantion();
        //TODO 将下面两行代码写道control中直接进行调用
        control.m_Agent.enabled = true;
        control.characterSkillManager.canCastSkill = true;
        control.isItAFastMove = false;
        control.transform.DOKill();
        deployer.exitEvent -= ResetCanSetDestantion;
        deployer.updateEvent -= DeployerUpdate;
        deployer.fixedUpdateEvent -= DeployerFixedUpdate;
        deployer.SkillData.coolRemain = deployer.SkillData.coolTime;
        CameraControl.Instance.ChangeCameraToARPG(ARPGManager.Instance.currentGeneralControl);
        deployer.DestorySkillDeployer();
        specialEffects.GetComponent<SpecialEffects>().Recycle();
    }

    public bool Releasable()
    {
        //增加对死灵能量的判断
        return true;
    }
}
