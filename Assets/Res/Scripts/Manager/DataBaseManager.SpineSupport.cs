using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class DataBaseManager : Singleton<DataBaseManager>
{

    public SkeletonDataAsset SkeletonDataLoad(string spineName,string modDirPath)
    {
        if (resTempStore.ContainsKey(spineName)) return resTempStore[spineName] as SkeletonDataAsset;
        resTempStore[spineName] = GetSkeletonDataAssetByPath(GetDirPath(spineName,modDirPath));
        return resTempStore[spineName] as SkeletonDataAsset;
    }

    string GetDirPath(string spineName, string modDirPath)
    {
        return $"{modDirPath}/Spine/{spineName}/";

    }

    SkeletonDataAsset GetSkeletonDataAssetByPath(string dirPath)
    {
        string skeletonTextPath = "";
        string skeletonAtlasPath = "";
        string skeletonTexturePath = "";
        DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
        if (!directoryInfo.Exists)
        {
            return null;
        }
        else
        {
            var result = directoryInfo.GetFiles();
            foreach (var file in result)
            {
#if UNITY_EDITOR
                if (file.Name.EndsWith("meta")) continue;
#endif
                if (file.Name.Contains(".atlas"))
                {
                    skeletonAtlasPath = file.Name;
                }
                if (file.Name.Contains(".png"))
                {
                    skeletonTexturePath = file.Name;
                }
                if (file.Name.Contains(".skel") || file.Name.Contains(".json"))
                {
                    skeletonTextPath = file.Name;
                }
            }
        }
        var bytes = File.ReadAllBytes(dirPath + skeletonTextPath);
        TextAsset skeletonText = new TextAsset(System.Text.Encoding.UTF8.GetString(bytes));
        skeletonText.name = skeletonTextPath;
        TextAsset atlasText = new TextAsset(File.ReadAllText(dirPath + skeletonAtlasPath));
        atlasText.name = skeletonAtlasPath;
        Texture2D[] textures = new Texture2D[1];
        textures[0] = LoadTextureByPath(dirPath + skeletonTexturePath);
        Material instanceCreate = new Material(Shader.Find("Spine/Skeleton Tint"));
        instanceCreate.mainTexture = textures[0];
        instanceCreate.name = ExtractMatName(atlasText);
        textures[0].name = instanceCreate.name;
        SpineAtlasAsset runtimeAtlasAsset = SpineAtlasAsset.CreateRuntimeInstance(atlasText, textures, instanceCreate, true);
        SkeletonDataAsset runtimeSkeletonDataAsset = SkeletonDataAsset.CreateRuntimeInstance(skeletonText, new[] { runtimeAtlasAsset }, true, coverBytes: bytes);
        return runtimeSkeletonDataAsset;
    }

    string ExtractMatName(TextAsset atlasText)
    {
        string atlasString = atlasText.text;
        atlasString = atlasString.Replace("\r", "");
        string[] atlasLines = atlasString.Split('\n');
        return atlasLines[0].Trim().Replace(".png", "");

        var pages = new List<string>();
        for (int i = 0; i < atlasLines.Length - 1; i++)
        {
            if (atlasLines[i].Trim().Length == 0)
                return atlasLines[i + 1].Trim().Replace(".png", "");
        }
        return "";
    }
}
