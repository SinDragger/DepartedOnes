using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeciesPanel : UIPanel
{
    public override string uiPanelName => "SpeciesPanel";
    public GameObject subPanel;
    public UnitAttributeArea unitAttributeArea;
    /// <summary>
    /// 名称
    /// </summary>
    public Text unitNameText;
    public SkeletonGraphic graphic;
    List<object> speciesDataList = new List<object>();
    List<SpeciesSlot> speciesSlotList = new List<SpeciesSlot>();
    public GameObject speciesListPrefab;
    public Transform content;
    public SpecialityArea specialityArea;
    public void Init(Force force)
    {
        speciesDataList.Clear();
        for (int i = 0; i < force.speciesList.Count; i++)
        {
            speciesDataList.Add(force.speciesList[i]);
            var te = force.speciesList[i].idName;
            foreach (var subSpecies in force.subSpeciesList.FindAll((sub) => sub.species == force.speciesList[i].idName))
            {
                speciesDataList.Add(subSpecies);
            }
        }
        ArrayUtil.ListShowFit(speciesSlotList, speciesDataList, speciesListPrefab, content, (slot, data) =>
        {
            slot.Init(data);
            slot.gameObject.SetActive(true);
            slot.SetOnClick(() =>
            {
                InitSubPanel(data);
            });
        });
        InitSubPanel(speciesDataList[0]);
        //俩表合并进行一个排序
    }

    void InitSubPanel(object species)
    {
        if (species is Species)
        {
            InitSubPanel((Species)species);
        }
        else if (species is SubSpecies)
        {
            InitSubPanel((SubSpecies)species);
        }
    }
    void InitSubPanel(Species species)
    {
        unitNameText.text = species.name;
        var unit = DataBaseManager.Instance.GetSpeciesBaseUnit(species.idName);
        unitAttributeArea.Init(unit);
        RefreshUnitModel(unit, species);
        List<StandardStatus> statusList = new List<StandardStatus>();
        if (species.statusAttachs != null)
        {
            for (int i = 0; i < species.statusAttachs.Length; i++)
            {
                statusList.Add(DataBaseManager.Instance.GetIdNameDataFromList<StandardStatus>(species.statusAttachs[i]));
            }
        }
        specialityArea.Init(statusList);
    }

    void InitSubPanel(SubSpecies subSpecies)
    {
        Species species = DataBaseManager.Instance.GetIdNameDataFromList<Species>(subSpecies.species);
        unitNameText.text = subSpecies.name;
        var unit = DataBaseManager.Instance.GetSpeciesBaseUnit(subSpecies.species, subSpecies.idName);
        unitAttributeArea.Init(unit);
        RefreshUnitModel(unit, species, subSpecies);
        List<StandardStatus> statusList = new List<StandardStatus>();
        if (species.statusAttachs != null)
        {
            for (int i = 0; i < species.statusAttachs.Length; i++)
            {
                statusList.Add(DataBaseManager.Instance.GetIdNameDataFromList<StandardStatus>(species.statusAttachs[i]));
            }
        }
        if (subSpecies.statusAttachs != null)
        {
            for (int i = 0; i < subSpecies.statusAttachs.Length; i++)
            {
                statusList.Add(DataBaseManager.Instance.GetIdNameDataFromList<StandardStatus>(subSpecies.statusAttachs[i]));
            }
        }
        specialityArea.Init(statusList);
    }

    void RefreshUnitModel(UnitData data, Species species, SubSpecies subSpecies = null)
    {
        SpineAtlasManager.Instance.SetSkeletonGraphicToUnit(graphic, data, species.idName, subSpecies == null ? null : subSpecies.idName);
        var model = DataBaseManager.Instance.GetIdNameDataFromList<EquipAbleModelData>(DataBaseManager.Instance.GetModelName(data));
        var actionMotion = data.weaponEquipSetId != null ? DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>(data.weaponEquipSetId).TargetActionModel : "MELEE";
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
