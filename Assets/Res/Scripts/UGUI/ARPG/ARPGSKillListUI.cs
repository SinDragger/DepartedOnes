using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARPGSkillListUI : UIPanel
{
    public override string uiPanelName => "ARPGSkillListUI";
    public GameObject showPanel;
    public List<Image> skillImage;
    public List<Image> coolDownImages;
    public GameObject commandPanel;
    //public Text commandText;
    public Button commandButton;
    List<SkillData> skillData;
    List<ItemData> itemDatas;

    public void Init(General general)
    {
        SetSkillData(general.skillDatas);
        SetItenmData(general.itemDatas);
    }
    public void SetSkillData(List<SkillData> skillData)
    {
        //TODO 通过skillID查标获取Iamge 
        this.skillData = skillData;
    }
    public void SetItenmData(List<ItemData> itemDatas)
    {
        this.itemDatas = itemDatas;
    }
    private void Update()
    {

        if (showPanel.activeSelf)
        {
            if (skillData != null)
                UpdateUI();

            bool able = Input.GetKey(KeyCode.LeftControl);

        }
    }

    public void ChargeOrStop()
    {
        BattleManager.instance.autoCharge = !BattleManager.instance.autoCharge;
    }

    void UpdateUI()
    {

        for (int i = 0; i < skillData.Count; i++)
        {
            coolDownImages[i].fillAmount = skillData[i].coolRemain / skillData[i].coolTime;
        }

        for (int i = 8; i < 10; i++)
        {
            if ((i - 8) < itemDatas.Count)
                coolDownImages[i].fillAmount = itemDatas[i - 8].skillData.coolRemain / itemDatas[i - 8].skillData.coolTime;
        }
    }


    public override void OnShow(bool withAnim = true)
    {
        //TODO 根据skillID获取Iamge 还有ItemID获取Iamge
 
        showPanel.SetActive(true);
        base.OnShow(withAnim);
    }

    public override void OnHide(bool withAnim = true)
    {
        showPanel.SetActive(false);
        base.OnHide(withAnim);
    }



}
