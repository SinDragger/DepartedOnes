using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : UIPanel
{
    public Text showText;
    public string relatedName;
    public bool isLock
    {
        get;
        private set;
    }
    public RectTransform rect;
    public Image circle;
    public Image bg;
    public Sprite originSprite;
    public Sprite lockSprite;
    public bool IsMouseIn()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, Input.mousePosition, CameraControl.Instance.mainCamera);
    }

    private void Update()
    {
        if (isLock && IsMouseIn())
        {
            UIManager.Instance.NowWithinRelated(relatedName);
        }
        //SetText(showText.text);
    }
    float textHeight;
    public void SetText(string text)
    {
        float with = 450;
        float curWith = 0;
        showText.text = text;
        if (showText.preferredWidth < with)
            curWith = showText.preferredWidth;
        else
            curWith = with;
        showText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, curWith);
        showText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, showText.preferredHeight);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, curWith + 50);
        textHeight = showText.preferredHeight + 50;
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textHeight);
    }

    public void SetLocalPos(Vector2 localPos, bool isUp = true)
    {
        if (isUp)
        {
            transform.localPosition = localPos + new Vector2(0, textHeight / 2);
        }
        else
        {
            transform.localPosition = localPos - new Vector2(0, textHeight / 2);
        }
    }

    public void SetLockOn()
    {
        isLock = true;
        circle.gameObject.SetActive(false);
        bg.sprite = lockSprite;
    }

    public void UpdateUnlock(float percent)
    {
        circle.fillAmount = percent;
    }

    public override void OnShow(bool withAnim = true)
    {
        gameObject.SetActive(true);
        base.OnShow(withAnim);
    }

    public override void OnHide(bool withAnim = true)
    {
        gameObject.SetActive(false);
        circle.gameObject.SetActive(true);
        bg.sprite = originSprite;
        isLock = false;
        relatedName = null;
        base.OnHide(withAnim);
    }
}
