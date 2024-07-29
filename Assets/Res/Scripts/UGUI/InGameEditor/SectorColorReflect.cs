using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SectorColorReflect : EventListener
{
    public Image image;
    protected override string EventId => MapEventType.SECTOR_SELECT.ToString();
    protected override void OnTrigger(EventData data)
    {
        Color selectColor = data.GetValue<Color>("SectorColor");
        image.color = selectColor;
    }
}
