using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorUtil
{
    public static Color sectorDefaultColor = new Color(172f / 255f, 236f / 255f, 255f / 255f, 168f / 255f);
    public static Color sectorSelectColor = new Color(253f / 255f, 99f / 255f, 128f / 255f, 168f / 255f);

    public static Color selfDetectColor = new Color(209f / 255f, 255f / 255f, 153 / 255f, 71f / 255f);
    public static Color enermyDetectColor = new Color(255f / 255f, 153f / 255f, 155f / 255f, 71f / 255f);

    public static Color test_TreeColor = new Color(172f / 255f, 255f / 255f, 128f / 255f, 192f / 255f);
    public static Color test_PlaneColor = new Color(250f / 255f, 255f / 255f, 128f / 255f, 172f / 255f);

    public static Color guideColorGreen = new Color(93f / 255f, 200f / 255f, 110f / 255f);
    public static Color guideColorRed = new Color(200f / 255f, 54f / 255f, 59f / 255f);

    public static Color ColorTrans(string value)
    {
        ColorUtility.TryParseHtmlString("#" + value, out Color result);
        return result;
    }
    public static string GetColorTextTip(Color color,bool isHead)
    {
        if (!isHead) return "</color>";
        return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>";
    }
    public static string GetColorTextStart(string HtmlString)
    {
        return $"<color=#{HtmlString}>";
    }
    public static string GetColorTextTipEnd()
    {
        return "</color>";
    }
}
