using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DragAbleObject : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public bool dragAble;
    [HideInInspector]
    public Vector2 startDragPos;
    [HideInInspector]
    public Vector2 endDragPos;
    public UnityEvent onDragStart;
    public UnityEvent<Vector2> onDraging;
    public UnityEvent onDragEnd;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!dragAble) return;
        onDragStart?.Invoke();
        startDragPos = transform.localPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragAble) return;
        Vector3 pos = transform.localPosition;
        pos += (Vector3)eventData.delta / GraphicUtil.SCREEN_SIZE_FIX;
        transform.localPosition = pos;
        onDraging?.Invoke(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragAble) return;
        endDragPos = transform.localPosition;
        onDragEnd?.Invoke();
    }
}
