using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestCode : MonoSingleton<TestCode>
{
    EventProcessModel model = new EventProcessModel();
    /// <summary>
    /// 基础模型样式
    /// </summary>
    public GameObject originPrefab;
    /// <summary>
    /// 生成的模型
    /// </summary>
    GameObject createdTarget;
    public Transform targetParent;


    public EquipAbleSlotPanel equipSlotPanel;
    public Button CreateModelButton;
    public InputField inputField;

    public Button LoadEquipButton;
    public Button changeActionButton;
    public InputField loadEquipInputField;

    public LegionTroopSlot showSlot;

    List<EquipAbleModelData> modelsData;
    List<EquipSetData> equipSetData;
    List<EquipData> equipData;
    List<UnitData> unitData;

    public void Init()
    {
        modelsData = DataBaseManager.Instance.GetTargetDataList<EquipAbleModelData>();
        equipSetData = DataBaseManager.Instance.GetTargetDataList<EquipSetData>();
        equipData = DataBaseManager.Instance.GetTargetDataList<EquipData>();
        unitData = DataBaseManager.Instance.GetTargetDataList<UnitData>();
        CreateModelButton.onClick.AddListener(OnCreateModelButtonClick);
        LoadEquipButton.onClick.AddListener(OnLoadEquipButtonClick);
        changeActionButton.onClick.AddListener(OnChangeModelAction);
        newEquipSetSaveButton.onClick.AddListener(OnNewEquipSaveButtonOnClick);

        InitEquipSetPanel();
    }

    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
        SpineAtlasManager.Instance.Init();
        Init();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            DataBaseManager.Instance.SaveXMLByType<EquipSetData>();
            //DataBaseManager.Instance.SaveTargetXML(typeof(EquipSetData).Name, modelsData.ToArray());
            //DataBaseManager.Instance.SaveTargetXML(typeof(EquipAbleModelData).Name, modelsData.ToArray());
        }
    }


    EquipAbleModelData modelData;
    EquipSetData equipSet;
    string targetUnitTypeName;
    public void OnCreateModelButtonClick()
    {
        //inputField.text = "TestNSoldier";
        targetUnitTypeName = inputField.text;
        //尝试获取阵型
        UnitData data = unitData.Find((t) => t.idName.Equals(targetUnitTypeName));
        if (data == null) return;

        if (targetParent.childCount > 0)
        {
            Destroy(targetParent.GetChild(0).gameObject);
        }
        var asset = originPrefab.GetComponentInChildren<Spine.Unity.SkeletonAnimation>().skeletonDataAsset;
        showSlot.Init(data, data.speciesType);
        //showSlot
        createdTarget = Instantiate(originPrefab, targetParent);
        SoldierStatus statusRef;
        var model = createdTarget.GetComponent<SoldierModel>();
        statusRef = new SoldierStatus();
        statusRef.EntityData = new TroopEntity(data);
        statusRef.model = model;
        model.Init(statusRef);
        model.enabled = false;
        GameObject target = model.GetComponent<SpineUnityControl>().meshRenderer.gameObject;
        EquipSwitchXML switchModel = model.EquipSwitch;
        switchModel.AddEquipSet(data.armourEquipSetId);
        switchModel.AddEquipSet(data.weaponEquipSetId);
        var modelIdName = DataBaseManager.Instance.GetModelName(data.armourEquipSetId, data.weaponEquipSetId, data.speciesType);
        switchModel.targetModelName = modelIdName;//使用模型装备文件
        switchModel.speciesType = data.speciesType;
        switchModel.subSpeciesType = data.subSpeciesType;
        modelData = modelsData.Find((m) => m.idName.Equals(modelIdName));
        equipSet = equipSetData.Find((m) => m.idName.Equals(data.armourEquipSetId));
        if (newSet != null)//清除临时装备数据
        {
            equipSetData.Remove(newSet);
            newSet = null;
        }
        //if (!string.IsNullOrEmpty(targetEquipSetName))
        //{
        //    EquipSetData localEquipSet = equipSetData.Find((m) => m.idName.Equals(targetEquipSetName));
        //    if (localEquipSet != null)
        //    {
        //        equipSet = localEquipSet;
        //        switchModel.ClearEquipSet();
        //        switchModel.AddEquipSet(targetEquipSetName);
        //    }
        //}
        switchModel.OnInit();
        createdTarget.GetComponentInChildren<ClickToMove>().enabled = false;
        createdTarget.GetComponentInChildren<ForceFaceCamera>().enabled = false;

        createdTarget.GetComponentInChildren<MeshRenderer>().transform.localScale = Vector3.one;
        createdTarget.GetComponentInChildren<MeshRenderer>().transform.rotation = Quaternion.identity;


        equipSlotPanel.Init(modelData, equipSet);
    }
    public const string TEMP_EQUIP_SET_NAME = "~Temp";
    EquipSetData newSet;
    public void OnLoadEquipButtonClick()
    {
        string targetEquipName = loadEquipInputField.text;
        LoadTargetEquip(targetEquipName);
    }

    int nowAction;
    public void OnChangeModelAction()
    {
        if (createdTarget == null) return;
        nowAction++;
        if (nowAction == 6) nowAction = 0;
        AnimState target = AnimState.DEFAULT;
        switch (nowAction)
        {
            case 0: target = AnimState.ATTACK; break;
            case 1: target = AnimState.DIE; break;
            case 2: target = AnimState.IDLE; break;
            case 3: target = AnimState.MOVE_LOOP; break;
                //case 5: target = AnimState.RUN_LOOP; break;
        }
        var action = createdTarget.GetComponentInChildren<SpineUnityControl>();
        if (nowAction == 4)
        {
            action.selfSkelAni.state.SetAnimation(0, "Attack_Shoot", true);
            return;
        }
        if (nowAction == 5)
        {
            action.selfSkelAni.state.SetAnimation(0, "Attack_Stab", true);
            return;
        }
        action.selfSkelAni.state.SetAnimation(0, action.GetAnimationName(target), true);
    }

    public InputField newEquipSetNameInput;
    public Button newEquipSetSaveButton;

    public void OnNewEquipSaveButtonOnClick()
    {
        newSet.idName = newEquipSetNameInput.text;
        DataBaseManager.Instance.SaveXMLByType<EquipSetData>();
    }

    public void OnNowEquipChangeSave()
    {
        if (newSet == null || string.IsNullOrEmpty(targetUnitTypeName)) return;
        int index = equipSetData.FindIndex((e) => e.idName.Equals(targetUnitTypeName));
        equipSetData[index].UseEquipTextures = newSet.UseEquipTextures;
        if (newSet != null)//清除临时装备数据
        {
            equipSetData.Remove(newSet);
            newSet = null;
        }
        DataBaseManager.Instance.SaveXMLByType<EquipSetData>();
    }


    public EquipAdjustEditorPanel adjustEditor;
    public void LoadTargetEquip(string targetEquipName)
    {
        SpineAtlasManager.Instance.RemoveTemp();
        var equip = equipData.Find((t) => t.idName.Equals(targetEquipName));
        if (equip == null && !string.IsNullOrEmpty(targetEquipName))
        {
            equip = new EquipData() { idName = targetEquipName, equipPositionName = EquipAbleSlotPanel.nowEditorEquipType, equipTexPath = $"Tex/{targetEquipName}" };
            equipData.Add(equip);
            DataBaseManager.Instance.SaveXMLByType<EquipData>("EquipData.Tex");
        }
        if (equip != null)
        {
            adjustEditor.Init(equip);
        }
        if (newSet == null)
        {
            newSet = equipSet.Clone();
            newSet.idName = TEMP_EQUIP_SET_NAME;
            equipSetData.Add(newSet);
        }
        newSet.ChangeEquip(EquipAbleSlotPanel.nowEditorEquipType, targetEquipName);
        //确定当前的EquipType类型
        //对目标资源进行导入加载
        //临时数据的刷新并用
        EquipSwitchXML switchModel = createdTarget.GetComponentInChildren<EquipSwitchXML>();
        switchModel.ClearEquipSet();
        switchModel.AddEquipSet(TEMP_EQUIP_SET_NAME);
        switchModel.Switch();

        equipSlotPanel.Refresh(newSet);
        //刷新
    }

    public void ModelUpdate()
    {
        SpineAtlasManager.Instance.RemoveTemp();
        EquipSwitchXML switchModel = createdTarget.GetComponentInChildren<EquipSwitchXML>();
        switchModel.Switch();
    }
    //增加调整逻辑

    public Transform equipSetContent;
    public GameObject equipSetOriginPrefab;

    public void InitEquipSetPanel()
    {
        equipSetOriginPrefab.gameObject.SetActive(false);
        for (int i = 0; i < unitData.Count; i++)
        {
            var g = Instantiate(equipSetOriginPrefab, equipSetContent);
            g.SetActive(true);
            var slot = g.GetComponent<EquipSlot>();

            slot.Init(unitData[i].idName, unitData[i].name.ToString());
            string targetName = unitData[i].idName;
            slot.clickCallback = () =>
            {
                inputField.text = targetName;
                targetUnitTypeName = targetName;
                OnCreateModelButtonClick();
            };
        }
    }
    //模型工作流建立——————导入-标记装备区域-对标记尝试挂载基础装备-修正基础装备参数
    //装备工作流建立——————导入图片-选择目标装备位-调整修正
    //套装工作流建立——————加载目标模型-加载装备区间-选择调整
}
