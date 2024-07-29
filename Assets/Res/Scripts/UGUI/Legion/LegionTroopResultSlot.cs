using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 军团部队的结算界面格子
/// </summary>
public class LegionTroopResultSlot : LegionTroopSlot
{
    public Text uiText;
    public float limitMaxPercent;
    public override void SetNumber(int max, int unitNumber)
    {
        float unitPercent = (float)unitNumber / (float)max;
        numberFill.fillAmount = unitPercent;
        uiText.text = unitNumber.ToString();
    }

    public override void OnClick()
    {
        //显示部队详情
    }

}
