using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 构造结构
/// </summary>
public class Construction : AggregationEntity
{
    //名称
    public string name;
    //描述
    public string des;
    //完整度
    public float durability;
    //所需工程量
    public int workload;
    /// <summary>
    /// 相关地图
    /// </summary>
    public string relatedMapId;
    /// <summary>
    /// 相关事件
    /// </summary>
    public string relatedEvent;
    /// <summary>
    /// 占比大小
    /// </summary>
    public int size;
    /// <summary>
    /// 建设所需
    /// </summary>
    public EntityStack[] cost;
    //建设中效果

    //建设完效果

    //被摧毁效果
    /// <summary>
    /// 岗位提供
    /// </summary>
    public EntityStack[] workStation;
    /// <summary>
    /// 存储空间-资源的存储
    /// </summary>
    public int storageSpace;
}

public class EntityStack
{
    public string idName;
    public int num;
}