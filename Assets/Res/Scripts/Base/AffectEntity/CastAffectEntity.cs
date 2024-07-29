using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastAffectEntity : BaseAffectEntity
{
    //目标
    /// <summary>
    /// 作用范围
    /// </summary>
    int affectRange;

    /// <summary>
    /// 作用数量上限
    /// </summary>
    int affectNumberMax;
    /// <summary>
    /// 移动追踪目标
    /// </summary>
    Transform followerTarget;


    public CastAffectEntity(params object[] param) : base(param) { }

    protected override void TriggerAffect(float timeDelta)
    {
        if (isEnd) return;

        base.TriggerAffect(timeDelta);
    }
}
