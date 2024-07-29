using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResurrectionPanel : UIPanel
{
    public Button button;
    public Text costText;
    public override string uiPanelName => "ResurrectionPanel";
    float cost;
    private void OnEnable()
    {
        button.SetBtnEvent(Rise);
        cost = 100f;
    }
    private void Start()
    {
        GameManager.timeRelyMethods += UpdateTime;
    }
    void UpdateTime(float deltaTime)
    {
        cost -= deltaTime / 60f;
        if (cost < 0f) cost = 0f;
    }

    private void Update()
    {
        costText.text = $"代价:{(int)cost}";
    }

    void Rise()
    {
        int store = GameManager.instance.playerForce.GetLimitedRes(Constant_AttributeString.RES_SOULPOINT);
        if (store > cost)
        {
            GameManager.instance.playerForce.ChangeLimitedRes("SoulPoint", -(int)cost);
            LegionManager.Instance.DeployPlayerLegionToStartPos();
            OnHide();
        }
        else
        {
            GameManager.instance.ShowTip("灵魂元素不足");
        }
    }

    public override void OnShow(bool withAnim = true)
    {
        base.OnShow(withAnim);
        gameObject.SetActive(true);
    }
    public override void OnHide(bool withAnim = true)
    {
        base.OnHide(withAnim);
        gameObject.SetActive(false);
    }
}
