using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CastRemind : UIPanel
{
    public ItemSlot castIcon;
    public CommandUnit target;
    public float countDown;
    public Image mask;
    public override string uiPanelName => "CastRemind";
    string spellId;

    public void Init(CommandUnit commandUnit, string idName, float countDown)
    {
        target = commandUnit;
        this.countDown = countDown;
        spellId = idName;
        var spell = DataBaseManager.Instance.GetIdNameDataFromList<CastableSpellData>(idName);
        GetComponent<SpellPanelTip>().spell = spell;
        GetComponent<ItemSlot>().iconImage.sprite = DataBaseManager.Instance.GetSpriteByIdName(spell.iconResIdName);
    }
    private void Update()
    {
        var prefIdName = $"SpellCast_{spellId}_CountDownTime";
        float left = target.GetFloatValue(prefIdName);
        mask.fillAmount = 1f - left / countDown;
        if (target.TroopState == TroopState.DESTROYED)
        {
            OnHide();
        }
    }

    public override void OnShow(bool withAnim = true)
    {
        base.OnShow(withAnim);
    }

    public override void OnHide(bool withAnim = true)
    {
        base.OnHide(withAnim);
    }

}
