using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EAUIPanel : UIPanel
{
    public override string uiPanelName => "EAUIPanel";
    public Button clickButton;
    public CanvasGroup canvasGroup;

    public override void OnInit()
    {
        base.OnInit();
        clickButton.SetBtnEvent(() =>
        {
            OnHide();
        });
    }

    public override void OnShow(bool withAnim = true)
    {
        transform.localScale = Vector3.one;
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.4f);
        transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 1, 0.4f);
        base.OnShow(withAnim);
    }

    public override void OnHide(bool withAnim = true)
    {
        transform.DOScale(0.9f, 0.2f);
        canvasGroup.DOFade(0f, 0.2f).OnComplete(()=> {
            base.OnHide(withAnim);
        });
    }
}
