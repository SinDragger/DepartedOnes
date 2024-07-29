using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Linq;

public class BattleGeneralSpellListUI : UIPanel
{
    public override string uiPanelName => "BattleGeneralSpellListUI";

    public GameObject prefab;

    public GameObject showPanel;

    public string[] spellArray;

    CommandUnit command;
    public List<ItemSlot> subItems;
    public void Init(CommandUnit command)
    {
        this.command = command;
    }

    public void SetCastCommandUnit(CommandUnit commandUnit)
    {
        if (commandUnit == null) return;
        spellArray = commandUnit.GetCastableSpells();

        for (int i = subItems.Count; i < spellArray.Length; i++)
        {
            CreateSubBtn();
        }
        for (int i = 0; i < spellArray.Length; i++)
        {
            InitSubBtn(subItems[i], spellArray[i]);
        }
    }

    public void SetCastCommandUnit(string[] spellArray)
    {
        this.spellArray = spellArray;
        for (int i = subItems.Count; i < spellArray.Length; i++)
        {
            CreateSubBtn();
        }
        for (int i = spellArray.Length; i < subItems.Count; i++)
        {
            subItems[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < spellArray.Length; i++)
        {
            //subItems[i].transform.position = points[i].position;
            InitSubBtn(subItems[i], spellArray[i]);
            subItems[i].gameObject.SetActive(true);
        }
    }

    public void CreateSubBtn()
    {
        var g = Instantiate(prefab, transform.position, transform.rotation, transform);
        g.gameObject.SetActive(true);
        subItems.Add(g.GetComponent<ItemSlot>());
    }

    public void InitSubBtn(ItemSlot button, string idName)
    {
        var spell = DataBaseManager.Instance.GetIdNameDataFromList<CastableSpellData>(idName);
        button.GetComponent<SpellPanelTip>().spell = spell;
        button.GetComponent<Button>().SetBtnEvent(() =>
        {
            BattleCastManager.instance.SwitchModeIntoCastSpell(idName);
        });
        button.iconImage.sprite = DataBaseManager.Instance.GetSpriteByIdName(spell.iconResIdName);
    }


    public override void OnShow(bool withAnim = true)
    {
        //通过技能名字 获取到技能数据 icon 
        SetCastCommandUnit(command);
        showPanel.SetActive(true);
        base.OnShow(withAnim);
    }

    public override void OnHide(bool withAnim = true)
    {
        showPanel.SetActive(false);
        base.OnHide(withAnim);
    }
    private void Update()
    {

        //if (showPanel.activeSelf)
        //{

        //    UpdateUI();

        //    //目前没做cd


        //}
    }

    void UpdateUI()
    {



    }

    public void SwitchModeIntoCastSpell(string spellIdName)
    {

        //DataReset();
        //CloseChooseWheel();
        //nowCastSpell = DataBaseManager.Instance.GetIdNameDataFromList<CastableSpellData>(spellIdName);
        //indicate.transform.position = InputManager.Instance.mouseWorldPos + new Vector3(0, 50, 0);
        //troopSpell_Input.Active();
        //indicate.gameObject.SetActive(true);
    }
}
