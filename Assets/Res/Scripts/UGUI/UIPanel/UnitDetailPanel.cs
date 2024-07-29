using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class UnitDetailPanel : UIPanel
{
    public override string uiPanelName => "UnitDetailPanel";
    public GameObject showPanel;
    public Text titleText;
    GameObject createdTarget;
    public LegionTroopSlot showSlot;
    public SkeletonGraphic graphic;
    public GameObject attributePanel;

    public Text speciesText;
    public UnitAttributeArea unitAttributeArea;
    /// <summary>
    /// TODO:需要完善并发布
    /// </summary>
    public TextUnit costText;
    public Text workLoadText;
    public GameObject workLoadImage;
    public EquipSetPanel weaponPanel;
    public EquipSetPanel armourPanel;
    TroopControl lastTroop;
    LegionControl nowLegion;
    //装备更替相关
    public EquipmentChangeArea changeArea;
    public GameObject blackBg;
    public Button weaponChangeButton;
    public Button armourChangeButton;
    public Button replenishButton;
    public void InitByTroop(TroopControl troop)
    {
        if (lastTroop == troop) return;
        nowLegion = LegionManager.Instance.nowLegion;
        bool needRefresh = true;
        lastTroop = troop;
        titleText.text = troop.troopEntity.originData.name;
        speciesText.text = DataBaseManager.Instance.GetIdNameDataFromList<Species>(troop.troopEntity.speciesType).name;
        //TODO:增加种族不同的两边特性与逻辑修正
        unitAttributeArea.Init(lastTroop.troopEntity);
        weaponPanel.Init(lastTroop.troopEntity.weaponEquipSet.data);
        armourPanel.Init(lastTroop.troopEntity.armourEquipSet.data);
        Dictionary<string, int> totalCost = lastTroop.troopEntity.GetEquipTotalCost();
        int targetWorkLoad = weaponPanel.equipSetData.WorkLoad + armourPanel.equipSetData.WorkLoad;
        workLoadText.text = $"{targetWorkLoad}";
        workLoadText.gameObject.SetActive(targetWorkLoad != 0);
        workLoadImage.gameObject.SetActive(targetWorkLoad != 0);
        costText.SpriteClear();
        int flag = 0;
        string textString = "";
        foreach (var entity in totalCost)
        {
            costText.SetSprite(flag, DataBaseManager.Instance.GetSpriteByIdName(DataBaseManager.Instance.GetIdNameDataFromList<Resource>(entity.Key).idName));
            textString += $"{"<quad>"}{entity.Value} ";
            flag++;
        }
        costText.SetText(textString);
        //自己人
        if (false)//nowLegion.belong == GameManager.instance.belong
        {
            weaponChangeButton.gameObject.SetActive(true);
            armourChangeButton.gameObject.SetActive(true);
            replenishButton.gameObject.SetActive(true);
            weaponChangeButton.SetBtnEvent(OnChangeWeaponClick);
            armourChangeButton.SetBtnEvent(OnChangeArmourClick);
            replenishButton.SetBtnEvent(OnReplenishButtonClick);
            blackBg.GetComponent<Button>().SetBtnEvent(OnCloseChangeClick);
        }
        else
        {
            weaponChangeButton.gameObject.SetActive(false);
            armourChangeButton.gameObject.SetActive(false);
            replenishButton.gameObject.SetActive(false);
        }
        if (needRefresh) RefreshUnitModel(troop, troop.troopEntity);
        //根据unit的类型进行目标Type的加载
    }

    void RefreshUnitModel(TroopControl troop, TroopEntity data)
    {
        showSlot.Init(troop);
        SpineAtlasManager.Instance.SetSkeletonGraphicToUnit(graphic, troop);
        var model = DataBaseManager.Instance.GetIdNameDataFromList<EquipAbleModelData>(DataBaseManager.Instance.GetModelName(data));
        var actionMotion = data.weaponEquipSet != null ? data.weaponEquipSet.data.TargetActionModel : "MELEE";
        string targetMotionName = model.idleMotionName_Melee;
        if (System.Enum.TryParse<ModelMotionType>(actionMotion, out ModelMotionType result))
        {
            switch (result)
            {
                case ModelMotionType.MELEE:
                    targetMotionName = model.idleMotionName_Melee;
                    break;
                case ModelMotionType.POLEARMS:
                    targetMotionName = model.idleMotionName_Polearm;
                    break;
                case ModelMotionType.TWOHANDED:
                    targetMotionName = model.idleMotionName_TwoHanded;
                    break;
            }
        }
        graphic.AnimationState.SetAnimation(0, targetMotionName, true);
        if (troop.IsEquipSet())
        {
            attributePanel.SetActive(false);
            graphic.timeScale = 0f;
        }
        else
        {
            attributePanel.SetActive(true);
            graphic.timeScale = 1f;
        }
    }

    public override void OnShow(bool withAnim = true)
    {
        showPanel.gameObject.SetActive(true);
        TimeManager.Instance.SetToStop();
        base.OnShow(withAnim);
    }

    public override void OnHide(bool withAnim = true)
    {
        showPanel.gameObject.SetActive(false);
        TimeManager.Instance.SetToRecovery();
        if (createdTarget != null)
        {
            Destroy(createdTarget.gameObject);
            createdTarget = null;
        }
        base.OnHide(withAnim);
    }

    public void OnChangeWeaponClick()
    {
        blackBg.SetActive(true);
        changeArea.gameObject.SetActive(true);
        List<(EquipSetData, int)> data = new List<(EquipSetData, int)>();
        foreach (var value in nowLegion.equipWeaponStore)
        {
            data.Add((value.Key, value.Value));
        }

        //TODO：优化成从势力实例获得数据
        //foreach (var id in DataBaseManager.Instance.GetTargetDataList<ForceData>().Find(i => i.id == nowLegion.belong).basicUnlimitedWeaponIds)
        //{
        //    data.Add((DataBaseManager.Instance.GetTargetDataList<EquipSetData>().Find(i => i.idName == id), 999));
        //}
        List<string> unlimitedEquip = new List<string>(GameManager.instance.playerForce.data.basicUnlimitedWeaponIds);
        foreach (var id in GameManager.instance.playerForce.data.basicUnlimitedWeaponIds)
        {
            data.Add((DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>(id), -1));
        }

        data.Sort((a, b) => a.Item2.CompareTo(b.Item2));

        EquipSetData baseWeapon = DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>("BareHand");
        changeArea.Init(data, (equip) =>
        {
            if (equip == lastTroop.troopEntity.weaponEquipSet.data)
            {
                if(!unlimitedEquip.Contains(equip.idName))
                    nowLegion.ChangeEquip(equip, lastTroop.nowNum, true);
                lastTroop.troopEntity.ChangeWeapon(baseWeapon);
                OnChangeWeaponClick();
                RefreshUnitModel(lastTroop, lastTroop.troopEntity);
                nowLegion.uiDataChanged = true;
            }
            else if (unlimitedEquip.Contains(equip.idName))
            {
                var equipData = lastTroop.troopEntity.weaponEquipSet.data;
                if (equip != equipData && !unlimitedEquip.Contains(equipData.idName))
                    nowLegion.ChangeEquip(lastTroop.troopEntity.weaponEquipSet.data, lastTroop.nowNum, true);
                lastTroop.troopEntity.ChangeWeapon(equip);
                weaponPanel.Init(equip);
                OnChangeWeaponClick();
                RefreshUnitModel(lastTroop, lastTroop.troopEntity);
                nowLegion.uiDataChanged = true;
            }
            else if (nowLegion.equipWeaponStore[equip] >= lastTroop.nowNum)
            {
                nowLegion.ChangeEquip(lastTroop.troopEntity.weaponEquipSet.data, lastTroop.nowNum, true);
                nowLegion.ChangeEquip(equip, -lastTroop.nowNum, true);
                lastTroop.troopEntity.ChangeWeapon(equip);
                //UI刷新
                weaponPanel.Init(equip);
                OnChangeWeaponClick();
                RefreshUnitModel(lastTroop, lastTroop.troopEntity);
                nowLegion.uiDataChanged = true;
            }
            else
            {
                GameManager.instance.ShowTip("数量不足");
            }
        });
    }

    public void OnChangeArmourClick()
    {
        blackBg.SetActive(true);
        changeArea.gameObject.SetActive(true);
        List<(EquipSetData, int)> data = new List<(EquipSetData, int)>();
        foreach (var value in nowLegion.equipArmourStore)
        {
            data.Add((value.Key, value.Value));
        }
        data.Sort((a, b) => a.Item2.CompareTo(b.Item2));

        EquipSetData baseArmour = DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>("Clothe");
        changeArea.Init(data, (equip) =>
        {
            if (equip == lastTroop.troopEntity.armourEquipSet.data)
            {
                nowLegion.ChangeEquip(equip, lastTroop.nowNum, true);
                lastTroop.troopEntity.ChangeArmour(baseArmour);
                OnChangeArmourClick();
                RefreshUnitModel(lastTroop, lastTroop.troopEntity);
                nowLegion.uiDataChanged = true;
            }
            else if (nowLegion.equipArmourStore[equip] >= lastTroop.nowNum)
            {
                nowLegion.ChangeEquip(lastTroop.troopEntity.armourEquipSet.data, lastTroop.nowNum, true);
                nowLegion.ChangeEquip(equip, -lastTroop.nowNum, true);
                lastTroop.troopEntity.ChangeArmour(equip);
                //UI刷新
                weaponPanel.Init(equip);
                OnChangeArmourClick();
                RefreshUnitModel(lastTroop, lastTroop.troopEntity);
                nowLegion.uiDataChanged = true;
            }
            else
            {
                GameManager.instance.ShowTip("数量不足");
            }
        });
    }

    public void OnReplenishButtonClick()
    {
        int cost = (lastTroop.maxNum - lastTroop.nowNum) * 2;
        if (GameManager.instance.playerForce.GetLimitedRes(Constant_AttributeString.RES_SOULPOINT) >= cost)
        {
            lastTroop.nowNum = lastTroop.maxNum;
            GameManager.instance.playerForce.SetLimitedRes(Constant_AttributeString.RES_SOULPOINT, -cost);
            nowLegion.uiDataChanged = true;
        }
        else
        {
            GameManager.instance.ShowTip("灵魂不足");
        }
    }

    public void OnCloseChangeClick()
    {
        blackBg.SetActive(false);
        changeArea.gameObject.SetActive(false);
    }
}
