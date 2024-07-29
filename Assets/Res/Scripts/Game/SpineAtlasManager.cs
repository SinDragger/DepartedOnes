using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineAtlasManager : MonoSingleton_AutoCreate<SpineAtlasManager>
{
    //皮肤生产
    public SkeletonGraphic skinFactory;
    List<EquipAbleModelData> modelsData;
    List<EquipSetData> equipSetData;
    List<EquipData> equipData;
    List<Species> speciesData;
    List<SubSpecies> subSpeciesData;
    public MaterialPropertyBlock BasicBlock
    {
        get
        {
            if (basicBlock == null)
            {
                basicBlock = new MaterialPropertyBlock();
                basicBlock.SetColor("_Color", Color.white);
            }
            return basicBlock;
        }
    }
    MaterialPropertyBlock basicBlock;
    //TODO增加势力从属的上级字典：用于势力管理：一方势力退场则完全移除
    public Dictionary<string, Skin> skinRestore = new Dictionary<string, Skin>();
    public Sprite basicClear;
    [HideInInspector]
    public Stack<string> waitInitSkinList = new Stack<string>();

    int count;

    protected override void Init()
    {
        if (hasInit) return;
        hasInit = true;
        Init();
    }

    public void Init(Action callback = null)
    {
        modelsData = DataBaseManager.Instance.GetTargetDataList<EquipAbleModelData>();
        equipSetData = DataBaseManager.Instance.GetTargetDataList<EquipSetData>();
        equipData = DataBaseManager.Instance.GetTargetDataList<EquipData>();
        speciesData = DataBaseManager.Instance.GetTargetDataList<Species>();
        subSpeciesData = DataBaseManager.Instance.GetTargetDataList<SubSpecies>();
    }
    public void RemoveTemp()
    {
        foreach (var pair in skinRestore)
        {
            if (pair.Key.Contains(TestCode.TEMP_EQUIP_SET_NAME))
            {
                skinRestore.Remove(pair.Key);
                return;
            }
        }
    }

    /// <summary>
    /// 获取Id对应的装备列表
    /// </summary>
    public List<EquipData> GetUseEquips(string equipSetName)
    {
        EquipSetData equipSet = equipSetData.Find((m) => m.idName.Equals(equipSetName));
        List<EquipData> useEquips = new List<EquipData>();
        for (int i = 0; i < equipSet.UseEquipTextures.Length; i++)
        {
            useEquips.Add(equipData.Find((e) => e.idName.Equals(equipSet.UseEquipTextures[i])));
        }
        return useEquips;
    }
    public List<EquipData> GetUseEquips(string[] equipSetIds)
    {
        List<EquipData> useEquips = new List<EquipData>();
        for (int i = 0; i < equipSetIds.Length; i++)
        {
            if (!string.IsNullOrEmpty(equipSetIds[i]))
            {
                EquipSetData equipSet = equipSetData.Find((m) => m.idName.Equals(equipSetIds[i]));
                if (equipSet.UseEquipTextures != null)
                {
                    for (int j = 0; j < equipSet.UseEquipTextures.Length; j++)
                    {
                        var equipSetData = equipData.Find((e) => e.idName.Equals(equipSet.UseEquipTextures[j]));
                        if (equipSetData == null)
                        {
                            Debug.LogError(equipSet.UseEquipTextures[j]);
                        }
                        else
                        {
                            useEquips.Add(equipSetData);
                        }
                    }
                }
            }
        }
        return useEquips;
    }

    /// <summary>
    /// 注册目标Unit单位的ID
    /// </summary>
    public void RegisterSpineAbleUse(UnitData unitData, string speciesType, string subSpeciesType, int forceId)
    {
        if (subSpeciesType == null) subSpeciesType = "";
        string targetModelName = DataBaseManager.Instance.GetModelName(unitData.armourEquipSetId, unitData.weaponEquipSetId, speciesType);
        string key = $"{targetModelName}_{speciesType}_{unitData.armourEquipSetId}_{unitData.weaponEquipSetId}_{forceId}";
        if (skinRestore.ContainsKey(key)) return;
        waitInitSkinList.Push($"{unitData.armourEquipSetId}&{unitData.weaponEquipSetId}&{speciesType}&{subSpeciesType}&{forceId}");
    }

    public bool isInit;
    private void FixedUpdate()
    {
        if (isInit) return;
        if (waitInitSkinList.Count > 0)
        {
            string needLoadSkin = waitInitSkinList.Pop();
            var param = needLoadSkin.Split('&');
            PreLoadCustomSkin(param[0], param[1], param[2], param[3], int.Parse(param[4]));
        }
    }

    public void PreLoadCustomSkin(string armourEquipSetId, string weaponEquipSetId, string speciesType, string subSpeciesType, int forceId)
    {
        var modelName = DataBaseManager.Instance.GetModelName(armourEquipSetId, weaponEquipSetId, speciesType);
        var model = DataBaseManager.Instance.GetTargetAggregationData<EquipAbleModelData>(modelName);
        var data = DataBaseManager.Instance.SkeletonDataLoad(model.spineResName, model.GetStringValue(Constant_AttributeString.DATA_DIR));
        skinFactory.skeletonDataAsset = data;
        skinFactory.Initialize(true);
        skinFactory.Rebuild(UnityEngine.UI.CanvasUpdate.PreRender);
        ApplySpinData(skinFactory, new string[] { armourEquipSetId, weaponEquipSetId }, modelName, speciesType, subSpeciesType, forceId);
    }

    public void SetSkeletonGraphicToUnit(SkeletonGraphic skeletonGraphic, TroopControl troop)
    {
        SetSkeletonGraphicToUnit(skeletonGraphic, troop.troopEntity, troop.belong);
    }

    public void SetSkeletonGraphicToUnit(SkeletonGraphic skeletonGraphic, TroopEntity troop, int belong = 1)
    {
        var speciesType = troop.speciesType;
        string armourEquipSetId = troop.armourEquipSet.data.idName;
        string weaponEquipSetId = troop.weaponEquipSet.data.idName;
        var modelName = DataBaseManager.Instance.GetModelName(armourEquipSetId, weaponEquipSetId, speciesType);
        var model = DataBaseManager.Instance.GetTargetAggregationData<EquipAbleModelData>(modelName);
        SkeletonDataAsset basicData = DataBaseManager.Instance.SkeletonDataLoad(model.spineResName, model.GetStringValue(Constant_AttributeString.DATA_DIR));
        skeletonGraphic.skeletonDataAsset = basicData;
        skeletonGraphic.Initialize(true);
        skeletonGraphic.Rebuild(UnityEngine.UI.CanvasUpdate.PreRender);
        ApplySpinData(skeletonGraphic, new string[] { armourEquipSetId, weaponEquipSetId }, DataBaseManager.Instance.GetModelName(armourEquipSetId, weaponEquipSetId, speciesType), speciesType, troop.subSpeciesType, belong);
        skeletonGraphic.Update();
    }

    public void SetSkeletonGraphicToUnit(SkeletonGraphic skeletonGraphic, UnitData unitData, string speciesType, string subSpecies)
    {
        var modelName = DataBaseManager.Instance.GetModelName(unitData.armourEquipSetId, unitData.weaponEquipSetId, speciesType);
        var model = DataBaseManager.Instance.GetTargetAggregationData<EquipAbleModelData>(modelName);
        SkeletonDataAsset basicData = DataBaseManager.Instance.SkeletonDataLoad(model.spineResName, model.GetStringValue(Constant_AttributeString.DATA_DIR));
        string armourEquipSetId = unitData.armourEquipSetId;
        string weaponEquipSetId = unitData.weaponEquipSetId;
        skeletonGraphic.skeletonDataAsset = basicData;
        skeletonGraphic.Initialize(true);
        skeletonGraphic.Rebuild(UnityEngine.UI.CanvasUpdate.PreRender);
        ApplySpinData(skeletonGraphic, new string[] { armourEquipSetId, weaponEquipSetId }, DataBaseManager.Instance.GetModelName(armourEquipSetId, weaponEquipSetId, speciesType), speciesType, subSpecies, 0);
        skeletonGraphic.Update();
    }

    /// <summary>
    /// 不嵌套种族皮肤的方式
    /// </summary>
    public Skin GetCustomSkin(string skinKey, Skeleton skeleton, SkeletonDataAsset dataAsset, List<EquipData> useEquips, EquipAbleModelData model, int forceId = 0)
    {
        Skin customSkin = null;
        if (skinRestore.ContainsKey(skinKey))
        {
            customSkin = skinRestore[skinKey];
        }
        else
        {
            customSkin = new Skin(skinKey);
            Skin templateSkin = skeleton.Data.DefaultSkin;
            if (templateSkin != null)
            {
                customSkin.AddSkin(templateSkin);
            }
            Material sourceMaterial = dataAsset.atlasAssets[0].PrimaryMaterial;
            ApplySkinMaterial(customSkin, sourceMaterial, model, skeleton, useEquips);//装备附加

            //合并贴图
            Skin repackedSkin = customSkin.GetRepackedSkin($"repacked skin{count}", sourceMaterial, out Material runtimeMaterial, out Texture2D runtimeAtlas); // Pack all the items in the skin.
            customSkin.Clear();
            customSkin = repackedSkin;
            skinRestore.Add(skinKey, customSkin);
            count++;
        }
        return customSkin;
    }

    void ApplySkinMaterial(Skin customSkin, Material sourceMaterial, EquipAbleModelData model, Skeleton skeleton, List<EquipData> useEquips)
    {

        float scale = 0.01f;
        if (model.scale != default)
        {
            scale *= model.scale;
        }
        float extraRotation = 0f;
        if (model.DataArray == null) return;
        for (int i = 0; i < model.DataArray.Length; i++)
        {
            ModelSuitData data = model.DataArray[i];
            Sprite sprite = basicClear;
            float deltaX = 0f;
            float deltaY = 0f;
            float deltaRot = 0f;
            string slotName = data.slotName;
            EquipData equipData = useEquips.Find((e) => e.equipPositionName.Equals(data.equipTypeName));//TODO:改成复数？
            if (Enum.TryParse<StandardEquipType>(data.equipTypeName, out StandardEquipType result))
            {
                if (result >= StandardEquipType.NONE_BASIC)
                {
                    if (equipData == null)
                    {
                        continue;
                    }
                }
            }
            else
            {
                if (equipData == null)
                {
                    continue;
                }
            }
            if (equipData != null)//存在可用的
            {
                deltaX += equipData.posDelta.x;
                deltaY += equipData.posDelta.y;
                deltaRot += equipData.rotDelta;
                Texture2D originTex = DataBaseManager.Instance.GetTexByIdName(equipData.equipTexPath);
                sprite = PictureUtil.CopyTexture(originTex);
                sprite.name = equipData.equipTexPath;
            }
            else
            {
                sprite = PictureUtil.CopyTexture(basicClear.texture);
                sprite.name = slotName;
            }
            Slot slot = skeleton.FindSlot(slotName);
            if (slot != null)
            {
                Attachment originalAttachment = slot.Attachment;
                AtlasRegion region = sprite.ToAtlasRegionPMAClone(sourceMaterial);
                region.offsetX += data.posDelta.x + deltaX;
                region.offsetY += data.posDelta.y + deltaY;
                Attachment changeAttachment = region.ToRegionAttachment(region.name, scale, extraRotation + deltaRot + data.rotationDelta);
                customSkin.SetAttachment(slot.Data.Index, originalAttachment != null ? originalAttachment.Name : slotName, changeAttachment);
                slot.Attachment = changeAttachment;
            }
        }
    }

    //在新增战场兵种时进行当场生成
    public void ApplySpinData(SkeletonRenderer skeletonRenderer, string[] equipSetIds, string targetModelName, string speciesType, string subSpeciesType, int forceId = 0)
    {
        ApplySpinData(skeletonRenderer.skeleton, skeletonRenderer.skeletonDataAsset, equipSetIds, targetModelName, speciesType, subSpeciesType, forceId);
    }

    public void ApplySpinData(SkeletonGraphic skeletonGraphic, string[] equipSetIds, string targetModelName, string speciesType, string subSpeciesType, int forceId = 0)
    {
        ApplySpinData(skeletonGraphic.Skeleton, skeletonGraphic.skeletonDataAsset, equipSetIds, targetModelName, speciesType, subSpeciesType, forceId);
    }
    void ApplySpinData(Skeleton skeleton, SkeletonDataAsset dataAsset, string[] equipSetIds, string targetModelName, string speciesType, string subSpeciesType, int forceId = 0)
    {
        EquipAbleModelData model = modelsData.Find((m) => m.idName.Equals(targetModelName));
       

        if (model == null) return;
        List<EquipData> useEquips = GetUseEquips(equipSetIds);
        //通过UseEquips索引武器对应的位置并构建attachId数组
        string equipKeyId = equipSetIds[0];
        for (int i = 1; i < equipSetIds.Length; i++)
        {
            if (string.IsNullOrEmpty(equipSetIds[i])) continue;
            equipKeyId += $"_{equipSetIds[i]}";
        }

        string key = $"{targetModelName}_{speciesType}_{subSpeciesType}_{equipKeyId}_{forceId}";
        List<EquipData> speciesSkinEquip = null;
        if (IsNeedReplace(speciesType, out string replaceTargetSet))
        {
            if (speciesSkinEquip == null)
                speciesSkinEquip = new List<EquipData>();
            //索引种族的换皮装备位
            EquipSetData speciesSet = equipSetData.Find((m) => m.idName.Equals(replaceTargetSet));
            for (int i = 0; i < speciesSet.UseEquipTextures.Length; i++)
            {
                speciesSkinEquip.Add(equipData.Find((e) => e.idName.Equals(speciesSet.UseEquipTextures[i])));
            }
        }
        if (!string.IsNullOrEmpty(subSpeciesType))
        {
            SubSpecies subSpecies = subSpeciesData.Find((s) => s.idName.Equals(subSpeciesType));
            if (subSpecies.replaceTextures != null && subSpecies.replaceTextures.Length > 0)
            {
                if (speciesSkinEquip == null)
                    speciesSkinEquip = new List<EquipData>();
                for (int i = 0; i < subSpecies.replaceTextures.Length; i++)
                {
                    var subEquipData = equipData.Find((e) => e.idName.Equals(subSpecies.replaceTextures[i]));
                    if (subEquipData != null)
                    {
                        speciesSkinEquip.RemoveAll((e) =>
                        {
                            return e.equipPositionName.Equals(subEquipData.equipPositionName);
                        });
                    }
                    speciesSkinEquip.Add(subEquipData);
                }
            }
        }
        if (speciesSkinEquip != null && speciesSkinEquip.Count > 0)
        {
            useEquips.RemoveAll((e) =>
            {
                if (speciesSkinEquip.Find((t) => t.equipPositionName.Equals(e.equipPositionName)) != null)
                {
                    return true;
                }
                return false;
            });
            useEquips.AddRange(speciesSkinEquip);
        }
        Skin customSkin = GetCustomSkin(key, skeleton, dataAsset, useEquips, model, forceId);
        skeleton.SetSkin(customSkin);
        skeleton.SetSlotsToSetupPose();
    }

    public bool IsNeedReplace(string speciesType, out string replaceTargetSet)
    {
        replaceTargetSet = "";
        if (string.IsNullOrEmpty(speciesType)) return false;
        Species species = speciesData.Find((s) => s.idName.Equals(speciesType));
        if (species != null && !string.IsNullOrEmpty(species.additionModelSet))
        {
            replaceTargetSet = species.additionModelSet;
            return true;
        }
        return false;
    }

    public Color GetTargetForceColor(int forceId)
    {
        return Color.white;
    }
}
