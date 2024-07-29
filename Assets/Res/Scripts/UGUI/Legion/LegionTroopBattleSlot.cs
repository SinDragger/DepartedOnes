using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LegionTroopBattleSlot : LegionTroopSlot
{
    public Text uiText;
    public float limitMaxPercent;
    public Image hideCover;
    CommandUnit commandUnit;
    public Image border;
    bool isHide;
    public override void SetNumber(int max, int unitNumber)
    {
        float unitPercent = (float)unitNumber / (float)max;
        numberFill.fillAmount = unitPercent * limitMaxPercent;
        uiText.text = unitNumber.ToString();
        if (unitNumber == 0 && !isHide)
        {
            transform.DOKill();
            transform.DOLocalMoveX(160, 0.1f).SetEase(Ease.InSine);
            hideCover.DOFade(0.6f, 0.1f);
            isHide = true;
            //Debug.LogError(transform.localPosition);
        }
        else if (unitNumber > 0 && isHide)
        {
            transform.DOKill();
            transform.DOLocalMoveX(100, 0f);
            hideCover.DOFade(0f, 0f);
            isHide = false;

        }
    }

    public override void OnClick()
    {
        if (commandUnit.troopState != TroopState.DESTROYED)
            UnitControlManager.instance.TargetCommondModelClick(commandUnit);
        if (!InputManager.Instance.isShiftMulAble || commandUnit.belong != BattleManager.instance.controlBelong)
        {
            CameraControl.Instance.SetDesiredPosition(commandUnit.lastPosition);
        }
    }

    void OnReset()
    {
        isHide = false;
        if (commandUnit != null)
        {
            commandUnit.onChoose -= SetOnChoose;
            commandUnit.onUnChoose -= SetOnUnChoose;
        }
        hideCover.color = Color.Lerp(Color.clear, Color.black, 0.2f);
        transform.localPosition = new Vector3(100, transform.localPosition.y, 0f);
        border.color = Color.white;
    }

    public override void Init(CommandUnit commandUnit)
    {
        if (this.commandUnit == commandUnit) return;
        OnReset();
        this.commandUnit = commandUnit;
        commandUnit.onChoose += SetOnChoose;
        commandUnit.onUnChoose += SetOnUnChoose;
        if (commandUnit.aliveCount == 0)
        {
            transform.localPosition = new Vector3(160, transform.localPosition.y, 0f);
            hideCover.color = Color.Lerp(Color.clear, Color.black, 0.6f);
        }
        else
        {
            if (commandUnit.isOnChoose)
            {
                SetOnChoose();
            }
        }
        base.Init(commandUnit);
        //BattleManager.instance.
        numberFill.transform.GetChild(0).GetComponent<Image>().color = GameManager.instance.GetForceColor(commandUnit.belong);
    }

    void SetOnChoose()
    {
        hideCover.DOKill();
        hideCover.DOFade(0f, 0.1f);
        border.color = Color.yellow;
    }

    void SetOnUnChoose()
    {
        hideCover.DOKill();
        border.color = Color.white;
        if (commandUnit.aliveCount == 0)
        {
            hideCover.color = Color.Lerp(Color.clear, Color.black, 0.6f);
        }
        else
        {
            hideCover.color = Color.Lerp(Color.clear, Color.black, 0.1f);
        }
    }
}
