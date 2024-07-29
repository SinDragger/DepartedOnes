using Sirenix.OdinInspector;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "NoiseGenerator", menuName = "ScriptableObject/Noise")]
public class NoiseGenerator : ScriptableObject
{
    public string saveToDiskPath = "Assets/Res/Graphic/Texture/PerlinNoise";
    public int resolution = 256;
    public int resolutionZ_3D = 1;
    [Range(1, 32)] public float frequency = 4;
    public bool is3D = false;
    public bool isTilable = false;
    [Range(1, 100)] public float randomSeed = 1;
    public bool autoReseed;
    public Vector3 evolution = Vector3.zero;
#if UNITY_EDITOR
    public TextureImporterCompression compression = TextureImporterCompression.Uncompressed;
#endif

    [Space]
    [Range(0, 8)] public int fbmIteration = 0;

    [Space]
    public bool remapTo01;
    public bool invert;
    public bool changeContrast;
    [Range(0, 5)] public float contrast = 1;

    [Space]
    public ComputeShader cs_core;
    public ComputeShader cs_postProcess;

    protected ComputeBuffer tempComputeBuffer;
    [Header("TempPreview")]
    public RenderTexture previewTexture2D;
    public RenderTexture previewTexture3D;

    private void OnDisable()
    {
        ReleaseTempResources();
    }

    [Button]
    public RenderTexture Generate()
    {
        ReleaseTempResources();

        if (autoReseed)
        {
            randomSeed = Random.Range(1f, 100f);
        }

        int resolutionZ = 1;
        if (is3D)
        {
            resolutionZ = resolutionZ_3D;
        }

        tempComputeBuffer = new ComputeBuffer(resolution * resolution * resolutionZ, 16);

        if (is3D)
        {
            previewTexture2D = new RenderTexture(4, 4, 0, RenderTextureFormat.R8);
            previewTexture2D.enableRandomWrite = true;

            previewTexture3D = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.R8);
            previewTexture3D.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
            previewTexture3D.volumeDepth = resolution;
            previewTexture3D.wrapMode = TextureWrapMode.Repeat;
            previewTexture3D.filterMode = FilterMode.Bilinear;
            previewTexture3D.enableRandomWrite = true;
        }
        else
        {
            previewTexture2D = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.R8);
            previewTexture2D.wrapMode = TextureWrapMode.Repeat;
            previewTexture2D.filterMode = FilterMode.Bilinear;
            previewTexture2D.enableRandomWrite = true;

