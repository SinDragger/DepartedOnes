using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sector地图UI样式
/// </summary>
public class SectorConstructionUI : MonoBehaviour, ISelectAble
{
    public SectorConstruction sectorConstruction;
    public Image icon;
    public Transform occupyParent;
    public Vector2 mapPos;
    public void LoadConstructionImage(string idName)
    {
        icon.sprite = DataBaseManager.Instance.GetSpriteByIdName(idName);
        GetComponentInChildren<ForceFaceCamera>(true).Update();
    }
    public void ChangeSize(int size)
    {
        icon.rectTransform.sizeDelta = Vector2.one * size;
    }

    public void OnSelect(bool value = true)
    {
        if (value)
        {
            UIManager.Instance.ShowUI("MapConstructionUI");
            icon.material = GameManager.instance.edgeMat;
        }
        else
        {
            UIManager.Instance.HideUI("MapConstructionUI");
            icon.material = null;
        }
    }

    public void OnUIClick()
    {
        //判断是否点到了建筑物头上
        EventManager.Instance.DispatchEvent(new EventData(MapEventType.CONSTRUCTION_SELECT, "Building", this));
        if (sectorConstruction.stationedLegions != null && sectorConstruction.stationedLegions.Count > 0)
        {
            EventManager.Instance.DispatchEvent(new EventData(MapEventType.LEGION_SELECT, "Legion", sectorConstruction.stationedLegions[0]));
            UIManager.Instance.SetNowSelect(GetComponentInChildren<LegionMarkUI>(), this);
        }
        else
        {
            UIManager.Instance.SetNowSelect(this);
        }
    }

    private void Update()
    {
        if (CameraControl.Instance.cameraLevel < 3)
        {
            transform.localScale = Vector3.one;
        }
        else
        {
            transform.localScale = Vector3.one * 1.75f;
        }
    }

    public void OnHide()
    {
        gameObject.SetActive(false);
    }

    public void OnShow()
    {
        gameObject.SetActive(true);
    }

}
