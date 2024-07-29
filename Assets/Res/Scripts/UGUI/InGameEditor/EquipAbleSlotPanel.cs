using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipAbleSlotPanel : MonoBehaviour
{
    public Text equipTitle;

    //模型总览面板
    public Transform content;
    public GameObject originSlot;
    List<EquipSlot> slots = new List<EquipSlot>();
    EquipSlot selectedTarget;
    EquipSlot selectedEquipTarget;

    //装备面板
    public Transform equipmentContent;
    /// <summary>
    /// 初始无装备面板不进行记录
    /// </summary>
    public GameObject originEquipmentSlot;
    List<EquipSlot> equipmentSlots = new List<EquipSlot>();

    EquipAbleModelData modelData;
    public void Init(EquipAbleModelData data, EquipSetData setData)
    {
        modelData = data;
        originSlot.gameObject.SetActive(false);
        var equipDataList = DataBaseManager.Instance.GetTargetDataList<EquipData>();
        List<EquipData> useEquips = new List<EquipData>();
        if (setData != null && setData.UseEquipTextures != null)
        {
            for (int i = 0; i < setData.UseEquipTextures.Length; i++)
            {
                useEquips.Add(equipDataList.Find((e) => e.idName.Equals(setData.UseEquipTextures[i])));
            }
        }
        for (int i = content.childCount - 1; i > 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }
        slots.Clear();
        for (int i = 0; i < data.DataArray.Length; i++)
        {
            var copy = Instantiate(originSlot, content);
            copy.gameObject.SetActive(true);
            var slot = copy.GetComponent<EquipSlot>();
            slots.Add(slot);
            slot.Init(data.DataArray[i].equipTypeName, data.DataArray[i].slotName);

            EquipData equipData = useEquips.Find((e) => e.equipPositionName.Equals(data.DataArray[i].equipTypeName));//TODO:改成复数？
            if (equipData != null)//存在可用的
            {
                Texture2D originTex = DataBaseManager.Instance.GetTexByIdName(equipData.equipTexPath);
                slot.iconImage.sprite = Sprite.Create(originTex, new Rect(0, 0, originTex.width, originTex.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                slot.iconImage.sprite = null;
            }
            slot.clickCallback = () =>
            {
                OnTargetSelect(copy, slot);
            };
        }
    }

    public void Refresh(EquipSetData setData)
    {
        List<EquipData> useEquips = new List<EquipData>();
        var equipDataList = DataBaseManager.Instance.GetTargetDataList<EquipData>();
        if (setData != null && setData.UseEquipTextures != null)
        {
            for (int i = 0; i < setData.UseEquipTextures.Length; i++)
            {
                useEquips.Add(equipDataList.Find((e) => e.idName.Equals(setData.UseEquipTextures[i])));
            }
        }
        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            EquipData equipData = useEquips.Find((e) => e.equipPositionName.Equals(modelData.DataArray[i].equipTypeName));//TODO:改成复数？
            if (equipData != null)//存在可用的
            {
                Texture2D originTex = DataBaseManager.Instance.GetTexByIdName(equipData.equipTexPath);
                slot.iconImage.sprite = Sprite.Create(originTex, new Rect(0, 0, originTex.width, originTex.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                slot.iconImage.sprite = null;
            }
        }
    }

    public void OnTargetSelect(GameObject target, EquipSlot slot)
    {
        if (selectedTarget != null)
        {
            if (selectedTarget != slot)
            {
                selectedTarget.OnSelected(false);
            }
        }
        selectedTarget = slot;
        if (selectedTarget != null)
        {
            selectedTarget.OnSelected(true);
            string equipType = selectedTarget.titleText.text;
            LoadEquipPanel(equipType);
        }
    }

    public static string nowEditorEquipType;
    public void LoadEquipPanel(string equipType)
    {
        selectedEquipTarget = null;
        equipTitle.text = equipType;
        nowEditorEquipType = equipType;
        var equipDataList = DataBaseManager.Instance.GetTargetDataList<EquipData>();
        equipDataList = equipDataList.FindAll((t) => t.equipPositionName.Equals(equipType));
        originEquipmentSlot.gameObject.SetActive(false);
        for (int i = equipmentContent.childCount - 1; i > 0; i--)
        {
            Destroy(equipmentContent.GetChild(i).gameObject);
        }
        for (int i = 0; i < equipDataList.Count; i++)
        {
            var copy = Instantiate(originEquipmentSlot, equipmentContent);
            copy.gameObject.SetActive(true);
            var slot = copy.GetComponent<EquipSlot>();
            equipmentSlots.Add(slot);
            slot.Init("", equipDataList[i].idName);
            EquipData equipData = equipDataList[i];//TODO:改成复数？
            if (equipData != null)//存在可用的
            {
                Texture2D originTex = DataBaseManager.Instance.GetTexByIdName(equipData.equipTexPath);
                slot.iconImage.sprite = Sprite.Create(originTex, new Rect(0, 0, originTex.width, originTex.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                slot.iconImage.sprite = null;
            }
            slot.clickCallback = () =>
            {
                OnTargetEquipSelect(copy, slot);
            };
        }
    }

    public void OnTargetEquipSelect(GameObject target, EquipSlot slot)
    {
        if (selectedEquipTarget != null)
        {
            if (selectedEquipTarget == slot)
            {
                selectedEquipTarget.OnSelected(false);
                TestCode.instance.LoadTargetEquip(null);
                selectedEquipTarget = null;
                return;
            }
            if (selectedEquipTarget != slot)
            {
                selectedEquipTarget.OnSelected(false);
            }
        }
        selectedEquipTarget = slot;
        if (selectedEquipTarget != null)
        {
            selectedEquipTarget.OnSelected(true);
            TestCode.instance.LoadTargetEquip(selectedEquipTarget.desText.text);
        }
    }
    public void TargetEquipSetup()
    {
        //通过nowEquipType定位当前在用的Slot
        //通过在用的slot的情况确定是否移除Set内的内容

        //然后附加

        //用EquipSet 影响EquipSwitch
    }

}
