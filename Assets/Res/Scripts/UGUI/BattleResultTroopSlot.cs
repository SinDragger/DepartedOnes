using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleResultTroopSlot : MonoBehaviour
{
    public LegionTroopSlot slot;
    public CanvasGroup selfAlpha;
    public GameObject preview;
    public GameObject res;
    public GameObject resOriginPrefab;
    List<ResourceShowSlot> slots = new List<ResourceShowSlot>();
    [HideInInspector]
    public int number;
    public void Init(TroopControl troop, int originNum)
    {
        slot.Init(troop);
        number = originNum;
        slot.SetNumber(troop.maxNum, originNum);
    }


    public void SetDrag(System.Action onDragStart = null, System.Action onDrag = null, System.Func<bool> onDragEnd = null)
    {
        var dragAbleObject = slot.GetComponent<DragAbleObject>();
        dragAbleObject.onDragStart.AddListener(() => { onDragStart?.Invoke(); });
        dragAbleObject.onDraging.AddListener((v) => { onDrag?.Invoke(); });
        dragAbleObject.onDragEnd.AddListener(() =>
        {
            if (onDragEnd.Invoke())
            {

            }
            else
            {
                //dragAbleObject.transform.localPosition = dragAbleObject.startDragPos;
            }
        });
    }

    public void ShowPreview(Sprite icon, int number)
    {
        preview.gameObject.SetActive(true);
        preview.GetComponent<Image>().sprite = icon;
        preview.GetComponentInChildren<Text>().text = number.ToString();
    }

    public void ShowRes(TroopControl data, int number)
    {
        List<EntityStack> resList = new List<EntityStack>();
        if (data.troopEntity.originData.resContain != null)
        {
            for (int i = 0; i < data.troopEntity.originData.resContain.Length; i++)
            {
                resList.Add(data.troopEntity.originData.resContain[i]);
            }
        }
        ShowRes(resList, number);
    }

    public void ShowRes(List<EntityStack> resList, int number)
    {
        slot.gameObject.SetActive(false);
        res.SetActive(true);
        ArrayUtil.ListShowFit(slots, resList, resOriginPrefab, resOriginPrefab.transform.parent, (slot, data) =>
        {
            slot.Init(data.idName, data.num * number);
        });
    }

    public void OnReset()
    {
        preview.gameObject.SetActive(false);
        res.gameObject.SetActive(false);
    }
}
