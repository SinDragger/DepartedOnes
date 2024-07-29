using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseTroopGet : UIPanel
{
    public override string uiPanelName => "ChooseTroopGet";
    public Text armyPointText;
    public SoldierPack[] troopSlots;
    public SoldierPackSlot[] troopPackSlots;
    public Text[] texts;
    public Text[] costs;
    public Button[] clickAbleArea;
    public Button continueButton;
    int nowChooseTarget;
    TroopEntity entity;

    public override void OnInit()
    {
        base.OnInit();
        continueButton.SetBtnEvent(() =>
        {
            OnHide();
            if (GameManager.instance.nowLayerCount % 2 == 1)//偶数
            {
                UIManager.Instance.ShowUI("ChooseMysteryMaster");
            }
            else//奇数
            {
                UIManager.Instance.ShowUI("ChooseNextPanel");
            }
        });
    }

    public override void OnShow(bool withAnim = true)
    {
        nowChooseTarget = -1;
        armyPointText.text = GameManager.instance.armyPointCount.ToString();
        continueButton.gameObject.SetActive(false);
        base.OnShow(withAnim);
        RandomSlotShow();
    }
    void RandomSlotShow()
    {
        var array = GameManager.instance.playerData.ableTroopList.ToArray();
        ArrayUtil.Shuffle(array);
        array[Random.Range(0, 3)] = "DriedSkeleton";
        for (int i = 0; i < 3; i++)
        {
            InitSlot(i, new TroopEntity(array[i]));
        }
    }

    public override void OnHide(bool withAnim = true)
    {
        GameManager.instance.playerData.AddNewAbleTroop(entity.originData, 1);
        //if(BattleManager.instance.)

        GameManager.instance.armyPointCount -= entity.originData.cost;
        base.OnHide(withAnim);
    }

    void InitSlot(int flag, TroopEntity troopEntity)
    {
        troopPackSlots[flag].Init(troopEntity, () =>
        {
            OnButtonClick(flag);
        });
    }

    void OnButtonClick(int flag)
    {
        if (GameManager.instance.armyPointCount < troopPackSlots[flag].troopEntity.originData.cost)
        {
            GameManager.instance.ShowTip("点数不足");
            return;
        }
        for (int i = 0; i < troopPackSlots.Length; i++)
        {
            troopPackSlots[i].OnSelect(i == flag);
        }
        entity = troopPackSlots[flag].troopEntity;
        nowChooseTarget = flag;
        continueButton.gameObject.SetActive(true);
    }
}