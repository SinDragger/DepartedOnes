using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTermVFX : EffectTerm
{
    /// <summary>
    /// 效果预制体名称
    /// </summary>
    public string effectPrefabName;
    /// <summary>
    /// 回收时间
    /// </summary>
    public float recycleTime;
    /// <summary>
    /// 创造时间
    /// </summary>
    public float createTime;
    public EffectTerm[] effects;
    public override void Execution()
    {
        string effectRoute = $"Prefab/{effectPrefabName}";
        var pos = effectPos;
        CoroutineManager.DelayedCoroutine(createTime, () =>
        {
            if (effects != null)
            {
                foreach (var effect in effects)
                {
                    effect.effectPos = pos;
                    effect.Execution();
                }
            }
            var e = GameObjectPoolManager.Instance.Spawn(effectRoute, BattleCastManager.instance.transform);
            e.transform.position = pos;
            CoroutineManager.DelayedCoroutine(recycleTime, () =>
            {
                GameObjectPoolManager.Instance.Recycle(e, effectRoute);
            });
        });
    }
}
