using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 劳动力页面
/// </summary>
public class LabourUI : MonoBehaviour
{
    public Text labourUI;
    public Text workingPercent;

    private void Update()
    {
        labourUI.text = LabourWorkManager.Instance.labourPoint.ToString();
    }
}
