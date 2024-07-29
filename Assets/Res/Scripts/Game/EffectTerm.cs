using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 效果——非状态类需求数据持有对象
/// 数据载体为BaseAffectEntity——作为持续时间内被更新的管理目标
/// ？内置字典持有数据？
/// </summary>
public class EffectTerm : AggregationEntity
{
    //生效的位置
    public Vector3 effectPos;
    public virtual void Execution() { }
}
