using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellProgressSlot : MonoBehaviour
{
    public Text title;
    public Text des;
    public ItemSlot iconImage;
    public void Init(string idName)
    {
        var status = DataBaseManager.Instance.GetIdNameDataFromList<StandardStatus>(idName);
        title.text = status.name;
        des.text = status.effectDescribe;
        var spellIdName = GameManager.instance.playerData.progressToSpellDic[idName];
        var castSpell = DataBaseManager.Instance.GetIdNameDataFromList<CastableSpellData>(spellIdName);
        iconImage.SetImage(DataBaseManager.Instance.GetSpriteByIdName(castSpell.iconResIdName));
        iconImage.GetComponent<SpellPanelTip>().spell = castSpell;
    }
}