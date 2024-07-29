using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaiseGetTroopPanel : UIPanel
{
    public override string uiPanelName => "RaiseGetTroopPanel";
    public LegionTroopSlot slot;
    public System.Action callback;
    public Button enter;

    public void Init(LegionControl legion, UnitData unitData,System.Action callback)
    {
        var color = GameManager.instance.GetForceColor(GameManager.instance.belong);
        slot.numberColor.color = color;
        slot.Init(unitData);
        slot.SetNumber(unitData.soldierNum,unitData.soldierNum);
        enter.SetBtnEvent(() =>
        {
            legion.AddTroop(new TroopControl(unitData, unitData.soldierNum, legion.belong));

            OnHide();
            callback?.Invoke();
        });
    }

    public override void OnShow(bool withAnim = true)
    {
        TimeManager.Instance.SetToStop();
        base.OnShow(withAnim);
    }

    public override void OnHide(bool withAnim = true)
    {
        TimeManager.Instance.SetToRecovery();
        base.OnHide(withAnim);
    }
}
