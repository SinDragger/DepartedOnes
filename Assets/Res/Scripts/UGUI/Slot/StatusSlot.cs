using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusSlot : MonoBehaviour
{
    public Text text;
    StandardStatus status;
    public StatusPanelTip tip;
    public void Init(string statusId)
    {
        status = DataBaseManager.Instance.GetIdNameDataFromList<StandardStatus>(statusId);
        text.text = status.name;
    }

    public void Init(StandardStatus standardStatus)
    {
        status = standardStatus;
        text.text = status.name;
        if (tip)
        {
            tip.status = standardStatus;
        }
    }
}
