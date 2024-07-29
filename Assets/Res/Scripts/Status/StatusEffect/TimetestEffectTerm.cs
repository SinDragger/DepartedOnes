using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimetestEffectTerm : StatusEffectTerm
{
    public float time;
    public override void Execution(AggregationEntity target)
    {
        float f = target.GetFloatValue("计时器");
        f += Time.deltaTime;
        if (f > time)
        {
            target.SetFloatValue("计时器", f - time);
            var tt = DataBaseManager.Instance.GetTempData<EntityStatus>("LastUpdateTarget");
            tt.heapNum += 1;
        }
    }
}

