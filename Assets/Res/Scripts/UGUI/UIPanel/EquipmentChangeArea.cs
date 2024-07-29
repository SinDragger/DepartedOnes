using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentChangeArea : MonoBehaviour
{
    public GameObject equipmentSlot;
    public Transform equipmentContent;
    public List<EquipSetData> equipmentDatas;
    public List<EquipSetPanel> equipmentSlots = new List<EquipSetPanel>();

    public void Init(List<(EquipSetData,int)> datas,System.Action<EquipSetData> onClick)
    {
        ArrayUtil.ListShowFit(equipmentSlots, datas, equipmentSlot, equipmentContent, (slot, data) =>
        {
            slot.gameObject.SetActive(true);
            slot.Init(data.Item1);
            if (data.Item2 == -1)
                slot.materialText.text = "∞";
            else
                slot.materialText.text = data.Item2.ToString();
            slot.GetComponent<Button>().SetBtnEvent(() =>
            {
                onClick?.Invoke(data.Item1);
            });
        });
    }
}
