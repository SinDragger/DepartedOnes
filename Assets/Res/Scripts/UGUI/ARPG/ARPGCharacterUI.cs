using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARPGCharacterUI : UIPanel
{
    public override string uiPanelName => "ARPGCharacterUI";

    public GameObject showPanel;

    public Image nowHPImage;

    public Image nowEnergyImage;

    SoldierStatus status;
    float NowHP => status.nowHp;
    float MaxHP => status.EntityData.maxLife;

    public Text hp;

    General general;
    float NowEnergy => general.nowEnergy;
    float MaxEnergy => general.sourceData.maxEnergy;

    public Text energy;

    //public void Start()
    //{
    //    OnShow();
    //}

    //void Update()
    //{
    //    if (showPanel.activeSelf)
    //    {
    //        UpdateUI();
    //    }
    //}

    private void UpdateUI()
    {
        if (status == null) return;
        nowHPImage.fillAmount = NowHP / MaxHP;
        hp.text = NowHP + " / " + MaxHP;
        if (general == null) return;
        nowEnergyImage.fillAmount = NowEnergy / MaxEnergy;
        energy.text = ((int)NowEnergy) + " / " + MaxEnergy;
    }

    public void Init(SoldierStatus status, General general)
    {
        //TODO:改为外部传参
        this.status = status;
        this.general = general;

    }


    public override void OnShow(bool withAnim = true)
    {
        Debug.LogError("Show");
        showPanel.SetActive(true);
        base.OnShow(withAnim);
    }

    public override void OnHide(bool withAnim = true)
    {
        showPanel.SetActive(false);
        base.OnHide(withAnim);
    }



}
