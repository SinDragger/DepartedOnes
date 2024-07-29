using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrepareTroopSlot : MonoBehaviour
{
    public RectTransform content;
    public LegionTroopSlot slot;
    public GameObject selectBorder;
    public Image icon;
    public Sprite defendSprite;
    public Sprite attackSprite;
    public bool isAttackState;
    public int nowX;
    public int nowY;

    public void Init(TroopControl troop)
    {
        slot.Init(troop);
    }

    public void SetOnClick(System.Action onClick)
    {
        GetComponent<Button>().SetBtnEvent(() => onClick?.Invoke());
    }

    public void OnSelected(bool value)
    {
        selectBorder.SetActive(value);
    }
    public void OnClick()
    {
        if (!BattlePreparePanel.isCreateMode && slot.troopData.belong != GameManager.instance.belong)
        {
            return;
        }
        isAttackState = !isAttackState;
        if (isAttackState)
        {
            icon.sprite = attackSprite;
        }
        else
        {
            icon.sprite = defendSprite;
        }
    }
    public void OnRightClick()
    {
        UIManager.Instance.ShowUI("UnitDetailPanel", (ui) =>
        {
            (ui as UnitDetailPanel).InitByTroop(slot.troopData);
        });
    }
    public void SetAttack()
    {
        isAttackState = true;
        icon.sprite = attackSprite;
    }
}
