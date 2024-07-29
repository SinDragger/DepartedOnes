using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 技能效果接口
/// </summary>
public interface IImpactEffect 
{
    /// <summary>
    /// 技能影响类的算法执行方法
    /// </summary>
    /// <param name="deployer"></param>
    void Execute(SkillDeployer deployer);
    bool Releasable();
}
