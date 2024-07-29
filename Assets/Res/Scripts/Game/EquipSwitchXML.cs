using UnityEngine;
using System.Collections.Generic;
using Spine;
using Spine.Unity.AttachmentTools;
using Spine.Unity;

public class EquipSwitchXML : MonoBehaviour
{
    [HideInInspector]
    public string targetModelName;
    [HideInInspector]
    List<string> equipSetList = new List<string>();
    [HideInInspector]
    public int equipBelong;
    /// <summary>
    /// 生物类型
    /// </summary>
    [HideInInspector]
    public string speciesType;
    public string subSpeciesType;
    public string defaultMotion;
    public bool auto = false;
    private void OnEnable()
    {
        if (!auto) return;
        var skeletonRenderer = GetComponent<SkeletonRenderer>();
        skeletonRenderer.OnRebuild += SwitchSlotAttach;
        if (skeletonRenderer.valid) SwitchSlotAttach(skeletonRenderer);
    }

    public void AddEquipSet(string equipSetId)
    {
        if (!string.IsNullOrEmpty(equipSetId))
            equipSetList.Add(equipSetId);
    }

    public void ClearEquipSet()
    {
        equipSetList.Clear();
    }
    string lastSpineResName;
    public void OnInit()
    {
        var skeletonRenderer = GetComponent<SkeletonRenderer>();
        var model = DataBaseManager.Instance.GetTargetAggregationData<EquipAbleModelData>(targetModelName);
        if (skeletonRenderer.skeletonDataAsset == null || lastSpineResName != model.spineResName)
        {
            skeletonRenderer.skeletonDataAsset = DataBaseManager.Instance.SkeletonDataLoad(model.spineResName, model.GetStringValue(Constant_AttributeString.DATA_DIR));
            lastSpineResName = model.spineResName;
        }
        skeletonRenderer.Initialize(true);
        skeletonRenderer.OnRebuild += SwitchSlotAttach;
        if (skeletonRenderer.valid) SwitchSlotAttach(skeletonRenderer);

    }

    private void SwitchSlotAttach(SkeletonRenderer skeletonRenderer)
    {
        Debug.Log(targetModelName);

        CoroutineManager.StartWaitUntil(() => SpineAtlasManager.Instance != null, () =>
        {
            SpineAtlasManager.Instance.ApplySpinData(skeletonRenderer, equipSetList.ToArray(), targetModelName, speciesType, subSpeciesType, equipBelong);
        });
    }
    public void Switch()
    {
        var skeletonRenderer = GetComponent<SkeletonRenderer>();
        skeletonRenderer.skeleton.SetSkin(skeletonRenderer.skeleton.Data.DefaultSkin);
        SwitchSlotAttach(skeletonRenderer);
    }
}
