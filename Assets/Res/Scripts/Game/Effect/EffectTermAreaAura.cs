using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EffectTermAreaAura : EffectTerm
{
    //-1为无时间限制
    public float durationTime;
    public string[] halos;
    public float radius;


    public string materialName;
    public MaterialEffect materialEffect;

    public int isPraticle;
    public string effectName;


    public override void Execution()
    {
        //生成空光环挂载实例，让其管理自身存在时间，同时持有投影对其管理
        AuraFlagEntity e = GameObjectPoolManager.Instance.Spawn("Prefab/AuraFlagEntity", BattleManager.instance.transform).GetComponent<AuraFlagEntity>();
        e.Init(this);
        if (durationTime < 0f) return;
        CoroutineManager.DelayedCoroutine(durationTime, () =>
        {
            e.EndEffect();
        });
    }
}

public enum MaterialEffect
{ 
    None,
    Noise,
}
