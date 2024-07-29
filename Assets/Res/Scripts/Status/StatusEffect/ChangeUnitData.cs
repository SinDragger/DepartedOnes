using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeUnitData : StatusEffectTerm
{
    public EntityStack subEntity;
    /// <summary>
    /// 破坏力
    /// </summary>
    public int destructivePower;
    /// <summary>
    /// 命中技巧
    /// </summary>
    public int hitRate;
    /// <summary>
    /// 防御等级
    /// </summary>
    public int defendLevel;
    /// <summary>
    /// 负重能力
    /// </summary>
    public int weightBearing;
    /// <summary>
    /// 移动速度
    /// </summary>
    public float speed;


    public override void Execution(AggregationEntity target)
    {
        base.Execution(target);
        //只对UnitData起效
        if (target is UnitData)
        {
            UnitData unitData = (target as UnitData);
            unitData.destructivePower += destructivePower;
            unitData.hitRate += hitRate;
            unitData.defendLevel += defendLevel;
            unitData.weightBearing += weightBearing;
            unitData.speed += speed;
          
        }
    }
    public override void ReverseExecution(AggregationEntity target)
    {
        base.ReverseExecution(target);
        if (target is UnitData)
        {
            UnitData unitData = (target as UnitData);
            unitData.destructivePower -= destructivePower;
            unitData.hitRate -= hitRate;
            unitData.defendLevel -= defendLevel;
            unitData.weightBearing -= weightBearing;
            unitData.speed -= speed;
        }
    }
}
