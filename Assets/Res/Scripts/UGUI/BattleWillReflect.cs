using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleWillReflect : MonoBehaviour
{
    public Image fillImage_Left;
    public Image fillImage_Right;
    
    // Update is called once per frame
    void Update()
    {
        fillImage_Left.fillAmount = BattleManager.instance.enermyBattleWill;
        fillImage_Left.color = Color.Lerp(Color.red, Color.green, fillImage_Left.fillAmount);
        fillImage_Right.fillAmount = BattleManager.instance.enermyBattleWill;
        fillImage_Right.color = Color.Lerp(Color.red, Color.green, fillImage_Right.fillAmount);
    }
}
