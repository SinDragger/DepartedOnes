using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel : MonoBehaviour
{
    public virtual string uiPanelName => "";

    protected virtual void Awake()
    {
        UIManager.Instance.RegisterUI(this);
        OnInit();
    }
    /// <summary>
    /// 界面创建初始化
    /// </summary>
    public virtual void OnInit()
    {

    }
    /// <summary>
    /// 界面显示
    /// </summary>
    public virtual void OnShow(bool withAnim = true)
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 界面隐藏
    /// </summary>
    public virtual void OnHide(bool withAnim = true)
    {
        gameObject.SetActive(false);
        UIManager.Instance.OnHide(this);
    }

    /// <summary>
    /// 相关资源释放
    /// </summary>
    public virtual void OnRelease()
    {

    }

}
