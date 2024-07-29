using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Map样式UI
/// </summary>
public class MapConstructionUI : UIPanel
{
    public override string uiPanelName => "MapConstructionUI";
    public GameObject showPanel;
    /// <summary>
    /// 详情面板
    /// </summary>
    public Button buttonEnter;

    //资源变动列表
    Dictionary<string, int> resChangeDic = new Dictionary<string, int>();

    public WorkSidePanel workSidePanel;
    public RefugeeWorkButton searchWorkButton;
    public Text nameText;
    public Text desText;
    public Image buildingIcon;
    public GameObject workArea;
    public TextUnit resText;

    SectorConstruction data;
    void Start()
    {
        EventManager.Instance.RegistEvent(MapEventType.CONSTRUCTION_SELECT, (e) => InitByData(e.GetValue<SectorConstructionUI>("Building")));
        buttonEnter.SetBtnEvent(() =>
        {
            LegionManager.Instance.LegionEnterSectorConstruction(data.stationedLegions[0], data);
            //部队进入当前地点
        });
    }

    void Update()
    {
        if (data != null)
            UpdateUI();
    }

    void UpdateUI()
    {
        bool canEnter = true;
        if (data.stationedLegions.Count == 0 || data.stationedLegions[0].belong != GameManager.instance.belong || data.stationedLegions[0].mainGeneral == null)
        {
            canEnter = false;
        }
        if (string.IsNullOrEmpty(data.constructionData.relatedMapId))
            canEnter = false;
        buttonEnter.gameObject.SetActive(canEnter);
        //if (uiBars.Count < data.workList.Count)
        //{
        //    for (int i = uiBars.Count; i < data.workList.Count; i++)
        //    {
        //        uiBars.Add(Instantiate(originPrefab, parentContent).GetComponent<WorkUIBar>());
        //    }
        //}
        //for (int i = 0; i < data.workList.Count; i++)
        //{
        //    uiBars[i].UpdateUI(data.workList[i]);
        //}
    }

    public void InitByData(SectorConstructionUI ui)
    {
        data = ui.sectorConstruction;
        desText.text = data.constructionData.des;
        nameText.text = data.constructionName;
        resChangeDic.Clear();
        bool isAble = false;
        for (int i = 0; i < data.workList.Count; i++)
        {
            data.workList[i].LoadResourceChange(resChangeDic);
            if (data.workList[i].workType == "REFUGEE") isAble = true;
        }
        resText.SpriteClear();
        string textString = "";
        if (isAble)
        {
            int flag = 0;
            if (resChangeDic.Count > 0)
            {
                foreach (var entity in resChangeDic)
                {
                    resText.SetSprite(flag, DataBaseManager.Instance.GetSpriteByIdName(DataBaseManager.Instance.GetIdNameDataFromList<Resource>(entity.Key).idName));
                    textString += $"{"<quad>"}{entity.Value} ";
                    flag++;
                }
                buildingIcon.rectTransform.sizeDelta = new Vector2(160, 75f);
            }
            else
            {
                buildingIcon.rectTransform.sizeDelta = new Vector2(160, 115f);
            }
            workArea.SetActive(true);
            var work = data.workList[0];
            searchWorkButton.Init(work, workSidePanel);
        }
        else
        {
            buildingIcon.rectTransform.sizeDelta = new Vector2(160, 115f);
            workArea.SetActive(false);
        }
        buildingIcon.sprite = ui.icon.sprite;
        resText.SetText(textString);
        workSidePanel.gameObject.SetActive(false);
        UpdateUI();
    }
    public override void OnShow(bool withAnim = true)
    {
        showPanel.gameObject.SetActive(true);
        //for (int i = uiBars.Count; i < data.workList.Count; i++)
        //{
        //    uiBars.Add(Instantiate(originPrefab, parentContent).GetComponent<WorkUIBar>());
        //}
        //for (int i = 0; i < data.workList.Count; i++)
        //{
        //    uiBars[i].gameObject.SetActive(true);
        //}
        //for (int i = data.workList.Count; i < uiBars.Count; i++)
        //{
        //    uiBars[i].gameObject.SetActive(false);
        //}
        base.OnShow(withAnim);
    }
    public override void OnHide(bool withAnim = true)
    {
        showPanel.gameObject.SetActive(false);
        base.OnHide(withAnim);
    }
}
