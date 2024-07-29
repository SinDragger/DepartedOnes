using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecruitArea : MonoBehaviour
{
    List<UnitData> unitList;
    public List<RecruitSlot> slots;
    public GameObject slotPrefab;
    public Transform content;
    public System.Action<UnitData> onButtonClick;
    public void Show()
    {
        gameObject.SetActive(true);
        unitList = GameManager.instance.playerForce.unitList;
        ArrayUtil.ListShowFit<RecruitSlot, UnitData>(slots, unitList, slotPrefab, content, (slot, data) =>
        {
            slot.gameObject.SetActive(true);
            slot.Init(data);
            slot.clickButton.SetBtnEvent(() => TryRecruitTargetUnitTroop(data));
        });
    }

    void TryRecruitTargetUnitTroop(UnitData unit)
    {
        onButtonClick?.Invoke(unit);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
