using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextChooseSlot : MonoBehaviour
{
    public Text titleText;
    public UnitPanelSlot unitSlot;

    public void Init(string idName)
    {
        BattleMapData battleMapData = DataBaseManager.Instance.GetIdNameDataFromList<BattleMapData>(idName);
        titleText.text = battleMapData.troopName;
        if (unitSlot)
        {
            UnitData data = DataBaseManager.Instance.GetIdNameDataFromList<UnitData>(battleMapData.relatedRogueTroop);
            unitSlot.Init(data);
            var color = GameManager.instance.GetForceColor(GameManager.instance.belong);
            unitSlot.slot.numberColor.color = color;
        }
    }
}
