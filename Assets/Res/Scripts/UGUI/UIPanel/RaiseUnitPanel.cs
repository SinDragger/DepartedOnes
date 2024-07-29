using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaiseUnitPanel : UIPanel
{
    public override string uiPanelName => "RaiseUnitPanel";
    public LegionTroopSlot slot;
    public System.Action callback;
    public Button raiseButton;
    public Text singleCostText;
    public Text totalCostText;
    public Text maxAbleText;
    public NumberSwitch numberSwitch;
    public RecruitArea recruitArea;

    public Transform resContent;
    public GameObject resSlot;
    List<ItemSlot> itemSlots = new List<ItemSlot>();

    public Transform buttonResContent;
    public GameObject buttonResSlot;
    List<ItemSlot> buttonItemSlots = new List<ItemSlot>();

    LegionControl nowLegion;
    UnitData nowUnit;
    int cost;
    int nowNumber;
    int baseMax;
    public void Init(LegionControl legion, UnitData unitData, System.Action callback)
    {
        cost = 80;
        nowUnit = unitData;
        nowLegion = legion;
        var color = GameManager.instance.GetForceColor(GameManager.instance.belong);
        slot.numberColor.color = color;
        slot.Init(unitData);
        slot.SetNumber(unitData.soldierNum, unitData.soldierNum);
        singleCostText.text = cost.ToString();
        baseMax = LegionControl.maxSize - legion.troops.Count;
        CountMax();
        //numberSwitch.SetNumber(0);
        totalCostText.text = "0";
        //最大招募允许量
        raiseButton.SetBtnEvent(() =>
        {
            if (cost * nowNumber > GameManager.instance.soulPointCount)
            {
                GameManager.instance.ShowTip("灵魂能量不足");
            }
            else
            {
                OnRaise();
                OnHide();
                callback?.Invoke();
            }
        });
        recruitArea.onButtonClick = OnUnitTypeClick;
        numberSwitch.onValueChange = OnValueChange;
        OnUnitTypeClick(unitData);
        OnValueChange(0);
    }

    public void OnRaise()
    {
        ResourcePool resPool = null;
        resPool = GameManager.instance.playerForce.resourcePool;
        //if (nowLegion.isOnFriendLand)
        //{
        //    //仓库库存验资
        //    resPool = GameManager.instance.playerForce.resourcePool;
        //}
        //else
        //{
        //    resPool = nowLegion.resourcePool;
        //    //自己库存验资
        //}
        resPool.ResourceConsume(nowUnit.resContain, nowUnit.soldierNum * nowNumber);
        GameManager.instance.playerData.soulPoint -= cost * nowNumber;
        for (int i = 0; i < nowNumber; i++)
        {
            var troop = new TroopControl(nowUnit, nowUnit.soldierNum, nowLegion.belong);
            nowLegion.AddTroop(troop);
            EventManager.Instance.DispatchEvent(new EventData(GameEventType.OnPlayerGetTroop, Constant_QuestEventDataKey.PlayerGetTroop, troop));
            QuestManager.Instance.CheckCompleteQuest();
        }
    }

    void CountMax()
    {
        int soulPointMax = GameManager.instance.soulPointCount / cost;
        int resMax = baseMax;
        //检测每个able
        ResourcePool resPool = null;
        resPool = GameManager.instance.playerForce.resourcePool;
        //if (nowLegion.isOnFriendLand)
        //{
        //    //仓库库存验资
        //    resPool = GameManager.instance.playerForce.resourcePool;
        //}
        //else
        //{
        //    resPool = nowLegion.resourcePool;
        //    //自己库存验资
        //}
        int max = Mathf.Min(resMax, resPool.MaxAble(nowUnit.resContain) / nowUnit.soldierNum, soulPointMax);
        numberSwitch.SetMax(max);
        maxAbleText.text = $"/ {max}";
    }

    public void OnSwitchClick()
    {
        recruitArea.Show();
    }

    public void OnUnitTypeClick(UnitData unitData)
    {
        slot.Init(unitData);
        nowUnit = unitData;
        recruitArea.Hide();
        ArrayUtil.ListShowFit(itemSlots, unitData.resContain, resSlot, resContent, (slot, data) =>
        {
            slot.gameObject.SetActive(true);
            slot.SetImage(DataBaseManager.Instance.GetSpriteByIdName(data.idName));
            slot.SetText((data.num * unitData.soldierNum).ToString());
        });
        CountMax();
        numberSwitch.SetNumber(0);
        if (numberSwitch.max > 0)
        {
            nowNumber = 1;
        }
        else
        {
            nowNumber = 0;
        }
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

    public void OnValueChange(int value)
    {
        nowNumber = value;
        totalCostText.text = (cost * nowNumber).ToString();
        ArrayUtil.ListShowFit(buttonItemSlots, nowUnit.resContain, buttonResSlot, buttonResContent, (slot, data) =>
        {
            slot.gameObject.SetActive(true);
            slot.SetImage(DataBaseManager.Instance.GetSpriteByIdName(data.idName));
            slot.SetText((data.num * nowUnit.soldierNum * nowNumber).ToString());
        });
    }
}
