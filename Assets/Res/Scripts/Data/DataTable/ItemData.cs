using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ItemData : AggregationEntity, IXMLPrintable
{
    public int itemID;
    //item名字
    public string itemName;
    //物品描述
    public string itemDescription;

    //被动数据
    //EntityStack
    //物品自带的技能使用次数 
    public int skillUsageFrequency;
    //物品技能数据
    public SkillData skillData;
    //可以使用元组



    public string PrintXML()
    {
        throw new System.NotImplementedException();
    }

   
}
