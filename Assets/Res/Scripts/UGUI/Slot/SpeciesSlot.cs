using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeciesSlot : MonoBehaviour
{
    public RectTransform content;
    public LegionTroopSlot slot;
    public GameObject selectBorder;
    public void Init(object param)
    {
        if (param is Species)
        {
            Species species = param as Species;
            slot.Init(DataBaseManager.Instance.GetSpeciesBaseUnit(species.idName), species.idName);
            content.anchoredPosition = new Vector2(-10, 0);
        }
        else if (param is SubSpecies)
        {
            SubSpecies subSpecies = param as SubSpecies;
            slot.Init(DataBaseManager.Instance.GetSpeciesBaseUnit(subSpecies.species), subSpecies.species,subSpecies.idName);
            content.anchoredPosition = new Vector2(10, 0);
        }
    }

    public void SetOnClick(System.Action onClick)
    {
        GetComponent<Button>().SetBtnEvent(()=>onClick?.Invoke());
    }

    public void OnSelected(bool value)
    {
        selectBorder.SetActive(value);
    }
}
