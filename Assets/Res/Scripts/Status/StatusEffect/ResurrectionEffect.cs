using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResurrectionEffect : StatusEffectTerm
{
    public float waitTime;
    public int maxAutoTime;
    public override void Execution(AggregationEntity target)
    {
        (target as SoldierStatus).onDie += OnDie;
    }

    void OnDie(SoldierStatus soldier)
    {
        if (soldier.raiseTime > maxAutoTime) return;
        CoroutineManager.StartFrameDelayedCoroutine(() =>
        {
            soldier.commander.RaiseUnit(soldier);
        });
    }

    public override void ReverseExecution(AggregationEntity target)
    {
        (target as SoldierStatus).onDie -= OnDie;
    }
}