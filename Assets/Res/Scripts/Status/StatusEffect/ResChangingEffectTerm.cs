using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// General专属词条
/// </summary>
public class ResChangingEffectTerm : StatusEffectTerm
{
    public string resName;
    public float changeRate;
    float count;
    /// <summary>
    /// 释放技能所需能量
    /// </summary>
    public override void Execution(AggregationEntity target)
    {
        count += TimeManager.Instance.nowDeltaTime * changeRate / 3600f;
        if (count > 1f)
        {
            int value = (int)count;
            count -= value;
            GameManager.instance.playerForce.ChangeLimitedRes(resName, value);//, (target as General).belong
        }
    }
}
