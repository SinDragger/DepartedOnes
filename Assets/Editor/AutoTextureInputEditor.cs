using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AutoTextureInputEditor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        TextureImporter importer = (TextureImporter)assetImporter;
        //importer.textureType = TextureImporterType.Sprite;
        importer.isReadable = true;
    }
}
