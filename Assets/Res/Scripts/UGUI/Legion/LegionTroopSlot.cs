using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LegionTroopSlot : MonoBehaviour
{
    public SkeletonGraphic graphic;
    public Image background;
    public Image numberFill;
    public Image numberColor;
    public GameObject targetGraphicObject;
    //给予基础SkeletonData与EquipSet自动生成
    int equipBelong;
    public Text numberText;
    public Text nameText;
    public TroopControl troopData;
    string weaponId;
    string armourId;
    public void Init(TroopControl troop, bool fullShow = true)
    {
        int nowNumber = troop.nowNum;
        SpeciesTypeFit(troop.troopEntity.speciesType, GameManager.instance.GetForceColor(troop.belong), () =>//ColorManager.Instance.GetColor(troop.unitType)
        {
            SetNumber(troop.maxNum, nowNumber);
        });
        if (numberText != null) numberText.text = nowNumber.ToString();
        if (troop == troopData && troop.troopEntity.weaponEquipSet.data.idName == weaponId && troop.troopEntity.armourEquipSet.data.idName == armourId) return;
        weaponId = troop.troopEntity.weaponEquipSet.data.idName;
        armourId = troop.troopEntity.armourEquipSet.data.idName;
        troopData = troop;
        equipBelong = troop.belong;
        if (fullShow)
        {
            Init(troop.troopEntity);
            targetGraphicObject.gameObject.SetActive(true);
        }
        else
        {
            targetGraphicObject.gameObject.SetActive(false);
        }
    }

    void SpeciesTypeFit(string speciesType, Color color, System.Action normalAction = null)
    {
        bool normalSize = true;
        if (speciesType == Constant_AttributeString.SPECIES_EQUIP)
        {
            numberColor.color = Color.gray;
            numberFill.fillAmount = 1;
        }
        else
        {
            numberColor.color = color;
            normalAction?.Invoke();
            if (speciesType == "Beast")
            {
                normalSize = false;
            }
        }
        if (normalSize)
        {
            targetGraphicObject.transform.localPosition = new Vector3(-45, -80, 0);
        }
        else
        {
            targetGraphicObject.transform.localPosition = new Vector3(-35, -65, 0);
        }
    }

    public virtual void Init(CommandUnit commandUnit)
    {
        string speciesType = commandUnit.entityData.speciesType;
        SpeciesTypeFit(speciesType, GameManager.instance.GetForceColor(commandUnit.belong), () =>
        {
            SetNumber(commandUnit.troopsData.Count, commandUnit.aliveCount);
        });
        Init(commandUnit.entityData);
        targetGraphicObject.gameObject.SetActive(true);
        MotionFit(commandUnit.entityData);
    }

    /// <summary>
    /// 基础的底层适配
    /// </summary>
    /// <param name="unitData"></param>
    /// <param name="speciesType"></param>
    public void Init(UnitData unitData, string speciesType)
    {
        Init(unitData, speciesType, unitData.subSpeciesType);
    }
    public void Init(UnitData unitData)
    {
        Init(unitData, unitData.speciesType, unitData.subSpeciesType);
    }
    public void Init(TroopEntity entityData)
    {
        Init(entityData, entityData.speciesType, entityData.subSpeciesType);
    }

    /// <summary>
    /// 基础的底层适配
    /// </summary>
    /// <param name="unitData"></param>
    /// <param name="speciesType"></param>
    public void Init(TroopEntity entityData, string speciesType, string subSpecies)
    {
        //var modelName = DataBaseManager.Instance.GetModelName(entityData.armourEquipSet.data.idName, entityData.weaponEquipSet.data.idName, speciesType);
        //var model = DataBaseManager.Instance.GetTargetAggregationData<EquipAbleModelData>(modelName);
        //var spineData = DataBaseManager.Instance.SkeletonDataLoad(model.spineResName, model.GetStringValue(Constant_AttributeString.DATA_DIR));
        //spineData.name = model.spineResName;
        //GraphicInit(spineData, entityData.armourEquipSet.data.idName, entityData.weaponEquipSet.data.idName, speciesType, subSpecies);
        if (graphic == null)
        {
            graphic = targetGraphicObject.AddComponent<SkeletonGraphic>();
        }
        else
        {
            graphic.Initialize(true);
        }
        SpineAtlasManager.Instance.SetSkeletonGraphicToUnit(graphic, entityData);
        if (numberText != null) numberText.text = entityData.originData.soldierNum.ToString();
        if (nameText != null) nameText.text = entityData.originData.name.ToString();
        MotionFit(entityData);
    }

    /// <summary>
    /// 基础的底层适配
    /// </summary>
    /// <param name="unitData"></param>
    /// <param name="speciesType"></param>
    public void Init(UnitData unitData, string speciesType, string subSpecies)
    {
        var modelName = DataBaseManager.Instance.GetModelName(unitData.armourEquipSetId, unitData.weaponEquipSetId, speciesType);
        var model = DataBaseManager.Instance.GetTargetAggregationData<EquipAbleModelData>(modelName);
        var spineData = DataBaseManager.Instance.SkeletonDataLoad(model.spineResName, model.GetStringValue(Constant_AttributeString.DATA_DIR));
        spineData.name = model.spineResName;
        GraphicInit(spineData, unitData.armourEquipSetId, unitData.weaponEquipSetId, speciesType, subSpecies);
        if (numberText != null) numberText.text = unitData.soldierNum.ToString();
        if (nameText != null) nameText.text = unitData.name.ToString();
        MotionFit(unitData);
    }

    void MotionFit(UnitData unitData)
    {
        var model = DataBaseManager.Instance.GetIdNameDataFromList<EquipAbleModelData>(DataBaseManager.Instance.GetModelName(unitData));
        var actionMotion = unitData.weaponEquipSetId != null ? DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>(unitData.weaponEquipSetId).TargetActionModel : "MELEE";
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
        graphic.timeScale = 0f;
    }

    void MotionFit(TroopEntity entityData)
    {
        var model = DataBaseManager.Instance.GetIdNameDataFromList<EquipAbleModelData>(DataBaseManager.Instance.GetModelName(entityData.originData));
        var actionMotion = entityData.weaponEquipSet.data.idName != null ? DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>(entityData.weaponEquipSet.data.idName).TargetActionModel : "MELEE";
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
        graphic.timeScale = 0f;
    }

    public void GraphicInit(SkeletonDataAsset basicData, string armourEquipSetId, string weaponEquipSetId, string speciesType, string subSpeciesType)
    {
        if (graphic == null)
        {
            graphic = targetGraphicObject.AddComponent<SkeletonGraphic>();
        }
        else
        {
            graphic.Initialize(true);
        }
        graphic.skeletonDataAsset = basicData;
        graphic.Initialize(true);
        graphic.Rebuild(UnityEngine.UI.CanvasUpdate.PreRender);
        SpineAtlasManager.Instance.ApplySpinData(graphic, new string[] { armourEquipSetId, weaponEquipSetId }, DataBaseManager.Instance.GetModelName(armourEquipSetId, weaponEquipSetId, speciesType), speciesType, subSpeciesType, equipBelong);
        graphic.Update();
    }

    public virtual void SetNumber(int max, int unitNumber)
    {
        float unitPercent = (float)unitNumber / (float)max;
        numberFill.fillAmount = unitPercent;
        if (numberText != null) numberText.text = unitNumber.ToString();
    }

    public void RefreshNumber(CommandUnit commandUnit)
    {
        SetNumber(commandUnit.troopsData.Count, commandUnit.aliveCount);
    }

    /// <summary>
    /// 触发点击
    /// </summary>
    public virtual void OnClick()
    {
        UIManager.Instance.ShowUI("UnitDetailPanel", (ui) =>
        {
            (ui as UnitDetailPanel).InitByTroop(troopData);
        });
    }

}