            previewTexture3D = new RenderTexture(4, 4, 0, RenderTextureFormat.R8);
            previewTexture3D.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
            previewTexture3D.enableRandomWrite = true;
        }

        int kernel = cs_core.FindKernel("Main");
        cs_core.SetBuffer(kernel, "_Colors", tempComputeBuffer);
        cs_core.SetTexture(kernel, "_Texture2D", previewTexture2D);
        cs_core.SetTexture(kernel, "_Texture3D", previewTexture3D);

        cs_core.SetInt("_Resolution", resolution);
        cs_core.SetFloat("_Frequency", frequency);
        cs_core.SetBool("_Is3D", is3D);
        cs_core.SetBool("_IsTilable", isTilable);
        cs_core.SetFloat("_RandomSeed", randomSeed);
        cs_core.SetVector("_Evolution", evolution);
        cs_core.SetInt("_FBMIteration", fbmIteration);

        int dispatchX = Mathf.CeilToInt(resolution / 16f);
        int dispatchY = Mathf.CeilToInt(resolution / 16f);

        cs_core.Dispatch(kernel, dispatchX, dispatchY, resolutionZ);

        if (ShouldPostProcess())
        {
            cs_postProcess.SetBuffer(kernel, "_Colors", tempComputeBuffer);
            cs_postProcess.SetTexture(kernel, "_Texture2D", previewTexture2D);
            cs_postProcess.SetTexture(kernel, "_Texture3D", previewTexture3D);

            cs_postProcess.SetInt("_Resolution", resolution);
            cs_postProcess.SetBool("_Is3D", is3D);
            cs_postProcess.SetBool("_RemapTo01", remapTo01);
            cs_postProcess.SetBool("_Invert", invert);
            cs_postProcess.SetBool("_ChangeContrast", changeContrast);

            if (remapTo01)
            {
                Color[] colors = new Color[tempComputeBuffer.count];
                tempComputeBuffer.GetData(colors);
                float min = float.PositiveInfinity;
                float max = float.NegativeInfinity;
                for (int i = 0; i < colors.Length; i++)
                {
                    min = Mathf.Min(min, colors[i].r);
                    max = Mathf.Max(max, colors[i].r);
                }

                cs_postProcess.SetFloat("_MinValue", min);
                cs_postProcess.SetFloat("_MaxValue", max);
            }

            if (changeContrast)
            {
                cs_postProcess.SetFloat("_Contrast", contrast);
            }

            cs_postProcess.Dispatch(kernel, dispatchX, dispatchY, resolutionZ);
        }

        if (is3D)
            return previewTexture3D;
        else
            return previewTexture2D;
    }

    [Button]
    public void SaveToDisk()
    {
#if UNITY_EDITOR
        if (is3D)
        {
            Texture3D texture = new Texture3D(resolution, resolution, resolutionZ_3D, TextureFormat.R8, false);
            texture.wrapMode = TextureWrapMode.Repeat;
            texture.filterMode = FilterMode.Bilinear;

            Color[] colors = new Color[tempComputeBuffer.count];
            tempComputeBuffer.GetData(colors);
            texture.SetPixels(colors);
            texture.Apply();

            string path = saveToDiskPath + ".asset";

            AssetDatabase.DeleteAsset(path);
            AssetDatabase.Refresh();

            AssetDatabase.CreateAsset(texture, path);
            AssetDatabase.Refresh();

        }
        else
        {
            Texture2D texture = new Texture2D(resolution, resolution, TextureFormat.R8, false);
            texture.wrapMode = TextureWrapMode.Repeat;
            texture.filterMode = FilterMode.Bilinear;

            Color[] colors = new Color[tempComputeBuffer.count];
            tempComputeBuffer.GetData(colors);
            texture.SetPixels(colors);
            texture.Apply();


            string path = saveToDiskPath + ".png";

            File.WriteAllBytes(path, texture.EncodeToPNG());
            AssetDatabase.Refresh();

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            TextureImporterSettings settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.singleChannelComponent = TextureImporterSingleChannelComponent.Alpha;
            settings.alphaSource = TextureImporterAlphaSource.FromGrayScale;
            importer.SetTextureSettings(settings);
            importer.textureType = TextureImporterType.SingleChannel;
            importer.mipmapEnabled = false;
            importer.textureCompression = compression;
            importer.SaveAndReimport();
        }
#endif
    }

    public Texture GetNoiseTexture()
    {
        if (is3D)
        {
            Texture3D texture = new Texture3D(resolution, resolution, resolutionZ_3D, TextureFormat.R8, false);
            texture.wrapMode = TextureWrapMode.Repeat;
            texture.filterMode = FilterMode.Bilinear;

            Color[] colors = new Color[tempComputeBuffer.count];
            tempComputeBuffer.GetData(colors);
            texture.SetPixels(colors);
            texture.Apply();
            return texture;
        }
        else
        {
            Texture2D texture = new Texture2D(resolution, resolution, TextureFormat.R8, false);
            texture.wrapMode = TextureWrapMode.Repeat;
            texture.filterMode = FilterMode.Bilinear;

            Color[] colors = new Color[tempComputeBuffer.count];
            tempComputeBuffer.GetData(colors);
            texture.SetPixels(colors);
            texture.Apply();
            return texture;
        }
    }

    protected void ReleaseTempResources()
    {
        if (previewTexture2D != null)
            previewTexture2D.Release();

        if (previewTexture3D != null)
            previewTexture3D.Release();

        if (tempComputeBuffer != null)
            tempComputeBuffer.Release();
    }

    protected bool ShouldPostProcess()
    {
        if (remapTo01)
        {
            return true;
        }

        if (invert)
        {
            return true;
        }

        if (changeContrast)
        {
            return true;
        }

        return false;
    }
}
