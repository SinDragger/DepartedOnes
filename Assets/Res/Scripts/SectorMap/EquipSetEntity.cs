using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipSetEntity : AggregationEntity
{
    public EquipSetData data;
    /// <summary>
    /// 是否是可获取装备
    /// </summary>
    public bool isGettable;


    public EquipSetEntity(string equipSetDataIdName)
    {
        data = DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>(equipSetDataIdName);
    }
    public EquipSetEntity(EquipSetData equipSetData)
    {
        data = equipSetData;
    }
}
