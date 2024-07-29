using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitAttributeArea : MonoBehaviour
{
    public Text maxLifeText;
    public Text destructivePowerText;
    public Text hitRateText;
    public Text defendLevelText;
    public Text moveSpeedText;
    public Text burdenText;

    public void Init(UnitData unit)
    {
        maxLifeText.text = unit.maxLife.ToString();
        destructivePowerText.text = unit.destructivePower.ToString();
        hitRateText.text = unit.hitRate.ToString();
        defendLevelText.text = unit.defendLevel.ToString();
        moveSpeedText.text = unit.speed.ToString();
        burdenText.text = unit.maxLife.ToString();
    }

    public void Init(TroopEntity troop)
    {
        maxLifeText.text = troop.maxLife.ToString();
        destructivePowerText.text = troop.destructivePower.ToString();
        hitRateText.text = troop.hitRate.ToString();
        defendLevelText.text = troop.defendLevel.ToString();
        moveSpeedText.text = troop.speed.ToString();
        burdenText.text = troop.weightBearing.ToString();
    }
}
