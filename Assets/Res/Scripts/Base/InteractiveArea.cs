using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 交互区域
/// </summary>
public class InteractiveArea : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler, IPointerClickHandler
{
    public UnityEvent onClick;
    public UnityEvent onPress;
    public UnityEvent<Vector2> onDrag;

    public UnityEvent rightClick;
    public void TriggerClick()
    {
        onClick?.Invoke();
    }

    public void TriggerOnDrag(Vector2 delta)
    {
        onDrag?.Invoke(delta);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        InputManager.Instance.mouseInteractiveArea = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (InputManager.Instance.mouseInteractiveArea == this)
            InputManager.Instance.mouseInteractiveArea = null;
    }
    private void OnDisable()
    {
        if (InputManager.Instance.mouseInteractiveArea == this)
            InputManager.Instance.mouseInteractiveArea = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            rightClick.Invoke();
    }
}