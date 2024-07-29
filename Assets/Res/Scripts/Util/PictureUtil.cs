using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class PictureUtil
{
    public static void ShowTexture2DAlpha(Texture2D texture)
    {
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
            }
        }
    }
    public static void ChangeTextureColor(Texture2D source, Texture2D bitmap, Color adjustColor)
    {
        if (source.width != bitmap.width) return;
        if (source.height != bitmap.height) return;
        for (int i = 0; i < source.width; i++)
        {
            for (int j = 0; j < source.height; j++)
            {
                if (bitmap.GetPixel(i, j).r > 0.5f)
                {
                    source.SetPixel(i, j, ColorChange(source.GetPixel(i, j), adjustColor));
                }
            }
        }
        source.Apply();
    }

    public static Color ColorChange(Color source, Color mix)
    {
        Color.RGBToHSV(source, out float H, out float S, out float V);
        Color.RGBToHSV(mix, out float mixH, out float mixS, out float mixV);
        return Color.HSVToRGB(mixH, Mathf.Lerp(mixS, S, mix.a), V);// (V + mixV) / 2);

    }

    public static Sprite ToSprite(this Texture2D origin)
    {
        Sprite sprite = Sprite.Create(origin, new Rect(0, 0, origin.width, origin.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }

    public static Sprite CopyTexture(Texture2D origin)
    {
        Color[] srcPixels = origin.GetPixels();
        Texture2D copyTexture = new Texture2D(origin.width, origin.height, origin.format, false);
        copyTexture.SetPixels(srcPixels);
        copyTexture.Apply();
        Sprite sprite = Sprite.Create(copyTexture, new Rect(0, 0, origin.width, origin.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }
    public static int CompareTo(this Color a,Color b)
    {
        if (a.r > b.r) return 1;
        else if (a.r < b.r) return -1;
        if (a.g > b.g) return 1;
        else if (a.g < b.g) return -1;
        if (a.b > b.b) return 1;
        else if (a.b < b.b) return -1;
        return 0;
    }
}
