using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 数字反射显示的脚本
/// </summary>
public class GameNumberReflect : MonoBehaviour
{
    public Text numText;
    public int belong = -1;
    public string resName;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance)
            numText.text = GameManager.instance.playerForce.GetLimitedRes(resName).ToString();
    }
}
