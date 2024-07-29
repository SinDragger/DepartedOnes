using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LegionMarkUI : MonoBehaviour, ISelectAble
{
    /// <summary>
    /// TODO 之后更改成可编辑形式
    /// </summary>
    public Image[] legionFlags;
    public GameObject[] leaderAreas;
    public Image[] leaderMarks;
    public RectTransform mark;
    public GameObject subBar;
    public Text nameText;
    public Text numText;
    public Image fillImage;
    public Image stateImage;
    public Image repeatImage;
    public Image chooseIcon;

    public Image[] reminds;

    public LifeBarControl lifeBarControl;
    /// <summary>
    /// 指示线
    /// TODO:迁移State至此
    /// </summary>
    [HideInInspector]
    public LineRoute interacterLine;

    /// <summary>
    /// 当前的即时控制
    /// </summary>
    [HideInInspector]
    public LegionControl legion;
    /// <summary>
    /// 标志所下属的军团
    /// </summary>
    List<LegionControl> legions = new List<LegionControl>();

    bool isHide;
    //基础化简略
    public void Init(LegionControl control)
    {
        this.legion = control;
        for (int i = 0; i < legionFlags.Length; i++)
        {
            legionFlags[i].gameObject.SetActive(i == 0);
        }
        for (int i = 0; i < leaderAreas.Length; i++)
        {
            leaderAreas[i].gameObject.SetActive(false);
        }
        if (legion.mainGeneral != null)
        {
            leaderAreas[0].gameObject.SetActive(true);
            leaderMarks[0].sprite = legion.mainGeneral.GetHeadIcon();
        }
        fillImage.fillAmount = 1f;
        nameText.text = legion.name;
        Sprite sprite = LegionManager.Instance.GetTargetBelongMark(control.belong);
        foreach (var image in legionFlags)
        {
            image.sprite = sprite;
        }
        if (!control.isExposed)
        {
            OnHide();
        }
        //依据信息进行内容初始化
        GetComponentInChildren<ForceFaceCamera>(true).Update();
        var block = SectorBlockManager.Instance.GetBlock(legion.position);
        if (block == null || block.hideLevel > 0)
        {
            OnHide();
        }
    }
    public void OnSelect(bool value = true)
    {
        if (gameObject == null) return;
        chooseIcon.gameObject.SetActive(value);
        //影响外部的点击
        if (value)
        {
            UIManager.Instance.ShowUI("LegionTopDetailUI");
        }
        else
        {
            UIManager.Instance.HideUI("LegionTopDetailUI");
        }
    }

    public void SetPosition(Vector2 pos)
    {
        transform.localPosition = new Vector3(pos.x, pos.y, 0f);
    }

    string lastStateName;
    private void Update()
    {
        if (legion == null) return;
        numText.text = legion.TotalNum.ToString();
        //需要镜头绑定
        if (CameraControl.Instance.cameraLevel < 3 || legion.State is LegionOccupyState)
        {
            mark.localScale = Vector3.one * 0.02f;
        }
        else
        {
            mark.localScale = Vector3.one * 0.035f;
        }
        string iconName = legion.State.GetLegionStateIcon();
        if (iconName != lastStateName)
        {
            lastStateName = iconName;
            stateImage.sprite = DataBaseManager.Instance.GetSpriteByIdName(lastStateName);
        }
        if (legion.State is LegionCollectState)
        {
            repeatImage.fillAmount = (legion.State as LegionCollectState).nowProgressPercent();
        }
        else
        {
            repeatImage.fillAmount = 0f;
        }
        //TODO:增加屏蔽
        lifeBarControl.Init(legion.TotalNum);
        fillImage.fillAmount = 1f;
    }

    public void SetRemind(int flag, float size, Color color = default)
    {
        if (flag >= reminds.Length)
        {
            Debug.LogError("OutOfLength");
            return;
        }
        if (color != default)
        {
            reminds[flag].color = color;
        }
        reminds[flag].rectTransform.sizeDelta = new Vector2(size, size);
    }

    public void OnHide()
    {
        if (isHide) return;
        isHide = true;
        //踪迹隐匿
        mark.gameObject.SetActive(false);
        for (int i = 0; i < reminds.Length; i++)
        {
            reminds[i].gameObject.SetActive(false);
        }
    }

    public void OnShow()
    {
        if (!isHide) return;
        isHide = false;
        mark.gameObject.SetActive(true);
        if (!(legion.State is LegionOccupyState))
        {
            for (int i = 0; i < reminds.Length; i++)
            {
                reminds[i].gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 进入占据状态-劳作部队出现工作图标
    /// </summary>
    public void IntoOccupyMode(SectorConstructionUI ui)
    {
        transform.parent = ui.occupyParent;
        transform.localScale = Vector3.one * 0.7f;
        for (int i = 0; i < reminds.Length; i++)
        {
            reminds[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 进入战争状态
    /// </summary>
    public void IntoBattleMode()
    {
        for (int i = 0; i < reminds.Length; i++)
        {
            reminds[i].gameObject.SetActive(false);
        }
    }

    public void LeaveBattleMode()
    {
        for (int i = 0; i < reminds.Length; i++)
        {
            reminds[i].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 离开占据模式
    /// </summary>
    public void LeaveOccupyMode()
    {
        transform.parent = LegionParentMark.instance.transform;
        transform.localScale = Vector3.one;
        //mark.anchoredPosition = new Vector2(0, 0);
        transform.SetAsLastSibling();
        subBar.gameObject.SetActive(true);
        for (int i = 0; i < reminds.Length; i++)
        {
            reminds[i].gameObject.SetActive(true);
            reminds[i].transform.localScale = Vector3.one;
        }
    }
    public void OnUIClick()
    {
        LegionManager.Instance.ChooseTargetLegion(legion);
        EventManager.Instance.DispatchEvent(new EventData(MapEventType.LEGION_SELECT, "Legion", legion));
        if (legion.State is LegionOccupyState)
        {
            var ui = ConstructionManager.Instance.GetMapUI((legion.State as LegionOccupyState).sectorConstruction);
            EventManager.Instance.DispatchEvent(new EventData(MapEventType.CONSTRUCTION_SELECT, "Building", ui));
            UIManager.Instance.SetNowSelect(this, ui);
        }
        else
        {
            UIManager.Instance.SetNowSelect(this);
        }
    }
}
