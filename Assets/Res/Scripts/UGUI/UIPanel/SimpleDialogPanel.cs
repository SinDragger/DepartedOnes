using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleDialogPanel : UIPanel
{
    public System.Action callback;
    public Text contentText;
    public Button enter;
    public override string uiPanelName => "SimpleDialogPanel";

    public void Init(string content, System.Action callback)
    {
        contentText.text = content;
        enter.SetBtnEvent(() =>
        {
            OnHide();
            callback?.Invoke();
        });
    }

    public override void OnShow(bool withAnim = true)
    {
        TimeManager.Instance.SetToStop();
        base.OnShow(withAnim);
    }

    public override void OnHide(bool withAnim = true)
    {
        TimeManager.Instance.SetToRecovery();
        base.OnHide(withAnim);
    }
}
