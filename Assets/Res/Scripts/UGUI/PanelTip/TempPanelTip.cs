using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 临时页面Panel:依据预设的TipXmlString内容分布进行内容显示与扩容
/// TODO:继承
/// </summary>
public class TempPanelTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Vector2 deltaPos;
    /// <summary>
    /// 标记目标
    /// </summary>
    public string tipTarget;
    /// <summary>
    /// 是否悬停
    /// </summary>
    bool isHover;
    public bool forceLocalPos;

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHover = true;
        //向UIManager赋予相关的位置与大小信息
        Vector2 transformPos = RectTransformUtility.WorldToScreenPoint(CameraControl.Instance.mainCamera, transform.position);
        Vector2 pos = default;
        if (forceLocalPos)
        {
            pos = (transformPos - new Vector2(Screen.width / 2, Screen.height / 2) + deltaPos) / GraphicUtil.SCREEN_SIZE_FIX;
        }
        else
        {
            pos = (new Vector2(Input.mousePosition.x, transformPos.y) - new Vector2(Screen.width / 2, Screen.height / 2) + deltaPos) / GraphicUtil.SCREEN_SIZE_FIX;
        }
        ShowTip(pos);
    }

    protected virtual void ShowTip(Vector2 pos)
    {
        UIManager.Instance.NowWithinRelated(tipTarget, pos, deltaPos.y >= 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHover = false;
    }

    private void Update()
    {
        if (isHover)
        {
            UIManager.Instance.NowWithinRelated(tipTarget);
        }
    }
}
