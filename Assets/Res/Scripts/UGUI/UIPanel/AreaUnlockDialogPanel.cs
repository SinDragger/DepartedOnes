using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaUnlockDialogPanel : UIPanel
{
    public Text contentText;
    public Button enter;
    public override string uiPanelName => "AreaUnlockDialogPanel";

    public void Init(string content)
    {
        contentText.text = content;
        enter.SetBtnEvent(() =>
        {
            OnHide();
            SectorBlockManager.Instance.UpdateToNextShowLevel();
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
