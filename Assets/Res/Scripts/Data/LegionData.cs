using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegionData : AggregationEntity, IXMLPrintable
{
    /// <summary>
    /// 军团名称
    /// </summary>
    public string name;
    ///// <summary>
    ///// 初始地图位置
    ///// </summary>
    //public Vector2 position;
    /// <summary>
    /// 统领者ID
    /// </summary>
    public string generalIdName;
    /// <summary>
    /// 部队的Id-存储时的转化
    /// </summary>
    public TroopData[] troops;
    //内部Troop情况

    public EntityStack[] resCarry;

    public string PrintXML()
    {
        return "";
    }
}
