using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewSpellSlot : MonoBehaviour
{
    public Text spellTitle;
    public ItemSlot spellIcon;

    // Start is called before the first frame update
    public void Init(string idName)
    {
        var castSpell = DataBaseManager.Instance.GetIdNameDataFromList<CastableSpellData>(idName);
        spellIcon.SetImage(DataBaseManager.Instance.GetSpriteByIdName(castSpell.iconResIdName));
        spellTitle.text = castSpell.name;
        spellIcon.GetComponent<SpellPanelTip>().spell = castSpell;
    }
}
