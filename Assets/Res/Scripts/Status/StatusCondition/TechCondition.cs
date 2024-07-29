using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 科技解锁条件
/// </summary>
public class TechCondition : StatusCondition
{
    public string techIdName;

    /// <summary>
    /// </summary>
    /// <param name="paramValue"></param>
    /// <returns></returns>
    public override bool CheckCondition(EntityStatus entity)
    {
        return GameManager.instance.CheckTechAble(techIdName);
        return false;
    }
}
