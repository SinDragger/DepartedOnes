using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipAdjustEditorPanel : MonoBehaviour
{
    public InputField posX;
    public InputField posY;
    public InputField rot;

    EquipData targetEquip;
    private bool hasInit;
    public void Init(EquipData targetEquipData)
    {
        targetEquip = targetEquipData;
        hasInit = false;
        posX.text = targetEquipData.posDelta.x.ToString();
        posY.text = targetEquipData.posDelta.y.ToString();
        rot.text = targetEquipData.rotDelta.ToString();
        hasInit = true;
        //if (!hasInit)
        //{
        //    posX.onValueChanged = OnValueChange;
        //}
    }

    public void OnValueChange(string value)
    {
        if (!hasInit) return;
        if (!float.TryParse(posX.text, out float x)) return;
        if (!float.TryParse(posY.text, out float y)) return;
        if (!float.TryParse(rot.text, out float rotation)) return;
        targetEquip.posDelta = new Vector2(x, y);
        targetEquip.rotDelta = rotation;
        TestCode.instance.ModelUpdate();
        DataBaseManager.Instance.SaveXMLByType<EquipData>("EquipData.Tex");
        //触发模型的刷新
    }

}
