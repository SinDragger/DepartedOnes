using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierPack : MonoBehaviour
{
    public SkeletonGraphic[] soldiers;

    public void SetColor(Color color)
    {
        foreach(var graph in soldiers)
        {
            graph.color = color;
        }
    }

    public void Init(TroopEntity data)
    {
        foreach(var soldier in soldiers)
        {
            RefreshUnitModel(soldier, data);
        }
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
