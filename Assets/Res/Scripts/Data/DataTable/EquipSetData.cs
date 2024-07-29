using System;
using System.Collections.Generic;

/// <summary>
/// 套装装备数据
/// </summary>
[Serializable]
public partial class EquipSetData : AggregationEntity, IXMLPrintable
{
    /// <summary>
    /// 序号
    /// </summary>
    public int ID;
    /// <summary>
    /// 原型款序号-用于继承识别与分组
    /// </summary>
    public string PrototypeID;
    public string Name;
    public string TargetActionModel;
    public string damageEffectPrefab;
    //武器——
    //基础攻速
    //攻击范围
    //伤害值
    //盾牌设定——抵挡所互为交战方的攻击/防御外界护盾

    public string[] UseEquipTextures;
    public EquipSetData Clone()
    {
        EquipSetData result = new EquipSetData();
        result.ID = ID;
        result.idName = idName;
        result.TargetActionModel = TargetActionModel;
        if (UseEquipTextures != null)
            result.UseEquipTextures = (string[])UseEquipTextures.Clone();
        return result;
    }

    public void ChangeEquip(string equipPosition, string equipName)
    {
        //从装备列表中搜寻
        var list = DataBaseManager.Instance.GetTargetDataList<EquipData>();
        list = list.FindAll((e) => e.equipPositionName.Equals(equipPosition));//装备位置剥离子集

        if (string.IsNullOrEmpty(equipName))
        {
            if (UseEquipTextures == null) return;
            List<string> tempList = new List<string>(UseEquipTextures);
            for (int i = 0; i < UseEquipTextures.Length; i++)
            {
                var e = list.Find((a) => a.idName == UseEquipTextures[i]);
                if (e != null)//搜寻
                {
                    tempList.RemoveAt(i);
                    UseEquipTextures = tempList.ToArray();
                    return;
                }
            }
            return;
        }
        if (UseEquipTextures != null)
        {
            for (int i = 0; i < UseEquipTextures.Length; i++)
            {
                var e = list.Find((a) => a.idName == UseEquipTextures[i]);
                if (e != null)//搜寻
                {
                    UseEquipTextures[i] = equipName;
                    return;
                }
            }
            string[] newArray = new string[UseEquipTextures.Length + 1];
            for (int i = 0; i < UseEquipTextures.Length; i++)
            {
                newArray[i] = UseEquipTextures[i];
            }
            newArray[UseEquipTextures.Length] = equipName;
            UseEquipTextures = newArray;
        }
        else
        {
            UseEquipTextures = new string[1];
            UseEquipTextures[0] = equipName;
        }
    }

    public string PrintXML()
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        if (string.IsNullOrEmpty(idName))
        {
            stringBuilder.AppendLine("<EquipSetData>");
        }
        else
        {
            stringBuilder.AppendLine($"<EquipSetData name=\"{idName}\">");
        }

        stringBuilder.Append("<ID>");
        stringBuilder.Append(ID);
        stringBuilder.Append("</ID>");
        stringBuilder.AppendLine();

        stringBuilder.Append("<SetName>");
        stringBuilder.Append(Name);
        stringBuilder.Append("</SetName>");
        stringBuilder.AppendLine();

        stringBuilder.Append("<TargetActionModel>");
        stringBuilder.Append(TargetActionModel);
        stringBuilder.Append("</TargetActionModel>");
        stringBuilder.AppendLine();

        stringBuilder.AppendLine("<UseEquips>");
        foreach (var value in UseEquipTextures)
        {
            stringBuilder.AppendLine(value);
        }
        stringBuilder.AppendLine("</UseEquips>");

        stringBuilder.AppendLine("</EquipSetData>");
        return stringBuilder.ToString();
    }


    public string Print()
    {
        return $"{ID} {Name} {TargetActionModel} {UseEquipTextures}";
    }
}