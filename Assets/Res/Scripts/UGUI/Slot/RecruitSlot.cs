using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecruitSlot : MonoBehaviour
{
    public Button clickButton;
    public TextUnit costText;
    public TextUnit bodyCostText;
    public LegionTroopSlot slot;
    public Dictionary<string, int> totalCost;

    public void Init(UnitData unitData)
    {
        slot.Init(unitData);
        totalCost = unitData.GetTroopTotalCost();
        costText.SpriteClear();
        int flag = 0;
        string textString = "";
        foreach (var entity in totalCost)
        {
            costText.SetSprite(flag, DataBaseManager.Instance.GetSpriteByIdName(DataBaseManager.Instance.GetIdNameDataFromList<Resource>(entity.Key).idName));
            textString += $"{"<quad>"}{entity.Value} ";
            flag++;
        }
        costText.SetText(textString);
        totalCost.Clear();
        if (unitData.resContain != null)
        {
            foreach (var pair in unitData.resContain)
            {
                totalCost.Add(pair.idName, pair.num * unitData.soldierNum);
            }
        }
        bodyCostText.SpriteClear();
        flag = 0;
        textString = "";
        foreach (var entity in totalCost)
        {
            bodyCostText.SetSprite(flag, DataBaseManager.Instance.GetSpriteByIdName(entity.Key));
            textString += $"{"<quad>"}{entity.Value} ";
            flag++;
        }
        bodyCostText.SetText(textString);
    }
}
