using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitOrganizationPanel : UIPanel
{
    public override string uiPanelName => "UnitOrganizationPanel";
    public EquipSetPanel unitWeaponSlot;
    public EquipSetPanel unitArmourSlot;
    public Text unitName;
    public InputField unitNameInput;
    public UnitAttributeArea unitAttributeArea;
    public Button createNewBtn;
    public Button comfirmNewBtn;

    public GameObject weaponAreaRemind;
    public GameObject armourAreaRemind;

    public GameObject equipPanel;
    public GameObject unitPanel;
    public Transform unitPanelContent;
    public GameObject unitPanelSlotPrefab;
    List<UnitData> unitDataList;
    List<UnitPanelSlot> unitPanelSlotList = new List<UnitPanelSlot>();

    public Transform equipPanelContent;
    public GameObject equipPanelSlotPrefab;
    List<EquipSetData> equipDataList = new List<EquipSetData>();
    List<EquipSetPanel> equipPanelSlotList = new List<EquipSetPanel>();


    public Transform statusContent;
    public GameObject statusSlotPrefab;
    List<EquipSetData> statusContentList = new List<EquipSetData>();

    public SkeletonGraphic graphic;
    bool isCreatingNew;
    UnitData nowUnitData;
    Force nowforce;

    protected override void Awake()
    {
        base.Awake();
        createNewBtn.SetBtnEvent(OnCreateNewClick);
        comfirmNewBtn.SetBtnEvent(OnComfirmNewClick);
        //临时
        equipPanel.SetActive(false);
        unitWeaponSlot.GetComponent<Button>().SetBtnEvent(OnWeaponSlotClick);
        unitArmourSlot.GetComponent<Button>().SetBtnEvent(OnArmourSlotClick);
    }


    public void Init(Force force)
    {
        nowforce = force;
        unitDataList = force.unitList;
        ResetArea();
        ArrayUtil.ListShowFit(unitPanelSlotList, unitDataList, unitPanelSlotPrefab, unitPanelContent, (slot, data) =>
        {
            slot.Init(data);
            slot.gameObject.SetActive(true);
            slot.SetOnClick(() =>
            {
                if (isCreatingNew) return;
                OnSlotSelect(slot);
                nowUnitData = data;
                ShowUnitInfo();
            });
        });
        OnSlotSelect(unitPanelSlotList[0]);
        nowUnitData = unitDataList[0];
        ShowUnitInfo();
        //俩表合并进行一个排序
    }

    UnitPanelSlot selectSlot;
    public void OnSlotSelect(UnitPanelSlot slot)
    {
        if (isCreatingNew) return;
        if (selectSlot != null && selectSlot != slot)
        {
            selectSlot.OnSelected(false);
        }
        if (slot != null)
        {
            selectSlot = slot;
            slot.OnSelected(true);
        }
    }

    void ResetArea()
    {
        createNewBtn.gameObject.SetActive(true);
        comfirmNewBtn.gameObject.SetActive(false);
        weaponAreaRemind.gameObject.SetActive(false);
        armourAreaRemind.gameObject.SetActive(false);
        unitNameInput.gameObject.SetActive(false);
        unitName.gameObject.SetActive(true);
    }

    void OnCreateNewClick()
    {
        if (nowUnitData == null)
        {
            nowUnitData = unitDataList[0];
        }
        nowUnitData = nowUnitData.Clone();
        ShowUnitInfo();
        equipPanel.SetActive(true);
        isCreatingNew = true;
        createNewBtn.gameObject.SetActive(false);
        comfirmNewBtn.gameObject.SetActive(true);
        weaponAreaRemind.gameObject.SetActive(true);
        armourAreaRemind.gameObject.SetActive(true);
        OnWeaponSlotClick();
        unitNameInput.SetTextWithoutNotify(unitName.text);
        unitNameInput.gameObject.SetActive(true);
        unitName.gameObject.SetActive(false);
    }

    void OnComfirmNewClick()
    {
        nowUnitData.name = unitNameInput.text;
        unitName.text = unitNameInput.text;
        nowUnitData.FitWeaponUnitType();
        nowUnitData.idName = DataBaseManager.Instance.GetSelfCreateIdName();
        unitDataList.Add(nowUnitData);
        DataBaseManager.Instance.GetTargetDataList<UnitData>().Add(nowUnitData);
        ResetArea();
        ArrayUtil.ListShowFit(unitPanelSlotList, unitDataList, unitPanelSlotPrefab, unitPanelContent, (slot, data) =>
        {
            slot.Init(data);
            slot.gameObject.SetActive(true);
            slot.SetOnClick(() =>
            {
                if (isCreatingNew) return;
                OnSlotSelect(slot);
                nowUnitData = data;
                ShowUnitInfo();
            });
        });
        isCreatingNew = false;
        OnSlotSelect(unitPanelSlotList[unitPanelSlotList.Count - 1]);
    }

    public void ShowUnitInfo()
    {
        unitWeaponSlot.Init(nowUnitData.weaponEquipSetId);
        unitArmourSlot.Init(nowUnitData.armourEquipSetId);
        RefreshUnitModel(nowUnitData, nowUnitData.speciesType, nowUnitData.subSpeciesType);
        unitAttributeArea.Init(nowUnitData);
        unitName.text = nowUnitData.name;
    }

    void RefreshUnitModel(UnitData data, string species, string subSpecies)
    {
        RefreshUnitModel(data, DataBaseManager.Instance.GetIdNameDataFromList<Species>(species), DataBaseManager.Instance.GetIdNameDataFromList<SubSpecies>(subSpecies));
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

    void OnWeaponSlotClick()
    {
        weaponAreaRemind.GetComponent<Image>().color = Color.Lerp(Color.clear, Color.black, 0.4f);
        armourAreaRemind.GetComponent<Image>().color = Color.Lerp(Color.clear, Color.black, 0.2f);
        equipDataList = nowforce.weaponSetList;
        ArrayUtil.ListShowFit(equipPanelSlotList, equipDataList, equipPanelSlotPrefab, equipPanelContent, (slot, data) =>
        {
            slot.Init(data);
            slot.gameObject.SetActive(true);
            slot.GetComponent<Button>().SetBtnEvent(() => ChangeUnitWeaponSet(data));
        });
    }

    void ChangeUnitWeaponSet(EquipSetData equipSetData)
    {
        nowUnitData.weaponEquipSetId = equipSetData.idName;
        ShowUnitInfo();
    }

    void OnArmourSlotClick()
    {
        weaponAreaRemind.GetComponent<Image>().color = Color.Lerp(Color.clear, Color.black, 0.2f);
        armourAreaRemind.GetComponent<Image>().color = Color.Lerp(Color.clear, Color.black, 0.4f);
        equipDataList = nowforce.armourSetList;
        ArrayUtil.ListShowFit(equipPanelSlotList, equipDataList, equipPanelSlotPrefab, equipPanelContent, (slot, data) =>
        {
            slot.Init(data);
            slot.gameObject.SetActive(true);
            slot.GetComponent<Button>().SetBtnEvent(() => ChangeUnitArmourSet(data));
        });
    }

    void ChangeUnitArmourSet(EquipSetData equipSetData)
    {
        nowUnitData.armourEquipSetId = equipSetData.idName;
        ShowUnitInfo();
    }
}
