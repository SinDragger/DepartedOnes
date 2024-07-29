using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LegionDownDetailUI : UIPanel
{
    public override string uiPanelName => "LegionDownDetailUI";
    public Button collectButton;
    public Button buildButton;
    public Button rearmamentButton;
    public GameObject buildArea;
    public GameObject showPanel;
    LegionControl nowLegion;
    private void Start()
    {
        collectButton.SetBtnEvent(() =>
        {
            UIManager.Instance.ShowUI("SpeciesPanel", (ui) =>
            {
                (ui as SpeciesPanel).Init(GameManager.instance.playerForce);
            });
            //nowLegion.State = new LegionCollectState(nowLegion, SectorBlockManager.Instance.GetBlock(nowLegion.position));
        });
        rearmamentButton.SetBtnEvent(() => {
            UIManager.Instance.ShowUI("UnitOrganizationPanel", (ui) =>
            {
                (ui as UnitOrganizationPanel).Init(GameManager.instance.playerForce);
            });
        }); 
        buildButton.SetBtnEvent(() => {
            //buildArea.gameObject.SetActive(!buildArea.activeSelf);
        });
    }

    public override void OnShow(bool withAnim = true)
    {
        showPanel.SetActive(true);
        base.OnShow(withAnim);
    }

    public override void OnHide(bool withAnim = true)
    {
        showPanel.SetActive(false);
        base.OnHide(withAnim);
    }

    /// <summary>
    /// 获取当前Legion的装备缺口Click
    /// TODO:快速补充：点击移动向最近工匠营地进行修整。劳力转职
    /// 配给补充：详细面板进行展示
    /// 工匠营地可以预生产
    /// </summary>
    public void GetNowLegionEquipGap()
    {

    }
}
