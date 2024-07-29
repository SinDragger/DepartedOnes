using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberSwitch : MonoBehaviour
{
    public int max;
    public int min;
    public int now;
    public Text show;
    public Button upButton;
    public Button downButton;
    public Action onMaxClick;
    public Action<int> onValueChange;

    private void Awake()
    {
        now = 0;
        show.text = 0.ToString();
        upButton.SetBtnEvent(OnUpButtonClick);
        downButton.SetBtnEvent(OnDownButtonClick);
    }

    public void SetNumber(int value)
    {
        now = value;
        if (now > max)
        {
            now = max;
        }
        show.text = now.ToString();
    }

    public void SetMax(int value)
    {
        max = value;
        if (now > max) now = max;
        show.text = now.ToString();
    }

    public void OnUpButtonClick()
    {
        now++;
        if (now > max)
        {
            onMaxClick?.Invoke();
        }
        now = Mathf.Clamp(now, min, max);
        show.text = now.ToString();
        onValueChange?.Invoke(now);
    }

    public void OnDownButtonClick()
    {
        now--;
        now = Mathf.Clamp(now, min, max);
        show.text = now.ToString();
        onValueChange?.Invoke(now);
    }
}
