using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoulPointReflect : MonoBehaviour
{
    public Text numText;

    void Update()
    {
        if (GameManager.instance)
            numText.text = GameManager.instance.playerData.soulPoint.ToString();
    }
}
