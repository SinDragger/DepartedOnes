using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSkillDeployer : SkillDeployer
{
    public override void DeploySkill()
    {
        //计算在目标内的敌人
        CalculateTargets();

        //对目标内的敌人施加技能效果
        ImapactTargets();
    }
    
}
