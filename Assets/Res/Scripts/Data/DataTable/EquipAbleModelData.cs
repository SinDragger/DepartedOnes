using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 模型的位置修正数据
/// </summary>
public class EquipAbleModelData : AggregationEntity, IXMLPrintable
{
    /// <summary>
    /// 基础物种
    /// </summary>
    public string baseSpeciesName;
    ///// <summary>
    ///// 动作模式类型
    ///// </summary>
    public string idleMotionName_Melee;
    public string attackMotionName_Melee;
    public string[] extraAttackMotionName_Melee;
    public string attackMotionName_Shoot;
    public string moveMotionName_Melee;

    public string idleMotionName_Polearm;
    public string attackMotionName_Polearm;
    public string moveMotionName_Polearm;

    public string idleMotionName_TwoHanded;
    public string attackMotionName_TwoHanded;
    public string moveMotionName_TwoHanded;

    public string dieMotionName;
    public string dieMotionSecName;

    public string targetModelName;
    public float scale;

    //public string moveStartMotionName;
    //public string moveEndMotionName;


    /// <summary>
    /// spine的资源名称
    /// </summary>
    public string spineResName;
    //增设modelName
    public ModelSuitData[] DataArray;

    public string Print()
    {
        string result="";
        result = idName;
        for (int i = 0; i < DataArray.Length; i++)
        {
            result += '\n' + $"{DataArray[i].slotName} {DataArray[i].rotationDelta} {DataArray[i].posDelta}";
        }
        return result;
    }

    public string PrintXML()
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        stringBuilder.AppendLine($"<EquipAbleModelData name=\"{idName}\">");

        stringBuilder.Append("<baseSpeciesName>");
        stringBuilder.Append(baseSpeciesName);
        stringBuilder.Append("</baseSpeciesName>");
        stringBuilder.AppendLine();

        //stringBuilder.Append("<motionType>");
        //stringBuilder.Append(motionType);
        //stringBuilder.Append("</motionType>");
        //stringBuilder.AppendLine();

        stringBuilder.AppendLine("<DataArray>");
        foreach (IXMLPrintable data in DataArray)
        {
            stringBuilder.Append(data.PrintXML());
        }
        stringBuilder.AppendLine("</DataArray>");
        stringBuilder.AppendLine("</EquipAbleModelData>");
        return stringBuilder.ToString();
    }
}


[System.Serializable]
public class ModelSuitData : IXMLPrintable
{
    public string slotName;
    public string equipTypeName;
    public Vector2 posDelta;
    public float rotationDelta;

    public ModelSuitData() { }
    public ModelSuitData(string partName, Vector2 delta, float rotation)
    {
        slotName = partName;
        posDelta = delta;
        rotationDelta = rotation;
    }

    public string PrintXML()
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        stringBuilder.AppendLine("<ModelSuitData>");

        stringBuilder.Append("<slotName>");
        stringBuilder.Append(slotName);
        stringBuilder.Append("</slotName>");
        stringBuilder.AppendLine();

        stringBuilder.Append("<equipTypeName>");
        stringBuilder.Append(equipTypeName);
        stringBuilder.Append("</equipTypeName>");
        stringBuilder.AppendLine();

        stringBuilder.Append("<posDelta>");
        stringBuilder.Append(posDelta.x + "," + posDelta.y);
        stringBuilder.Append("</posDelta>");
        stringBuilder.AppendLine();

        stringBuilder.Append("<rotationDelta>");
        stringBuilder.Append(rotationDelta);
        stringBuilder.Append("</rotationDelta>");
        stringBuilder.AppendLine();

        stringBuilder.AppendLine("</ModelSuitData>");
        return stringBuilder.ToString();
    }
}