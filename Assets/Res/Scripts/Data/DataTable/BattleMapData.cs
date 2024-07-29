using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMapData : AggregationEntity,IXMLPrintable
{
    public int randomMapSeed;
    public string troopName;
    public string relatedRogueTroop;
    public BattleMapTroopData[] legionDatas;

    public string PrintXML()
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        stringBuilder.AppendLine("<BattleMapData>");

        stringBuilder.Append("<randomMapSeed>");
        stringBuilder.Append(randomMapSeed);
        stringBuilder.Append("</randomMapSeed>");
        stringBuilder.AppendLine();

        stringBuilder.Append("<legionDatas>");
        foreach (IXMLPrintable data in legionDatas)
        {
            stringBuilder.Append(data.PrintXML());
        }
        stringBuilder.Append("</legionDatas>");
        stringBuilder.AppendLine();

        stringBuilder.AppendLine("</BattleMapData>");
        return stringBuilder.ToString();
    }
}

public class BattleMapTroopData: IXMLPrintable
{
    public string unitIdName;
    public int state;
    public int belong;
    public int posX;
    public int posY;

    public string PrintXML()
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        stringBuilder.AppendLine("<BattleMapTroopData>");

        stringBuilder.Append("<unitIdName>");
        stringBuilder.Append(unitIdName);
        stringBuilder.Append("</unitIdName>");
        stringBuilder.AppendLine();

        stringBuilder.Append("<belong>");
        stringBuilder.Append(belong);
        stringBuilder.Append("</belong>");
        stringBuilder.AppendLine();

        if (state > 0)
        {
            stringBuilder.Append("<state>");
            stringBuilder.Append(state);
            stringBuilder.Append("</state>");
            stringBuilder.AppendLine();
        }

        stringBuilder.Append("<posX>");
        stringBuilder.Append(posX);
        stringBuilder.Append("</posX>");
        stringBuilder.AppendLine();

        stringBuilder.Append("<posY>");
        stringBuilder.Append(posY);
        stringBuilder.Append("</posY>");
        stringBuilder.AppendLine();

        stringBuilder.AppendLine("</BattleMapTroopData>");
        return stringBuilder.ToString();
    }
}
