using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地形
/// </summary>
public class Landform : AggregationEntity
{
    public string name;
    public string color;

    public Sprite GetSprite()
    {
        return DataBaseManager.Instance.GetSpriteByIdName($"Landform_{idName}");
    }
}
