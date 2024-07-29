using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCondition : StatusCondition
{
    public float time;

    public override bool CheckCondition(EntityStatus entity)
    {
        if (Time.realtimeSinceStartup - entity.timeStamp > time)
        {
            return true;
        }
        return false;
    }

}
