using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 区域
/// </summary>
public class BattleResultArea : MonoBehaviour
{
    public Image icon;
    public DragReceiveArea receiveArea;
    public Transform content;
    List<LegionTroopSlot> slots = new List<LegionTroopSlot>();
    int flag = 0;
    public bool isFocus => receiveArea.isFocus;

    /// <summary>
    /// 拖拽部队至其上
    /// </summary>
    public void ReceiveSlot(LegionTroopSlot slot)
    {
        slot.transform.SetParent(content);
        slot.transform.localScale = Vector3.one;
        slot.transform.position = transform.position;
        slots.Add(slot);
        RefreshPos();
    }

    public void RemoveSlot(LegionTroopSlot slot)
    {
        slots.Remove(slot);
        RefreshPos();
    }

    void RefreshPos()
    {
        float start = 30f;
        float end = -50f;
        if (slots.Count < 3)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].transform.localPosition = new Vector3(0f, 30 - 40 * i, 0f);
            }
        }
        else
        {

            float delta = (start - end) / (slots.Count - 1);
            for (int i = 1; i < slots.Count - 1; i++)
            {
                slots[i].transform.localPosition = new Vector3(0f, 30 - delta * i, 0f);
            }
        }
    }
    public void OnReset()
    {
        slots.Clear();
    }
}
