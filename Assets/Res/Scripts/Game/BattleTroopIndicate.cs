using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTroopIndicate : MonoBehaviour
{
    bool isShowAlly;
    public Button allyButton;
    public Button enermyButton;
    public Transform content;
    public GameObject viewPrefab;
    List<LegionTroopSlot> slots = new List<LegionTroopSlot>();
    List<CommandUnit> reflectCommanders;

    public void OnInit()
    {
        allyButton.SetBtnEvent(SwitchMode);
        enermyButton.SetBtnEvent(SwitchMode);
        SwitchMode();
        //TODO:监听部队增加
        UnitControlManager.instance.onNewCommandAdd += RefreshShow;
    }

    public void OnReset()
    {
        reflectCommanders = null;
        isShowAlly = true;
        UnitControlManager.instance.onNewCommandAdd -= RefreshShow;
    }

    void SwitchMode()
    {
        isShowAlly = !isShowAlly;
        allyButton.gameObject.SetActive(isShowAlly);
        enermyButton.gameObject.SetActive(!isShowAlly);
        RefreshShow();
    }
    void RefreshShow()
    {
        if (isShowAlly)
        {
            reflectCommanders = UnitControlManager.instance.GetAllianceCommanders();
        }
        else
        {
            reflectCommanders = UnitControlManager.instance.GetEnermyCommanders();
        }
        int count = 0;
        ArrayUtil.ListShowFit(slots, reflectCommanders, viewPrefab, content, (slot, data) =>
        {
            (slot.transform as RectTransform).anchoredPosition = new Vector2(-100f, 37.5f + 76f * count);
            slot.Init(data);
            count++;
        });
    }
    private void Update()
    {
        if (reflectCommanders == null) return;
        for (int i = 0; i < reflectCommanders.Count; i++)
        {
            slots[i].RefreshNumber(reflectCommanders[i]);
        }
        //TODO:增加每帧的刷新
    }
}
