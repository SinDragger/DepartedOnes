using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraEffectTargets : StatusEffectTerm
{
    /// <summary>
    /// 半径
    /// </summary>
    public float radius;
    /// <summary>
    /// 所要附加的效果群
    /// </summary>
    public string[] attachEffects;
    /// <summary>
    /// 光环前缀名
    /// </summary>
    public string haloSetPrefix;
    /// <summary>
    /// 所生效的士兵集合
    /// </summary>
    public float time;
    public float maxTime = 0.02f;
    public override void Execution(AggregationEntity target)
    {
        time += Time.deltaTime;

        if (time > maxTime)
        {
            time -= maxTime;
            HashSet<SoldierStatus> statusSet = target.GetObjectValue<HashSet<SoldierStatus>>(haloSetPrefix + Constant_AttributeString.STATUS_HALO);
            if (statusSet == null) statusSet = new HashSet<SoldierStatus>();
            int selfBelong = target.GetIntValue(Constant_AttributeString.BELONG);
            #region sampleCode
            Vector3 centorPos = target.GetObjectValue<Vector3>(Constant_AttributeString.POS);
            var newSetModel = GridMapManager.instance.gridMap.GetCircleGridContainType<SoldierModel>(centorPos, radius);
            HashSet<SoldierStatus> newSet = new HashSet<SoldierStatus>();
            foreach (var setModel in newSetModel)
            {
                //添加进nowset
                if (Vector3.Distance(centorPos, setModel.lastPosition) <= radius)
                {
                    newSet.Add(setModel.nowStatus);
                }
            }
            ObjectPoolManager.Instance.Recycle(newSetModel);
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
            target.SetObjectValue(haloSetPrefix + Constant_AttributeString.STATUS_HALO, statusSet);
            #endregion
            //光环持有者的Belong
        }
    }
}
