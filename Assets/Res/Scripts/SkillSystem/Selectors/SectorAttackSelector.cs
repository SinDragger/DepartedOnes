using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 扇形选区
/// </summary>
public class SectorAttackSelector : IAttackSelector
{
    //获取技能范围的内中的敌人
    public SoldierModel[] SelectTarget(SkillData data, Transform skillTF)
    {
        //根据技能数据中的标签 获取攻击的Tag
#if UNITY_EDITOR
        //Debug.LogError("目前的扇形选择器没有做到完全的扇形 只是圆形的选择器");
#endif
        var array = GridMapManager.instance.gridMap.GetCircleGridContain<SoldierModel>(skillTF.position, data.atackDistance);
        SoldierModel[] trs = new SoldierModel[array.Count];
        int i = 0;
        foreach (var unit in array)
        {
            trs[i] = (unit as SoldierModel);
            i++;
        }
        ObjectPoolManager.Instance.Recycle(array);
        return trs;
    }
}
