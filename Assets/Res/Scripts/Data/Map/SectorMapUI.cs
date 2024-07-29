using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SectorMapUI : UIPanel, ISelectAble
{
    public Transform root;
    public List<SectorControlBar> controlBars = new List<SectorControlBar>();
    public Text titleLandform;
    public Text forceText;
    public Image landformIcon;
    public WorkSidePanel workSidePanel;
    public RefugeeWorkButton searchWorkButton;
    SectorBlock data;
    public void Start()
    {
        EventManager.Instance.RegistEvent(MapEventType.SECTOR_SELECT, (e) => InitByData(e.GetValue<SectorBlock>("SectorBlock")));
        //foreach (var bar in controlBars)
        //{
        //    bar.SetBlock(0.2f);
        //}
    }

    bool hasShow;
    public void InitByData(SectorBlock data)
    {
        hasShow = false;
        if (data != null)
        {
            this.data = data;
            UIManager.Instance.SetNowSelect(this);
            float progress = data.GetProgress(GameManager.instance.belong);
            if (progress == 0)
            {
                titleLandform.text = "未知";
                landformIcon.sprite = DataBaseManager.Instance.GetSpriteByIdName($"Landform_Unknown");
                forceText.color = Color.black;
                forceText.text = "未知";
            }
            else
            {
                hasShow = true;
                Landform landform = DataBaseManager.Instance.GetIdNameDataFromList<Landform>(data.originData.landform);
                titleLandform.text = landform.name;
                landformIcon.sprite = landform.GetSprite();

                //没有可以搜索的内容
                if (data.searchAbleWorks.Count == 0)
                {
                    searchWorkButton.gameObject.SetActive(false);
                }
                else
                {
                    var work = data.searchAbleWorks[0];
                    //searchWorkButton.gameObject.SetActive(true);
                    searchWorkButton.Init(work, workSidePanel);
                }
                int belong = data.belong;
                if (belong != -1)
                {
                    forceText.color = GameManager.instance.GetForceColor(data.belong);
                    var force = DataBaseManager.Instance.GetTargetDataList<ForceData>().Find((f) => f.id == data.belong);
                    forceText.text = force.name;
                }
                else
                {
                    forceText.color = Color.black;
                    forceText.text = "未知";
                }
            }
            UpdateUI();
        }
    }

    void Update()
    {
        if (data != null)
            UpdateUI();
    }

    void UpdateUI()
    {
        int maxShow = Mathf.Min(controlBars.Count, data.progressList.Count);
        controlBars[0].SetBarColor(GameManager.instance.GetForceColor(data.belong));
        controlBars[0].SetBarProgress(data.GetProgress(data.belong) / 100f);
        controlBars[0].SetBarProgressMax(1f);
        //for (int i = 0; i < maxShow; i++)
        //{
        //    controlBars[i].gameObject.SetActive(true);
        //    controlBars[i].SetBarColor(GameManager.instance.GetForceColor(data.progressList[i].belong));
        //    controlBars[i].SetBarProgress(data.progressList[i].controlProcess / 100f);
        //    controlBars[i].SetBarProgressMax(data.progressList[i].controlMax / 100f);
        //}
        for (int i = maxShow; i < controlBars.Count; i++)
        {
            controlBars[i].gameObject.SetActive(false);
        }
        if (!hasShow && data.GetProgress(GameManager.instance.belong) > 0)
        {
            hasShow = true;
            Landform landform = DataBaseManager.Instance.GetIdNameDataFromList<Landform>(data.originData.landform);
            titleLandform.text = landform.name;
            landformIcon.sprite = landform.GetSprite();
            //没有可以搜索的内容
            int belong = data.belong;
            if (belong != -1)
            {
                forceText.color = GameManager.instance.GetForceColor(data.belong);
                var force = DataBaseManager.Instance.GetTargetDataList<ForceData>().Find((f) => f.id == data.belong);
                forceText.text = force.name;
            }
            else
            {
                forceText.color = Color.black;
                forceText.text = "未知";
            }
        }
        //向Legion获取所有在其上的
        if (data.searchAbleWorks.Count > 0)
        {
            var work = data.searchAbleWorks[0];
            searchWorkButton.Init(work,workSidePanel);
        }
        else
        {
            searchWorkButton.gameObject.SetActive(false);
            workSidePanel.gameObject.SetActive(false);
        }
    }

    public void OnSelect(bool value)
    {
        root.gameObject.SetActive(value);
        if (!value)
        {
            SectorBlockManager.Instance.TargetBlockOnSelect(default);
        }
    }

    public void OnUIClick()
    {

    }
}
