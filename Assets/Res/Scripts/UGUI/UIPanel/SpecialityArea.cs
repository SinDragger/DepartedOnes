using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialityArea : MonoBehaviour
{
    public Transform content;
    public GameObject slotPrefab;
    public List<StandardStatus> statusList = new List<StandardStatus>();
    public List<StatusSlot> statusSlotList = new List<StatusSlot>();
    public void Init(List<StandardStatus> specialities)
    {
        ArrayUtil.ListShowFit(statusSlotList, specialities, slotPrefab, content, (slot, data) =>
        {
            slot.Init(data);
            slot.gameObject.SetActive(true);
        });
    }
}
