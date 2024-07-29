using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DragReceiveArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isFocus;
    public void OnPointerEnter(PointerEventData eventData)
    {
        isFocus = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isFocus = false;
    }
}
