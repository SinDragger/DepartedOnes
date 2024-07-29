using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TroopTraitSlot : MonoBehaviour
{
    public Text title;
    public Text des;
    public LegionTroopSlot troopSlot;
    // Start is called before the first frame update
    public void Init(string idName)
    {
        var status = DataBaseManager.Instance.GetIdNameDataFromList<StandardStatus>(idName);
        title.text = status.name;
        des.text = status.effectDescribe;
        var unitIdName = GameManager.instance.playerData.traitsToUnitDic[idName];
        var unit = DataBaseManager.Instance.GetIdNameDataFromList<UnitData>(unitIdName);
        troopSlot.Init(unit);
    }
}
