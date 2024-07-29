using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TestMapClick : MonoBehaviour
{
    public Image targetColor;
    public void OnMapClick()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        foreach (var result in raycastResults)
        {
            if (result.gameObject.Equals(gameObject))
            {
                Debug.LogError(result.worldPosition);
                var c = SectorBlockManager.Instance.GetRegColor(result.worldPosition);
                targetColor.color = c;
                TriggerClick(c);
            }
        }
    }

    void TriggerClick(Color index)
    {
        SectorBlockManager.Instance.TargetBlockOnSelect(index);
    }
}
