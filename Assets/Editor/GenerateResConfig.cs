using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GenerateResConfig : Editor
{
    [MenuItem("Tools/Resources/Generate ResConfig File")]
    public static void Generate() {
        //生成资源查找配置文件
        //1.查找Resources目录下的所有预制体的完整文件
        string[] resFiles = AssetDatabase.FindAssets("t:prefab",new string[] { "Assets/Resources" });//获得是GUID
        //通过AssetDatabase.GUIDToAssetPath  将GUID转换为真实的路劲
        string fileName;
        string filePath;
        for (int i = 0; i < resFiles.Length; i++)
        {
        //2.生成对应关系
        // 名称=路径
            resFiles[i] = AssetDatabase.GUIDToAssetPath(resFiles[i]);
            fileName = Path.GetFileNameWithoutExtension(resFiles[i]);
            filePath = resFiles[i].Replace("Assets/Resources/", string.Empty).Replace(".prefab",string.Empty);
            resFiles[i] = fileName + "=" + filePath;
        }
        //3.写入文件
        
        File.WriteAllLines("Assets/StreamingAssets/Core_DeparterOnes/ConfigMap.txt", resFiles);

        AssetDatabase.Refresh();

    }



}
