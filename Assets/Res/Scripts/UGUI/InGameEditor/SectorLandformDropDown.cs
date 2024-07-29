using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SectorLandformDropDown : EventListener
{
    public Dropdown dropdown;
    List<Landform> list;
    SectorBlockData blockData;
    protected override string EventId => MapEventType.SECTOR_SELECT.ToString();
    protected override void OnTrigger(EventData data)
    {
        Color selectColor = data.GetValue<Color>("SectorColor");
        blockData = SectorBlockManager.Instance.map.GetTargetSectorBlockData(selectColor);
        dropdown.SetValueWithoutNotify(list.FindIndex(0, (l) => l.idName == blockData.landform));
    }

    private void Awake()
    {
        dropdown.options = new List<Dropdown.OptionData>();
        dropdown.onValueChanged.AddListener((index) =>
        {
            if (blockData != null)
            {
                blockData.landform = list[index].idName;
            }
        });
        list = DataBaseManager.Instance.GetTargetDataList<Landform>();
        foreach(var landform in list)
        {
            dropdown.options.Add(new Dropdown.OptionData(landform.name));
        }
    }
}
