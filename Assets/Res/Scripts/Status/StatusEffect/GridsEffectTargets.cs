using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地形效果
/// </summary>
public class GridsEffectTargets : StatusEffectTerm
{
    /// <summary>
    /// 所要附加的效果群
    /// </summary>
    public string[] attachEffects;
    float time;
    float maxTime = 0.02f;
    HashSet<SoldierModel> newSetModel = new HashSet<SoldierModel>();
    public override void Execution(AggregationEntity target)
    {
        time += Time.deltaTime;

        if (time > maxTime)
        {
            time -= maxTime;
            HashSet<SoldierStatus> statusSet = target.GetObjectValue<HashSet<SoldierStatus>>(Constant_AttributeString.STATUS_HALO);
            if (statusSet == null) statusSet = new HashSet<SoldierStatus>();
            int selfBelong = target.GetIntValue(Constant_AttributeString.BELONG);
            newSetModel.Clear();
            //目前无论count多少只会生效一层
            Dictionary<BaseGrid, int> gridSet = target.GetObjectValue<Dictionary<BaseGrid, int>>(Constant_AttributeString.STATUS_EFFECT_AREA);
            foreach (var grid in gridSet)
            {
                foreach (SoldierModel gridObject in grid.Key.GetGridContain<SoldierModel>())
                {
                    if (gridObject.nowStatus.GetBoolValue(Constant_AttributeString.STATUS_TERRAIN_PROOF)) continue;
                    newSetModel.Add(gridObject);
                }
            }
            HashSet<SoldierStatus> newSet = new HashSet<SoldierStatus>();
            foreach (var setModel in newSetModel)
            {
                //添加进nowset
                newSet.Add(setModel.nowStatus);
            }
            foreach (var newStatus in newSet)
            {
                //已在旧Set中持有，跳过
                if (!statusSet.Remove(newStatus))
                {
                    //旧Set未持有——对其增加光环影响状态
                    for (int i = 0; i < attachEffects.Length; i++)
                    {
                        newStatus.ApplyOutSideStatus(attachEffects[i], selfBelong);
                    }
                }
            }
            foreach (var oldNeedRemove in statusSet)
            {
                //所有在新的里面没有的Status——进行移除
                //移除buff
                for (int i = 0; i < attachEffects.Length; i++)
                {
                    oldNeedRemove.EndStatus(attachEffects[i]);
                }
            }
            statusSet = newSet;
            target.SetObjectValue(Constant_AttributeString.STATUS_HALO, statusSet);
            //光环持有者的Belong
        }
    }
}
