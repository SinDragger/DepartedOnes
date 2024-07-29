using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorBlockMap : AggregationEntity,IXMLPrintable
{
    public float CameraX;
    public float CameraY;
    /// <summary>
    /// 区域情况
    /// </summary>
    public SectorBlockData[] blockDatas;
    //建筑情况
    //基础部队情况

    //地图事件

    //势力在地图上的部署——以及区域占领
    public ForceDeployData[] forceDeployDatas;

    //部队出生点
    public LegionDeployData[] legionDeployDatas;
    public BuildingDeployData[] buildingDeployDatas;
    public SectorBlockData GetTargetSectorBlockData(Color color)
    {
        string colorString = ColorUtility.ToHtmlStringRGB(color);
        for (int i = 0; i < blockDatas.Length; i++)
        {
            if (blockDatas[i].recognizeColor == colorString)
            {
                return blockDatas[i];
            }
        }
        return null;
    }

    public string PrintXML()
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        stringBuilder.AppendLine($"<SectorBlockMap name=\"{idName}\">");

        stringBuilder.AppendLine("<blockDatas>");
        foreach (IXMLPrintable data in blockDatas)
        {
            stringBuilder.Append(data.PrintXML());
        }
        stringBuilder.AppendLine("</blockDatas>");
        stringBuilder.AppendLine("</SectorBlockMap>");
        return stringBuilder.ToString();
    }

    public void SynNewMapBlocks(Dictionary<Color, SectorBlock> blockDic)
    {
        if (blockDatas == null)
        {
            List<SectorBlockData> list = new List<SectorBlockData>();
            foreach (var b in blockDic)
            {
                string color = ColorUtility.ToHtmlStringRGB(b.Key);
                SectorBlockData block = new SectorBlockData();
                b.Value.originData = block;
                block.recognizeColor = color;
                list.Add(block);
            }
            blockDatas = list.ToArray();
        }
        else
        {
            //校验是否已废弃用在外在的
            //扩充已有的，移除没有的
            List<SectorBlockData> list = new List<SectorBlockData>();
            foreach (var b in blockDic)
            {
                string color = ColorUtility.ToHtmlStringRGB(b.Key);
                bool hasSyn = false;
                for (int i = 0; i < blockDatas.Length; i++)
                {
                    if (blockDatas[i].recognizeColor == color)
                    {
                        //存在 进行数据反同步
                        b.Value.SynOriginData(blockDatas[i]);
                        list.Add(blockDatas[i]);
                        hasSyn = true;
                        break;
                    }
                }
                if (!hasSyn)
                {
                    SectorBlockData block = new SectorBlockData();
                    block.recognizeColor = color;
                    list.Add(block);
                }
            }
            blockDatas = list.ToArray();
        }
        //同步建筑物信息
    }
}