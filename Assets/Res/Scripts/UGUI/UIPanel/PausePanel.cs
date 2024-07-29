using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : UIPanel
{
    public override string uiPanelName => "PausePanel";

    public Button restore;
    public Button exit;

    private void Start()
    {
        restore.SetBtnEvent(() => OnHide());
        exit.SetBtnEvent(() =>
        {
            Time.timeScale = 1f;
            OnHide();
            SceneManager.Instance.BackToStartMenu(0f);
        });
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
    public override void OnHide(bool withAnim = true)
    {
        base.OnHide(withAnim);
    }
}
