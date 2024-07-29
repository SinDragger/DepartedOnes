using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestAction_EquipControl : QuestEventAction
{
    public string[] equipIds;
    public string[] equipAmount;

    public override void OnInvoke(EventData eventData)
    {
        for (int i = 0; i < equipIds.Length; i++)
        {
            LegionManager.Instance.nowLegion.ChangeEquip(DataBaseManager.Instance.GetIdNameDataFromList<EquipSetData>(equipIds[i]), int.Parse(equipAmount[i]), true);
        }
    }
}
