using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 装备数据
/// </summary>
public partial class EquipData : AggregationEntity, IXMLPrintable
{
    public string equipPositionName;
    public string equipTexPath;
    public string alphaTexPath;                 
    public Vector2 posDelta;
    public float rotDelta;

    public string PrintXML()
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        if (string.IsNullOrEmpty(idName))
        {
            stringBuilder.AppendLine("<EquipData>");
        }
        else
        {
            stringBuilder.AppendLine($"<EquipData name=\"{idName}\">");
        }

        stringBuilder.Append("<equipPositionName>");
        stringBuilder.Append(equipPositionName);
        stringBuilder.Append("</equipPositionName>");
        stringBuilder.AppendLine();

        stringBuilder.Append("<equipTexPath>");
        stringBuilder.Append(equipTexPath);
        stringBuilder.Append("</equipTexPath>");
        stringBuilder.AppendLine();

        stringBuilder.Append("<alphaTexPath>");
        stringBuilder.Append(alphaTexPath);
        stringBuilder.Append("</alphaTexPath>");
        stringBuilder.AppendLine();

        stringBuilder.Append("<posDelta>");
        stringBuilder.Append(posDelta.x + "," + posDelta.y);
        stringBuilder.Append("</posDelta>");
        stringBuilder.AppendLine();

        stringBuilder.Append("<rotDelta>");
        stringBuilder.Append(rotDelta);
        stringBuilder.Append("</rotDelta>");
        stringBuilder.AppendLine();

        stringBuilder.AppendLine("</EquipData>");
        return stringBuilder.ToString();
    }
}

public enum StandardEquipType
{
    DEFAULT,//默认
    WEAPON,//基础
    SHIELD,
    ITEM,
    HEAD_FRONT,
    HEAD_BACK,
    BACK_DECORATION,
    BODY_FRONT,

    LEFT_FRONT_ARM,
    LEFT_BACK_ARM,
    LEFT_LEG,
    LEFT_FEET,

    RIGHT_FRONT_ARM,
    RIGHT_BACK_ARM,
    RIGHT_LEG,
    RIGHT_FEET,
    //——————非基础
    NONE_BASIC,
    CLOTHE,
    BODY,
    BODY_RIGHT_FRONT_ARM,
    BODY_RIGHT_SHOULDER,
    BODY_LEFT_FRONT_ARM,
    BODY_LEFT_SHOULDER,
    BODY_RIGHT_LEG,
    BODY_RIGHT_FEET,
    BODY_LEFT_LEG,
    BODY_LEFT_FEET,
    BODY_HAND,
    HEAD,
    HAIR,
    EYES,
    MOUTH,
}