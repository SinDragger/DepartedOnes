using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SectorControlBar : MonoBehaviour
{
    //填充量
    public Image fillImage;
    //填充上限
    public Image fillMaxImage;
    //阻隔量
    public Image blockImage;

    public void SetBarProgress(float percent)
    {
        fillImage.fillAmount = percent;
    }

    public void SetBarProgressMax(float percent)
    {
        fillMaxImage.fillAmount = percent;
    }

    public void SetBarColor(Color color)
    {
        fillImage.color = color;
        fillMaxImage.color = Color.Lerp(color, Color.gray, 0.8f);
    }

    public void SetBlock(float percent)
    {
        blockImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (transform as RectTransform).sizeDelta.x * percent);
    }
}
