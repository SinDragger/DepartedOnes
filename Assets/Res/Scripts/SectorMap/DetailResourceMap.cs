using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailResourceMap : MonoBehaviour
{
    List<ResourceShowSlot> showSlotList;
    public ResourceShowSlot originSlot;
    public Text equipNumText;
    public void Init(Dictionary<string,int> resourceDic)
    {
        if (showSlotList == null) showSlotList = new List<ResourceShowSlot>();
        for (int i = showSlotList.Count; i < resourceDic.Count; i++)
        {
            var slot = Instantiate(originSlot, originSlot.transform.parent).GetComponent<ResourceShowSlot>();
            showSlotList.Add(slot);
        }
        for (int i = resourceDic.Count; i < showSlotList.Count; i++)
        {
            showSlotList[i].gameObject.SetActive(false);
        }
        int flag = 0;
        foreach(var res in resourceDic)
        {
            showSlotList[flag].Init(res.Key, res.Value);
            showSlotList[flag].gameObject.SetActive(true);
            flag++;
        }
    }
    public void InitEquipStore(int number)
    {
        equipNumText.text = $"装备x{number}";
    }
}
