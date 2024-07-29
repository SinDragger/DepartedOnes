using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 护盾 
/// </summary>
public class ShapelessShieldImpact : IImpactEffect
{
    HashSet<CommandUnit> commandUnits = new HashSet<CommandUnit>();
    SkillDeployer deployer;
    ARPGControl control;
    public void Execute(SkillDeployer deployer)
    {
        this.deployer = deployer;
        control = deployer.SkillData.owner.GetComponent<ARPGControl>();
        control.control.PlayAnim(AnimState.ATTACK);
        //获取一定范围内所有的我方SoldierModel的commander中的belong与control的commander中的belong相同的commander加入到 HashSet<CommandUnit>中
        foreach (var sModel in deployer.SkillData.attackTargets)
        {
            //如果sModel在commander 在hashset中不做操作
            if (!commandUnits.Contains(sModel.commander) && sModel.commander.belong.Equals(control.m_SoldierModel.commander.belong))
            {
                //如果不在 且sModel.commander.belong与control的commander中的belong相同的commander加入到 HashSet<CommandUnit>中            
                commandUnits.Add(sModel.commander);
            }
        }
        //然后再对 HashSet<CommandUnit>中的元素遍历执行向活着的单位添加护甲的值
        foreach (var command in commandUnits)
        {
            command.ForEachAlive(AddShapelessShield);
        }
        deployer.SkillData.coolRemain = deployer.SkillData.coolTime;
    }

    public bool Releasable()
    {
        return true;
    }

    void AddShapelessShield(SoldierStatus soldierStatus) {
        
        soldierStatus.nowShield += 50;

    }

}
