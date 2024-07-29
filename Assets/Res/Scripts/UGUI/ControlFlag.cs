using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ControlFlag : MonoBehaviour
{
    public CanvasGroup canvas;
    public Image iconImage;
    public Image typeImage;
    public Image blackCover;
    public CommandUnit commander;
    public GameObject onChooseEffect;
    public static float upDelta = 30f;
    public static float canvasAlpha = 1f;

    public Image flank;

    public const float MIN_SIZE = 0.20f;
    public const float MAX_SIZE = 0.24f;
    //public static ControlFlag lastControl;
    public bool isCommand = false;
    public Image moraleImage;
    public Text moraleValue;
    private void OnEnable()
    {
        transform.localEulerAngles = Vector3.zero;
    }
    void LateUpdate()
    {
        UpdateShow();
    }

    public void Init(CommandUnit commander)
    {
        this.commander = commander;
        transform.localScale = Vector3.one * MIN_SIZE;
        flank.gameObject.SetActive(false);
        //right.gameObject.SetActive(false);
        //back.gameObject.SetActive(false);
    }

    //显示目标单位的UI
    public void InitDetail(TroopEntity unit)
    {
        InitDetail(unit.unitType, unit.weaponEquipSet.data);
    }
    public void SetBackColor(Color backColor)
    {
        iconImage.color = backColor;
        moraleImage.color = Color.Lerp(backColor, Color.white, 0.3f);
    }
    //显示目标单位的UI
    public void InitDetail(UnitType unitType, EquipSetData weaponEquip)
    {
        if (typeImage != null)
        {
            string spriteString = $"UnitType_{unitType}";
            if (weaponEquip == null)
            {
                typeImage.sprite = DataBaseManager.Instance.GetSpriteByIdName(spriteString);
                return;
            }
            if (unitType == UnitType.MONSTER)
            {
                typeImage.transform.localPosition = new Vector3(0, -10, 0);
            }
            else
            {
                typeImage.transform.localPosition = new Vector3(0, 10, 0);
            }
            if (unitType == UnitType.FIGHTER)
            {
                var s = weaponEquip.TargetActionModel;
                if (s == ModelMotionType.RANGE.ToString())
                {
                    spriteString += "_RANGE";
                    //typeImage.sprite = DataBaseManager.Instance.GetSpriteByIdName(unitType.ToString());
                }
                else if (s == ModelMotionType.POLEARMS.ToString())
                {

                    spriteString += "_POLEARMS";
                    //typeImage.sprite = DataBaseManager.Instance.GetSpriteByIdName(unitType.ToString());
                }
                else if (s == ModelMotionType.TWOHANDED.ToString())
                {
                    //spriteString += "_POLEARMS";
                }
            }
            else
            {
                var equipType = weaponEquip.EquipType;
                if (!string.IsNullOrEmpty(equipType))
                {
                    if (equipType == "Sword")
                    {
                        typeImage.sprite = DataBaseManager.Instance.GetSpriteByIdName("UnitType_FIGHTER");
                        return;
                    }
                    else if (equipType == "AxeSheild")
                    {
                        typeImage.sprite = DataBaseManager.Instance.GetSpriteByIdName("UnitType_FIGHTER_AXESHEILD");
                        return;
                    }
                    else if (equipType == "Pike")
                    {
                        typeImage.sprite = DataBaseManager.Instance.GetSpriteByIdName("UnitType_FIGHTER_POLEARMS");
                        return;
                    }
                }
                //查武器类型
            }
            typeImage.sprite = DataBaseManager.Instance.GetSpriteByIdName(spriteString);
        }
    }
    public void UpdateShow()
    {
        if (commander == null) return;
        Vector3 pos = commander.lastPosition;
        if (commander.centerSoldier != null && commander.centerSoldier.nowHp > 0)
        {
            pos = commander.centerSoldier.model.lastPosition;
        }
        Vector3 focusPosition = Camera.main.WorldToScreenPoint(pos) - new Vector3(Screen.width / 2, Screen.height / 2 - upDelta, 0);
        canvas.alpha = Mathf.Clamp(canvasAlpha, 0.2f, 1f);
        focusPosition.z = 0;
        transform.localPosition = focusPosition;
        flank.gameObject.SetActive(commander.isBeFlank);
        if (commander.isBeFlank)
        {
            if (commander.GetBoolValue(Constant_AttributeString.STATUS_RESIST_FLANK))
            {
                flank.color = Color.black;
                if (commander.GetBoolValue(Constant_AttributeString.STATUS_BATTLE_FUROR))
                {
                    flank.color = Color.green;
                }
            }
            else
            {
                flank.color = Color.red;
            }
        }
        moraleImage.fillAmount = commander.CommandUnit_NowHP / (float)commander.CommandUnit_TotalHP;
        moraleValue.text = commander.NowMorale.ToString();
    }

    public void SetOnChoose(bool value)
    {
        onChooseEffect.SetActive(value);
    }

    /// <summary>
    /// 收到点击时的光暗变更
    /// </summary>
    public void TriggerShinyAnim()
    {
        blackCover.DOKill();
        blackCover.color = Color.clear;
        blackCover.DOFade(0.5f, 0.1f).OnComplete(() =>
        {
            blackCover.DOFade(0f, 0.1f).OnComplete(() =>
            {
            });
        });
    }
    /// <summary>
    /// 扩大0.1f的将选中
    /// </summary>
    public void OnWillSelectAnim()
    {

    }

    /// <summary>
    /// 缩回默认值
    /// </summary>
    public void OnRemoveWillSelectAnim()
    {

    }

    public void OnClick()
    {
        //if (!commander.belong.Equals(BattleManager.instance.controlBelong)) return;
        //UnitControlManager.instance.TargetCommondModelClick(commander);
    }
}
