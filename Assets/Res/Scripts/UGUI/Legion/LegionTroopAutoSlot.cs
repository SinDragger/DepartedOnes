using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 自动作战的TroopSlot
/// </summary>
public class LegionTroopAutoSlot : LegionTroopSlot
{
    public AutoBattleTroop autoTroop;
    public Text uiText;
    public Sprite attack;
    public Sprite move;
    public Image motionIndicate;
    public Image motionBackGround;
    public Transform pos;
    int actionState;
    public bool isFaceRight;//朝向右边

    public override void SetNumber(int max, int unitNumber)
    {
        float unitPercent = (float)unitNumber / (float)max;
        numberFill.fillAmount = unitPercent;
        uiText.text = unitNumber.ToString();
    }
    public void SetFaceTo(bool isRight)
    {
        if (isFaceRight != isRight)
        {
            if (isRight)
            {
                pos.localEulerAngles = new Vector3(0, 0, 0);
                uiText.transform.localPosition = new Vector3(59, -21, 0);
            }
            else
            {
                pos.localEulerAngles = new Vector3(0, 180, 0);
                uiText.transform.localPosition = new Vector3(-59, -21, 0);
            }
        }
        isFaceRight = isRight;
    }

    //移动20进行
    public void PosAnimUpdate(float nowTime)
    {
        float percent = autoTroop.GetMotionPercent(nowTime);
        float min = 0.9f;
        float max = 1f;
        if (percent < min || percent > max)
        {
            SetMotionPerparePercent(percent / min);
            percent = 0f;
        }
        else
        {
            percent = (percent - min) / (max - min);
        }
        percent *= 2;
        if (percent > 1f)
        {
            percent = 2f - percent;
        }
        transform.localPosition = new Vector3((isFaceRight ? 20 : -20) * percent, transform.localPosition.y, 0);
        SetNumber(autoTroop.originTroop.maxNum, autoTroop.originTroop.nowNum);
        if (actionState != autoTroop.actionState)
        {
            actionState = autoTroop.actionState;
            switch (actionState)
            {
                case 0:
                    motionIndicate.sprite = attack;
                    motionBackGround.sprite = attack;
                    break;
                case 1:
                    motionIndicate.sprite = move;
                    motionBackGround.sprite = move;
                    break;
            }
        }
    }

    public void SetMotionPerparePercent(float percent)
    {
        motionIndicate.fillAmount = percent;
    }
}
