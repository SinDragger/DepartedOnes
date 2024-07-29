using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceShowSlot : MonoBehaviour
{
    public Image icon;
    public Text textName;
    public Text textNum;

    public void Init(string resName, int num)
    {
        if (icon != null)
            icon.sprite = DataBaseManager.Instance.GetSpriteByIdName(resName);
        if (textName != null)
            textName.text = DataBaseManager.Instance.GetTargetDataList<Resource>().Find((item) => item.idName.Equals(resName)).name;
        if (textNum != null)
            textNum.text = num.ToString();
    }
}
