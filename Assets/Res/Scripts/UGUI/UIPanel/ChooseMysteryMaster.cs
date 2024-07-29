using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseMysteryMaster : UIPanel
{
    public override string uiPanelName => "ChooseMysteryMaster";
    public NewSpellSlot newSpellSlot;
    public TroopTraitSlot troopTraitSlot;
    public SpellProgressSlot spellProgressSlot;

    public Transform[] pos;
    /// <summary>
    /// 3个Slot
    /// </summary>
    List<GameObject> slots = new List<GameObject>();
    public override void OnShow(bool withAnim = true)
    {
        //新法术-泥沼术
        var data = GameManager.instance.playerData.GetRandomAbleMystery(3);
        for (int i = 0; i < 3; i++)
        {
            switch (data[i].Item1)
            {
                case 0: CreateNewSpellSlot(i, data[i].Item2); break;
                case 1: CreateSpellProgressSlot(i, data[i].Item2); break;
                case 2: CreateTroopTraitlSlot(i, data[i].Item2); break;
            }
        }
        //法术晋升-狂暴死灵
        //兵种特性解锁-箭雨
        newSpellSlot.gameObject.SetActive(false);
        troopTraitSlot.gameObject.SetActive(false);
        spellProgressSlot.gameObject.SetActive(false);
        base.OnShow(withAnim);
    }

    public void CreateNewSpellSlot(int flag, string idName)
    {
        var g = Instantiate(newSpellSlot.gameObject, transform);
        g.SetActive(true);
        g.transform.position = pos[flag].position;
        g.GetComponent<NewSpellSlot>().Init(idName);
        g.GetComponent<Button>().SetBtnEvent(() =>
        {
            GameManager.instance.playerData.ChooseNewSpell(idName);
            OnHide();
        });
    }
    public void CreateSpellProgressSlot(int flag, string idName)
    {
        var g = Instantiate(spellProgressSlot.gameObject, transform);
        g.SetActive(true);
        g.transform.position = pos[flag].position;
        g.GetComponent<SpellProgressSlot>().Init(idName);
        g.GetComponent<Button>().SetBtnEvent(() =>
        {
            GameManager.instance.playerData.ChooseSpellProgress(idName);
            OnHide();
        });
        //前置与关联性
    }
    public void CreateTroopTraitlSlot(int flag, string idName)
    {
        var g = Instantiate(troopTraitSlot.gameObject, transform);
        g.SetActive(true);
        g.transform.position = pos[flag].position;
        g.GetComponent<TroopTraitSlot>().Init(idName);
        g.GetComponent<Button>().SetBtnEvent(() =>
        {
            GameManager.instance.playerData.ChooseTroopTrait(idName);
            OnHide();
        });
        //前置与关联性
    }
    public override void OnHide(bool withAnim = true)
    {
        if (slots.Count > 0)
        {
            foreach (var slot in slots)
            {
                Destroy(slot.gameObject);
            }
        }
        slots.Clear();
        base.OnHide(withAnim);
        UIManager.Instance.ShowUI("ChooseNextPanel");
    }

}