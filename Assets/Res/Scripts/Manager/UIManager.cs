using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    /// <summary>
    /// 选取框
    /// </summary>
    public Image UISelectBox;
    /// <summary>
    /// 描边
    /// </summary>
    public Material edgeMat;
    /// <summary>
    /// UI从属字典
    /// </summary>
    public Dictionary<string, UIPanel> uiRefDic = new Dictionary<string, UIPanel>();

    public Dictionary<string, Transform> uiGroupDic = new Dictionary<string, Transform>();
    /// <summary>
    /// 当前选择的目标
    /// </summary>
    ISelectAble[] nowSelect;
    public bool uiBlock;

    public string nowTargetTipName;
    //Tip的显示
    public TipPanel topTipPanel;
    public Stack<TipPanel> tipPanels;
    public bool isWithin;

    float nowLockTime;
    float nowRemoveTime;
    const float MAX_LOCK_TIME = 1f;
    const float MAX_REMOVE_TIME = 0.2f;

    protected override void Init()
    {
        base.Init();
        GameManager.realTimeRelyMethods += Update;
    }

    public void RegisterUI(UIPanel panel)
    {
        uiRefDic[panel.uiPanelName] = panel;
    }

    public void RegisterUIGroup(Transform uiGroupTransform, string uiGroupName)
    {
        uiGroupDic[uiGroupName] = uiGroupTransform;
    }


    public void ShowTipPanel(string name, string context, Vector2 delta, bool isUp = true)
    {
        if (tipPanels == null) tipPanels = new Stack<TipPanel>();
        if (topTipPanel != null && topTipPanel.relatedName != name)
        {
            GameObjectPoolManager.Instance.Recycle(topTipPanel.gameObject, "Prefab/TipPanel");
            topTipPanel = null;
            nowLockTime = 0f;
            nowRemoveTime = 0f;
        }
        if (topTipPanel == null)
        {
            topTipPanel = GameObjectPoolManager.Instance.Spawn("Prefab/TipPanel").GetComponent<TipPanel>();
            topTipPanel.transform.parent = GetGroupTransform("Main");
            topTipPanel.transform.localScale = Vector3.one;
            topTipPanel.transform.localEulerAngles = Vector3.zero;
            topTipPanel.OnShow();
            topTipPanel.relatedName = name;
            topTipPanel.SetText(context);
            if (delta != default)
                topTipPanel.SetLocalPos(delta, isUp);
            tipPanels.Push(topTipPanel);
            isWithin = true;
        }
        else
        {
            if (topTipPanel.relatedName == name)
            {
                isWithin = true;
            }
            else
            {

            }
        }
    }

    public void NowWithinRelated(string name, Vector2 delta = default, bool isUp = true)
    {
        if (tipPanels == null) tipPanels = new Stack<TipPanel>();
        if (topTipPanel != null && topTipPanel.relatedName != name)
        {
            GameObjectPoolManager.Instance.Recycle(topTipPanel.gameObject, "Prefab/TipPanel");
            topTipPanel = null;
            nowLockTime = 0f;
            nowRemoveTime = 0f;
        }
        if (topTipPanel == null)
        {
            topTipPanel = GameObjectPoolManager.Instance.Spawn("Prefab/TipPanel").GetComponent<TipPanel>();
            topTipPanel.transform.parent = GetGroupTransform("Main");
            topTipPanel.transform.localScale = Vector3.one;
            topTipPanel.transform.localEulerAngles = Vector3.zero;
            topTipPanel.OnShow();
            topTipPanel.relatedName = name;
            TipText tipText = DataBaseManager.Instance.GetIdNameDataFromList<TipText>(name);
            if (tipText != null)
                topTipPanel.SetText(tipText.containText);
            if (delta != default)
                topTipPanel.SetLocalPos(delta, isUp);
            tipPanels.Push(topTipPanel);
            isWithin = true;
        }
        else
        {
            if (topTipPanel.relatedName == name)
            {
                isWithin = true;
            }
            else
            {

            }
        }
        //nowTargetTipName = name;
    }

    private void Update(float deltaTime)
    {
        if (topTipPanel != null)
        {
            //对Tip进行维护
            if (isWithin)
            {
                nowRemoveTime = 0;
                if (!topTipPanel.isLock)
                {
                    nowLockTime += deltaTime;
                    //topTipPanel.UpdateUnlock(nowLockTime / MAX_LOCK_TIME);
                    if (nowLockTime >= MAX_LOCK_TIME)
                    {
                        //topTipPanel.SetLockOn();
                    }
                }
            }
            else
            {
                nowLockTime = 0;
                if (!topTipPanel.isLock)
                {
                    topTipPanel.OnHide();
                    GameObjectPoolManager.Instance.Recycle(topTipPanel.gameObject, "Prefab/TipPanel");
                    if (tipPanels.Count > 0)
                        topTipPanel = tipPanels.Pop();
                    else
                        topTipPanel = null;
                    //直接关闭
                    nowRemoveTime = 0;
                }
                else
                {
                    nowRemoveTime += Time.unscaledDeltaTime;
                    if (nowRemoveTime > MAX_REMOVE_TIME)
                    {
                        topTipPanel.OnHide();
                        GameObjectPoolManager.Instance.Recycle(topTipPanel.gameObject, "Prefab/TipPanel");
                        if (tipPanels.Count > 0)
                            topTipPanel = tipPanels.Pop();
                        else
                            topTipPanel = null;
                        nowRemoveTime = 0f;
                    }
                }
                //倒计时0.1f不恢复within就关闭topTipPanel，并Stack推出下一个
            }
        }
        isWithin = false;
    }

    public Queue<(string, Action<UIPanel>, string)> queue = new Queue<(string, Action<UIPanel>, string)>();

    /// <summary>
    /// 链式显示UI 用于连续显示
    /// </summary>
    public UIPanel ShowChainUI(string uiName, Action<UIPanel> afterShow = null, string groupName = null)
    {
        queue.Enqueue((uiName, afterShow, groupName));
        if (queue.Count == 1)
        {
            return ShowUI(uiName, afterShow, groupName);
        }
        return null;
    }

    public void OnHide(UIPanel uiPanel)
    {
        if (queue.Count > 0)
        {
            if (queue.Peek().Item1 == uiPanel.uiPanelName) {
                queue.Dequeue();
                if (queue.Count > 0)
                {
                    var nextPanel = queue.Peek();
                    ShowUI(nextPanel.Item1, nextPanel.Item2, nextPanel.Item3);
                }
            }
        }
    }

    public UIPanel ShowUI(string uiName, Action<UIPanel> afterShow = null, string groupName = null)
    {
        var panel = GetUI(uiName);
        if (panel)
        {
            panel.OnShow();
            afterShow?.Invoke(panel);
            return panel;
        }
        else
        {
            var g = GameObject.Instantiate(Resources.Load<GameObject>($"Prefab/{uiName}"), GetGroupTransform(groupName));
            panel = g.GetComponent<UIPanel>();
            panel.OnInit();
            panel.OnShow();
            afterShow?.Invoke(panel);
            return panel;
            //尝试从Resource里进行加载
        }
    }

    Transform GetGroupTransform(string groupName)
    {
        if (!string.IsNullOrEmpty(groupName) && uiGroupDic.ContainsKey(groupName) && uiGroupDic[groupName] != null) return uiGroupDic[groupName];
        return uiGroupDic["Main"];
    }

    public void HideUI(string uiName)
    {
        GetUI(uiName)?.OnHide();
    }

    public UIPanel GetUI(string name)
    {
        if (uiRefDic.ContainsKey(name))
        {
            return uiRefDic[name];
        }
        return null;
    }


    public void ActiveSelectBox()
    {
    }

    public void ShowSelectBox(Vector2 startMousePoint, Vector2 endMousePoint)
    {
        startMousePoint /= GraphicUtil.SCREEN_SIZE_FIX;
        endMousePoint /= GraphicUtil.SCREEN_SIZE_FIX;

        if (UISelectBox == null)
        {
            UISelectBox = uiRefDic["UISelectBox"].GetComponent<Image>();
        }
        UISelectBox.enabled = true;
        UISelectBox.rectTransform.anchoredPosition = (startMousePoint + endMousePoint) / 2;
        UISelectBox.rectTransform.sizeDelta = new Vector2(Mathf.Abs(startMousePoint.x - endMousePoint.x), Mathf.Abs(startMousePoint.y - endMousePoint.y));
    }

    public void ShowSelectFlag()
    {


    }

    public void CloseSelectBox()
    {
        if (UISelectBox == null)
        {
            UISelectBox = uiRefDic["UISelectBox"].GetComponent<Image>();
        }
        UISelectBox.enabled = false;
    }

    public bool IsSelected(ISelectAble target)
    {
        if (nowSelect != null)
        {
            foreach (var oldselect in nowSelect)
            {
                if (oldselect == target)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void SetNowSelect(params ISelectAble[] newSelects)
    {
        if (nowSelect != null)
        {
            foreach (var oldselect in nowSelect)
            {
                if (oldselect != null)
                {
                    bool needSetFalse = true;
                    foreach (var newSelect in newSelects)
                    {
                        if (newSelect == oldselect)
                        {
                            needSetFalse = false;
                            break;
                        }
                    }
                    if (needSetFalse)
                    {
                        oldselect.OnSelect(false);
                    }
                }
            }
        }
        foreach (var newSelect in newSelects)
        {
            if (newSelect != null)
                newSelect.OnSelect(true);
        }
        nowSelect = newSelects;
    }
}

public interface ISelectAble
{
    void OnSelect(bool value);

    void OnUIClick();
}