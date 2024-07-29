using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipSetPanel : MonoBehaviour
{
    public Text equipSetNameText;
    /// <summary>
    /// 材质
    /// </summary>
    public Text materialText;
    /// <summary>
    /// 装备类型
    /// </summary>
    public Text equipType;
    /// <summary>
    /// 第一属性
    /// </summary>
    public Text Attribute_1;
    /// <summary>
    /// 第二属性
    /// </summary>
    public Text Attribute_2;
    /// <summary>
    /// 第三属性
    /// </summary>
    public Text Attribute_3;
    /// <summary>
    /// 负重
    /// </summary>
    public Text weightText;
    /// <summary>
    /// 工艺槽
    /// </summary>
    public List<GameObject> craftSlots;
    [HideInInspector]
    public EquipSetData equipSetData;

    public string attribute_1;
    public string attribute_2;
    public string attribute_3;
    public void Init(string equipSetId)
    {
        Init(DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>(equipSetId));
    }
    public void Init(EquipSetData equipSetData)
    {
        this.equipSetData = equipSetData;
        if (equipSetNameText)
            equipSetNameText.text = equipSetData.Name;
        if (materialText)
            materialText.text = equipSetData.Material;
        //if (equipType)
        //    equipType.text = equipSetData.EquipType;
        if (Attribute_1)
            Attribute_1.text = $"{equipSetData.GetIntValue(attribute_1)}";
        if (Attribute_2)
            Attribute_2.text = $"{equipSetData.GetIntValue(attribute_2)}";
        if (Attribute_3)
            Attribute_3.text = $"{equipSetData.GetIntValue(attribute_3)}%";
        if (weightText)
            weightText.text = $"{equipSetData.Weight}";

    }
}
