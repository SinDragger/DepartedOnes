using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public static class TextureDrawer
{
    public static int TextureSize = 64;
    private static Color[] pixels;


    public static Texture DrawNoiseCircleTexture(float radius)
    {
        int rad = Mathf.FloorToInt(radius * TextureSize);
        int size = rad * 2;
        if (size % 2 != 1) size++;
        RenderTexture texture = RenderTexture.GetTemporary(size, size);

        RenderTexture.active = texture;
        Material processMaterial = DataBaseManager.Instance.ResourceLoad<Material>("Material/NoiseCircle");
        processMaterial.SetFloat("_R", rad);
        Vector4 offsetXY = new Vector4(Random.Range(0f, 1f), Random.Range(0f, 1f), rad, rad);
        processMaterial.SetVector("OffsetXY", offsetXY);
        Graphics.Blit(texture, processMaterial);
        RenderTexture.active = null;
        //RenderTexture.ReleaseTemporary(texture);

        return texture;
    }

    public static Texture EdgeDetection(Texture2D texture)
    { 
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height);
        RenderTexture.active = renderTexture;

        Material edgeDetectionMaterial = DataBaseManager.Instance.ResourceLoad<Material>("Material/EdgeDetect");

        Graphics.Blit(texture, renderTexture, edgeDetectionMaterial);

        //我目前也不知道为什么下面的这个不行上面的可以，可能是参数问题
        //edgeDetectionMaterial.SetTexture("MainTex", rue);
        //Graphics.Blit(renderTexture, edgeDetectionMaterial);

        RenderTexture.active = null;
        return renderTexture;
    }


}
