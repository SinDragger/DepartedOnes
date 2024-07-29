using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPanelSlot : MonoBehaviour
{
    public Text numberText;
    public Text titleText;
    public LegionTroopSlot slot;
    public GameObject selectBorder;

    public void Init(UnitData unitData, string speicesType)
    {
        slot.Init(unitData, speicesType);
        titleText.text = unitData.name;
        //numberText.text = 统计所有Legion里该Type该种族的数量
    }
    public void Init(UnitData unitData)
    {
        slot.Init(unitData, unitData.speciesType);
        titleText.text = unitData.name;
    }

    public void SetOnClick(System.Action onClick)
    {
        GetComponent<Button>().SetBtnEvent(() => onClick?.Invoke());
    }
    public void OnSelected(bool value)
    {
        selectBorder.gameObject.SetActive(value);
    }
}
