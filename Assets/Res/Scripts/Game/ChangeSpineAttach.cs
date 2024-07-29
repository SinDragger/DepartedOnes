using Spine;
using Spine.Unity;
using System.Collections;
using UnityEngine;
using Spine.Unity.AttachmentTools;

public class ChangeSpineAttach : MonoBehaviour
{
    public ModelSlotAdjust adjustRef;
    [SpineSkin]
    public string curSkin;
    private SkeletonRenderer skeletonRenderer;
    private Skeleton skeleton;
    public Sprite sprite;
    public Texture2D originTex;
    public Sprite alphaSprite;
    public Color targetColor;
    [SpineSlot]
    public string slotName;


    //使用原有attachment的uv等数据
    public bool newUVAttach;
    //uv uv2 xy偏移
    public Vector2 uv;
    public Vector2 uv2;
    public Vector2 posxy;
    //大小角度
    public float scale;
    public float extraRotation;

    //合并material
    public bool repack;

    [Header("Do not assign")]
    public Texture2D runtimeAtlas;
    public Material runtimeMaterial;
    Skin customSkin;
    private Skin collectedSkin;

    //需要兵种装束表
    private void OnEnable()
    {
        skeletonRenderer = GetComponent<SkeletonRenderer>();
        skeletonRenderer.OnRebuild += SwitchSlotAttach;//重载时切换装束
        if (skeletonRenderer.valid) SwitchSlotAttach(skeletonRenderer);
    }

    void SwitchSlotAttach(SkeletonRenderer skeletonRenderer)
    {
        skeleton = skeletonRenderer.skeleton;
        customSkin = customSkin ?? new Skin("Collected skin");

        var templateSkin = skeleton.Data.FindSkin(curSkin);
        if (templateSkin != null)
            customSkin.AddSkin(templateSkin);
        skeleton.Skin = customSkin;
        //ClearEquip();
        ChangeSpine();
    }

