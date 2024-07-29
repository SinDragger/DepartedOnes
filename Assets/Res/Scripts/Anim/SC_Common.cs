using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Common : SpineUnityControl
{
    public EquipAbleModelData data;
    ///将要构成的伤害类型
    ///1:挥砍 2:刺击 3:
    public int forceDamageType;
    public ModelMotionType motionType;
    public System.Func<ModelMotionType, string> attackMotionTakeOver;

    public void Init(SoldierStatus statusData)
    {
        var model = DataBaseManager.Instance.GetIdNameDataFromList<EquipAbleModelData>(DataBaseManager.Instance.GetModelName(statusData.EntityData));
        data = model;
        var actionMotion = statusData.EntityData.weaponEquipSet.data.TargetActionModel;
        if (System.Enum.TryParse<ModelMotionType>(actionMotion, out ModelMotionType result))
        {
            motionType = result;
            if (result == ModelMotionType.RANGE && statusData.GetBoolValue("DisableRemote"))
            {
                motionType = ModelMotionType.MELEE;
            }
        }
        CoroutineManager.StartWaitUntil(() => selfSkelAni != null && selfSkelAni.skeletonDataAsset != null,
            () =>
            {
                PlayIdle();
            });
    }

    public override string GetAnimationName(AnimState state)
    {
        string result = "";
        switch (state)
        {
            case AnimState.DEFAULT:
                switch (motionType)
                {
                    case ModelMotionType.MELEE:
                        result = data.idleMotionName_Melee;
                        break;
                    case ModelMotionType.POLEARMS:
                        result = data.idleMotionName_Polearm;
                        break;
                    case ModelMotionType.TWOHANDED:
                        result = data.idleMotionName_TwoHanded;
                        break;
                    default:
                        result = data.idleMotionName_Melee;
                        break;
                }
                break;
            case AnimState.IDLE:
                switch (motionType)
                {
                    case ModelMotionType.MELEE:
                        result = data.idleMotionName_Melee;
                        break;
                    case ModelMotionType.POLEARMS:
                        result = data.idleMotionName_Polearm;
                        break;
                    case ModelMotionType.TWOHANDED:
                        result = data.idleMotionName_TwoHanded;
                        break;
                    default:
                        result = data.idleMotionName_Melee;
                        break;
                }
                break;
            case AnimState.ATTACK:
                if (attackMotionTakeOver != null)
                {
                    result = attackMotionTakeOver.Invoke(motionType);
                    break;
                }
                switch (motionType)
                {
                    case ModelMotionType.MELEE:
                        if (forceDamageType != 0)
                        {
                            if (forceDamageType == 1)
                                result = data.attackMotionName_Melee;
                        }
                        else
                        {
                            if (data.extraAttackMotionName_Melee != null && data.extraAttackMotionName_Melee.Length > 0)
                            {
                                int count = UnityEngine.Random.Range(0, data.extraAttackMotionName_Melee.Length + 1);
                                if (count == data.extraAttackMotionName_Melee.Length)
                                {
                                    result = data.attackMotionName_Melee;
                                }
                                else
                                {
                                    result = data.extraAttackMotionName_Melee[count].Trim();
                                }
                            }
                            else
                            {
                                result = data.attackMotionName_Melee;
                            }
                        }
                        break;
                    case ModelMotionType.POLEARMS:
                        result = data.attackMotionName_Polearm;
                        break;
                    case ModelMotionType.TWOHANDED:
                        result = data.attackMotionName_TwoHanded;
                        break;
                    case ModelMotionType.RANGE:
                        result = data.attackMotionName_Shoot;
                        break;
                    default:
                        result = data.attackMotionName_Melee;
                        break;
                }
                break;
            case AnimState.MOVE_BEGIN:
                switch (motionType)
                {
                    case ModelMotionType.MELEE:
                        result = data.moveMotionName_Melee;
                        break;
                    case ModelMotionType.POLEARMS:
                        result = data.moveMotionName_Polearm;
                        break;
                    case ModelMotionType.TWOHANDED:
                        result = data.moveMotionName_TwoHanded;
                        break;
                    default:
                        result = data.moveMotionName_Melee;
                        break;
                }
                break;
            case AnimState.MOVE_LOOP:
                switch (motionType)
                {
                    case ModelMotionType.MELEE:
                        result = data.moveMotionName_Melee;
                        break;
                    case ModelMotionType.POLEARMS:
                        result = data.moveMotionName_Polearm;
                        break;
                    case ModelMotionType.TWOHANDED:
                        result = data.moveMotionName_TwoHanded;
                        break;
                    default:
                        result = data.moveMotionName_Melee;
                        break;
                }
                break;
            case AnimState.MOVE_END:
                switch (motionType)
                {
                    case ModelMotionType.MELEE:
                        result = data.idleMotionName_Melee;
                        break;
                    case ModelMotionType.POLEARMS:
                        result = data.idleMotionName_Polearm;
                        break;
                    case ModelMotionType.TWOHANDED:
                        result = data.idleMotionName_TwoHanded;
                        break;
                    default:
                        result = data.idleMotionName_Melee;
                        break;
                }
                break;
            case AnimState.DIE:
                result = data.dieMotionName;
                if (!string.IsNullOrEmpty(data.dieMotionSecName) && Random.Range(0, 2) == 1)
                {
                    result = data.dieMotionSecName;
                }
                break;
        }
        return result;
    }
}
