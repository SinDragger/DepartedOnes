using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 地图数据内容：包括地形、资源分布、初始建筑物内容索引
/// </summary>
public class SectorBlockData : AggregationEntity,IXMLPrintable
{
    /// <summary>
    /// 识别色 RGB
    /// </summary>
    public string recognizeColor;
    /// <summary>
    /// 地形类别
    /// </summary>
    public string landform;
    /// <summary>
    /// 控制程度
    /// </summary>
    public EntityStack[] controlProcess;

    public int hideLevel;
    public string PrintXML()
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        stringBuilder.AppendLine($"<SectorBlockData>");

        stringBuilder.Append("<recognizeColor>");
        stringBuilder.Append(recognizeColor);
        stringBuilder.Append("</recognizeColor>");
        stringBuilder.AppendLine();

        if (!string.IsNullOrEmpty(landform))
        {
            stringBuilder.Append("<landform>");
            stringBuilder.Append(landform);
            stringBuilder.Append("</landform>");
            stringBuilder.AppendLine();
        }

        stringBuilder.AppendLine("</SectorBlockData>");
        return stringBuilder.ToString();
    }
}
