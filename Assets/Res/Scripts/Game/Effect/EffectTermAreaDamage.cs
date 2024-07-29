using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTermAreaDamage : EffectTerm
{
    public string damageType;
    public float range;
    public int damageNumber;
    /// <summary>
    /// 创造时间
    /// </summary>
    public float createTime;
    public override void Execution()
    {
        var pos = effectPos;
        CoroutineManager.DelayedCoroutine(createTime, () =>
        {
            GridMapManager.instance.gridMap.GetCircleGridContainTypeWithAction<SoldierModel>(pos, range, (model) =>
            {
                if (model.DistanceTo(pos) < range)
                    model.nowStatus.GetDamage(damageNumber);
            });
            GridMapManager.instance.gridMap.GetCircleGridContainTypeWithAction<IBlockTriggerable>(pos, range, (block) =>
            {
                block.OnBlock(damageType);
            });
        });

    }
}
