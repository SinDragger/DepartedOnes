using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitWeaponCondition : UnitCondition
{
    public string equipType;

    public override bool CheckCondition(AggregationEntity entity)
    {
        if(entity is UnitData)
        {
            UnitData unitData = (entity as UnitData);
            EquipSetData equipSetData = DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>(unitData.weaponEquipSetId);
            return equipSetData.EquipType == equipType;
        }
        return false;
    }
}
