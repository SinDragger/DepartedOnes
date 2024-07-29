using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCompletePanel : UIPanel
{
    public override string uiPanelName => "BattleCompletePanel";
    public Button clickButton;
    public CanvasGroup canvasGroup;
    public GameObject[] rewardAreas;

    public override void OnInit()
    {
        base.OnInit();
        clickButton.SetBtnEvent(() =>
        {
            if (GameManager.instance.nowLayerCount >= 8)
            {
                SceneManager.Instance.BackToStartMenu(0f);
                UIManager.Instance.ShowUI("BattleWinPanel");
                //显示已经打通
            }
            else
            {
                UIManager.Instance.ShowUI("ChooseTroopGet");
            }
            OnHide();
        });
    }

    public override void OnShow(bool withAnim = true)
    {
        //结算

        transform.localScale = Vector3.one;
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.4f);
        transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 1, 0.4f);
        base.OnShow(withAnim);
    }

    public override void OnHide(bool withAnim = true)
    {
        BattleManager.instance.OnBattleEnd();
        transform.DOScale(0.9f, 0.2f);
        canvasGroup.DOFade(0f, 0.2f).OnComplete(() =>
        {
            base.OnHide(withAnim);
        });
    }
}
