using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 效果实体，进行效果触发:法术、弹道、攻击的基类
/// 一个效果实体按规则影响其他单位
/// 效果实体不带范围与移动锁定规则，只负责对被投入交互的接收方进行影响
/// </summary>
public class BaseAffectEntity : AggregationEntity
{
    public bool isEnd;
    //关联的实体
    protected object[] param;
    //计时器相关物体
    public BaseAffectEntity(){}

    public BaseAffectEntity(params object[] param)
    {
        this.param = param;
    }
    /// <summary>
    /// 效果更新
    /// </summary>
    public virtual void UpdateAffect(float timeDelta)
    {
        TriggerAffect(timeDelta);
    }

    protected virtual void TriggerAffect(float timeDelta)
    {
        if (isEnd) return;
    }
    public virtual void EndEffect(bool instantly)
    {
    }
    public virtual bool IsRelatedTo(object target,int flag)
    {
        if (param.Length <= flag) return false;
        return (param[flag] as DrivableObject).nowObject.Equals(target);
    }
}

public enum AffectType
{
    WeaponAttack,//使用武器攻击:攻击方 目标方 所使用的武器 |（实体关联:弹药 计算弹道 时间）DOPath
}