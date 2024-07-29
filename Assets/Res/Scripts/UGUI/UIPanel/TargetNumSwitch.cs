using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetNumSwitch : MonoBehaviour
{
    public int max;
    public int min;
    public int now;
    public Text show;
    public Button upButton;
    public Button downButton;

    public string targetString;

    private void Awake()
    {
        now = PlayerPrefs.GetInt(targetString);
        show.text = now.ToString();
        upButton.SetBtnEvent(OnUpButtonClick);
        downButton.SetBtnEvent(OnDownButtonClick);
    }
    public void OnUpButtonClick()
    {
        now++;
        now = Mathf.Clamp(now, min, max);
        PlayerPrefs.SetInt(targetString, now);
        show.text = now.ToString();
    }

    public void OnDownButtonClick()
    {
        now--;
        now = Mathf.Clamp(now, min, max);
        PlayerPrefs.SetInt(targetString, now);
        show.text = now.ToString();
    }
}
