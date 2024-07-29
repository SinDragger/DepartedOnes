using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapConsoleSwitch : MonoBehaviour
{
    public Button button;
    public Transform controlTransform;

    // Start is called before the first frame update
    void Start()
    {
        button.SetBtnEvent(() =>
        {
            controlTransform.gameObject.SetActive(!controlTransform.gameObject.activeSelf);
        });
    }
}