    public void ClearEquip()
    {
        PictureUtil.ShowTexture2DAlpha(sprite.texture);
        Material sourceMaterial = skeletonRenderer.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial;
        if (adjustRef != null)
        {
            for (int i = 0; i < adjustRef.dataArray.Length; i++)
            {
                Slot slot = skeleton.FindSlot(adjustRef.dataArray[i].slotName);
                if (slot != null)
                {
                    Attachment originalAttachment = slot.Attachment;
                    AtlasRegion region = sprite.ToAtlasRegionPMAClone(sourceMaterial);
                    region.u += uv.x;
                    region.v += uv.y;
                    region.u2 += uv2.x;
                    region.v2 += uv2.y;
                    region.offsetX += posxy.x;
                    region.offsetY += posxy.y;
                    Attachment changeAttachment = null;

                    //skin下可能没有attachment
                    if (originalAttachment != null && !newUVAttach)
                    {
                        changeAttachment = originalAttachment.GetRemappedClone(region, true, true, scale);
                    }
                    else
                    {
                        changeAttachment = region.ToRegionAttachment(region.name, scale, extraRotation);
                    }
                    customSkin.SetAttachment(slot.Data.Index, originalAttachment != null ? originalAttachment.Name : slotName, changeAttachment);
                    slot.Attachment = changeAttachment;
                }
            }
        }

        //运行合并贴图，降低drawcall，耗时长
        collectedSkin = collectedSkin ?? new Skin("Collected skin");
        collectedSkin.Clear();
        //使用的是官方goblin，customSkin和  DefaultSkin中attachment取并集
        collectedSkin.AddSkin(skeleton.Data.DefaultSkin);//skeleton.Data.FindSkin(curSkin)); // Include the "default" skin. (everything outside of skin placeholders)
        collectedSkin.AddSkin(customSkin); // Include your new custom skin.
                                                  // Note: materials and textures returned by GetRepackedSkin() behave like 'new Texture2D()' and need to be destroyed
        if (runtimeMaterial)
            Destroy(runtimeMaterial);
        if (runtimeAtlas)
            Destroy(runtimeAtlas);
        //合并贴图
        Skin repackedSkin = collectedSkin.GetRepackedSkin("repacked skin", sourceMaterial, out runtimeMaterial, out runtimeAtlas); // Pack all the items in the skin.
        collectedSkin.Clear();
        skeleton.SetSkin(repackedSkin);
        skeleton.SetSlotsToSetupPose(); // Use the pose from setup pose.
    }
    public void ChangeSpine()
    {
        Sprite sprite = PictureUtil.CopyTexture(originTex);
        sprite.name = originTex.name;
        PictureUtil.ShowTexture2DAlpha(sprite.texture);
        if (alphaSprite != null)
        {
            PictureUtil.ChangeTextureColor(sprite.texture, alphaSprite.texture, targetColor);
        }
        Slot slot = skeleton.FindSlot(slotName);
        if (slot != null)
        {
            ModelSuitData adjust = null;
            Attachment originalAttachment = slot.Attachment;
            if (!isFirstComplete)
            {
                //AtlasRegion originRegion = slot.Attachment.GetRegion();
                if (adjustRef != null)
                {
                    adjust = adjustRef.GetSuitData(slotName);
                    if (adjust == null)
                    {
                        adjustRef.SetSuitData(slotName, new Vector2(0f, 0f), (originalAttachment as RegionAttachment).Rotation);
                        //TODO:暂时无法解决spine的偏移问题，暂时为手动赋值
                    }
                }
            }
            if (adjustRef != null)
            {
                adjust = adjustRef.GetSuitData(slotName);
            }
            Material sourceMaterial = skeletonRenderer.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial;


            AtlasRegion region = sprite.ToAtlasRegionPMAClone(sourceMaterial);
            region.u += uv.x;
            region.v += uv.y;
            region.u2 += uv2.x;
            region.v2 += uv2.y;
            region.offsetX += posxy.x;
            region.offsetY += posxy.y;
            if (adjust != null)
            {
                region.offsetX += adjust.posDelta.x;
                region.offsetY += adjust.posDelta.y;
            }
            Attachment changeAttachment = null;

            //skin下可能没有attachment
            if (originalAttachment != null && !newUVAttach)
            {
                changeAttachment = originalAttachment.GetRemappedClone(region, true, true, scale);
            }
            else
            {
                changeAttachment = region.ToRegionAttachment(region.name, scale, extraRotation + (adjust == null ? 0 : adjust.rotationDelta));
            }
            customSkin.SetAttachment(slot.Data.Index, originalAttachment != null ? originalAttachment.Name : slotName, changeAttachment);
            slot.Attachment = changeAttachment;

            if (repack)
            {
                //运行合并贴图，降低drawcall，耗时长
                collectedSkin = collectedSkin ?? new Skin("Collected skin");
                collectedSkin.Clear();
                //使用的是官方goblin，customSkin和  DefaultSkin中attachment取并集
                collectedSkin.AddSkin(skeleton.Data.DefaultSkin);//skeleton.Data.FindSkin(curSkin)); // Include the "default" skin. (everything outside of skin placeholders)
                collectedSkin.AddSkin(customSkin); // Include your new custom skin.
                                                          // Note: materials and textures returned by GetRepackedSkin() behave like 'new Texture2D()' and need to be destroyed
                if (runtimeMaterial)
                    Destroy(runtimeMaterial);
                if (runtimeAtlas)
                    Destroy(runtimeAtlas);
                //合并贴图
                Skin repackedSkin = collectedSkin.GetRepackedSkin("repacked skin", sourceMaterial, out runtimeMaterial, out runtimeAtlas); // Pack all the items in the skin.
                collectedSkin.Clear();
                skeleton.SetSkin(repackedSkin);
            }
            else
            {
                skeleton.SetSkin(customSkin); // Just use the custom skin directly.
            }

            skeleton.SetSlotsToSetupPose(); // Use the pose from setup pose.
#if UNITY_EDITOR
            lastPosxy = posxy;
            lastRotation = extraRotation;
            isFirstComplete = true;
            isChanging = false;
            lastColor = targetColor;
#endif
        }
    }
    bool isFirstComplete = false;
#if UNITY_EDITOR

    bool isChanging = false;
    Vector2 lastPosxy;
    float lastRotation;
    Color lastColor;
    public bool resetBasic;
    private void OnValidate()
    {
        if (isFirstComplete && !isChanging)
        {
            bool needRefresh = false;
            if (posxy != lastPosxy)
            {
                needRefresh = true;
            }
            if (extraRotation != lastRotation)
            {
                needRefresh = true;
            }
            if (lastColor != targetColor)
            {
                needRefresh = true;
            }
            if (needRefresh)
            {
                //changeSpine();
                customSkin = null;
                collectedSkin = null;
                if (runtimeMaterial)
                    DestroyImmediate(runtimeMaterial);
                if (runtimeAtlas)
                    DestroyImmediate(runtimeAtlas);
                runtimeAtlas = null;
                runtimeMaterial = null;
                isChanging = true;
                SwitchSlotAttach(skeletonRenderer);
                SwitchSlotAttach(skeletonRenderer);
            }
        }
    }

    private void OnDisable()
    {
        //如果在调整的没有，则进行ScriptableObject的调整
        if (adjustRef != null)
        {
            var adjust = adjustRef.GetSuitData(slotName);
            if (adjust == null)
            {
                adjustRef.SetSuitData(slotName, posxy, extraRotation);
            }
            else if (resetBasic)
            {
                adjustRef.AddSuitData(slotName, posxy, extraRotation);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Refresh();
        }
    }
    void Refresh()
    {
        customSkin = null;
        collectedSkin = null;
        if (runtimeMaterial)
            DestroyImmediate(runtimeMaterial);
        if (runtimeAtlas)
            DestroyImmediate(runtimeAtlas);
        runtimeAtlas = null;
        runtimeMaterial = null;
        SwitchSlotAttach(skeletonRenderer);
    }
#endif
}

