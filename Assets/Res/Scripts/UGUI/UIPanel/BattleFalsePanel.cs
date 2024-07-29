using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleFalsePanel : UIPanel
{
    public override string uiPanelName => "BattleFalsePanel";
    public Button clickButton;
    public SkeletonGraphic mainCharacter;

    public override void OnInit()
    {
        clickButton.SetBtnEvent(() =>
        {
            OnHide();
        });
        RefreshUnitModel(mainCharacter, new TroopEntity("DepartedOnes_Player"));
        base.OnInit();
    }

    void RefreshUnitModel(SkeletonGraphic graphic, TroopEntity data)
    {
        SpineAtlasManager.Instance.SetSkeletonGraphicToUnit(graphic, data, GameManager.instance.belong);
        var model = DataBaseManager.Instance.GetIdNameDataFromList<EquipAbleModelData>(DataBaseManager.Instance.GetModelName(data));
        graphic.AnimationState.SetAnimation(0, "DeathBackward", false);
    }

    public override void OnShow(bool withAnim = true)
    {
        base.OnShow(withAnim);
    }

    public override void OnHide(bool withAnim = true)
    {
        base.OnHide(withAnim);
    }
}
