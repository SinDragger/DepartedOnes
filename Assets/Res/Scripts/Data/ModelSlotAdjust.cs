using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 模型的调整参数
/// </summary>
[CreateAssetMenu(fileName = "ModelSlotAdjust", menuName = "Data/ModelSlotAdjust")]
public class ModelSlotAdjust : ScriptableObject
{
    public int modelSlotId;

    //增设modelName
    public ModelSuitData[] dataArray;

    public ModelSuitData GetSuitData(string slotName)
    {
        foreach (var data in dataArray)
        {
            if (data.slotName == slotName) return data;
        }
        return null;
    }
    public void SetSuitData(string slotName, Vector2 delta, float rotation)
    {
        ModelSuitData[] copy = (ModelSuitData[])dataArray.Clone();
        dataArray = new ModelSuitData[copy.Length + 1];
        for (int i = 0; i < copy.Length; i++)
        {
            dataArray[i] = copy[i];
        }
        dataArray[dataArray.Length - 1] = new ModelSuitData(slotName, delta, rotation);

    }
    public void AddSuitData(string slotName, Vector2 delta, float rotation)
    {
        Debug.LogError("Trigger");
        for (int i = 0; i < dataArray.Length; i++)
        {
            if (dataArray[i].slotName == slotName)
            {
                dataArray[i] = new ModelSuitData(slotName, dataArray[i].posDelta + delta, dataArray[i].rotationDelta + rotation);
                break;
            }
        }
    }

    public string GetDefaultId()
    {
        string result = modelSlotId.ToString();
        for (int i = 0; i < dataArray.Length; i++)
        {
            result += "-0";
        }
        return result;
    }

    public string OutPutXML()
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        stringBuilder.AppendLine("<DataArray>");
        for (int i = 0; i < dataArray.Length; i++)
        {
            stringBuilder.AppendLine("<ModelSuitData>");

            stringBuilder.Append("<slotName>");
            stringBuilder.Append(dataArray[i].slotName);
            stringBuilder.Append("</slotName>");
            stringBuilder.AppendLine();

            stringBuilder.Append("<equipTypeName>");
            stringBuilder.Append(dataArray[i].equipTypeName);
            stringBuilder.Append("</equipTypeName>");
            stringBuilder.AppendLine();

            stringBuilder.Append("<posDelta>");
            stringBuilder.Append(dataArray[i].posDelta.x + "," + dataArray[i].posDelta.y);
            stringBuilder.Append("</posDelta>");
            stringBuilder.AppendLine();

            stringBuilder.Append("<rotationDelta>");
            stringBuilder.Append(dataArray[i].rotationDelta);
            stringBuilder.Append("</rotationDelta>");
            stringBuilder.AppendLine();

            stringBuilder.AppendLine("</ModelSuitData>");
        }
        stringBuilder.AppendLine("</DataArray>");
        return stringBuilder.ToString();
    }
}
