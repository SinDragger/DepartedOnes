using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDieReflectEffect : StatusEffectTerm
{
    public int damage;
    public override void Execution(AggregationEntity target)
    {
        if (target is SoldierStatus)
        {
            (target as SoldierStatus).onBeenDamaged += OnBeenDamaged;
        }
        else Debug.Log(target.GetType().ToString());
    }

    void OnBeenDamaged(SoldierStatus soldierStatus)
    {
        soldierStatus.BeenHit(damage, 0);
    }
    public override void ReverseExecution(AggregationEntity target)
    {
        (target as SoldierStatus).onBeenDamaged -= OnBeenDamaged;
    }

}
