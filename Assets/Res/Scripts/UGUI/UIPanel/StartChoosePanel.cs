using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartChoosePanel : UIPanel
{
    public override string uiPanelName => "StartChoosePanel";
    public SkeletonGraphic mainCharacter;
    public SoldierPack[] packs;
    public List<LegionTroopSlot> slots;
    public ItemSlot castSpellSlot;
    public Button startBattleButton;
    public Button returnMapButton;
    public BattleMapTroopData[] legionDatas;
    public override void OnInit()
    {
        base.OnInit();
        startBattleButton.SetBtnEvent(() =>
        {
            OnHide();
            legionDatas = new BattleMapTroopData[2];
            for (int i = 0; i < legionDatas.Length; i++)
            {
                legionDatas[i] = new BattleMapTroopData();
                legionDatas[i].unitIdName = "DriedSkeleton";
                legionDatas[i].belong = GameManager.instance.belong;
                legionDatas[i].posX = 22 + (legionDatas.Length / 2 * -4) + i * 4;
                if (legionDatas.Length % 2 == 0)
                {
                    legionDatas[i].posX += 2;
                }
                legionDatas[i].posY = 11;
            }
            GameManager.instance.playerData.selfLegionDatas = new List<BattleMapTroopData>(legionDatas);
            GameManager.instance.EnterRogueBattle(0);
        });
        returnMapButton.SetBtnEvent(() =>
        {
            OnHide();
            SceneManager.Instance.BackToStartMenu(0f);
        });
    }
    public override void OnShow(bool withAnim = true)
    {
        RefreshUnitModel(mainCharacter, new TroopEntity("DepartedOnes_Player"));
        var spell = DataBaseManager.Instance.GetIdNameDataFromList<CastableSpellData>("RaiseCorpse");
        castSpellSlot.GetComponent<SpellPanelTip>().spell = spell;
        var initEntity = new TroopEntity("DriedSkeleton");
        foreach (var pack in packs)
        {
            pack.Init(initEntity);
        }
        if (slots != null)
        {
            foreach (var slot in slots)
            {
                slot.Init(initEntity);
                var color = GameManager.instance.GetForceColor(GameManager.instance.belong);
                slot.numberColor.color = color;
            }
        }
        base.OnShow(withAnim);
    }

    void RefreshUnitModel(SkeletonGraphic graphic, TroopEntity data)
    {
        SpineAtlasManager.Instance.SetSkeletonGraphicToUnit(graphic, data, GameManager.instance.belong);
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
    }

}
