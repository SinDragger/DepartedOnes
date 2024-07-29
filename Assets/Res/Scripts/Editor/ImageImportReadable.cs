using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ImageImportReadable : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        return;
        TextureImporter importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.isReadable = true;
    }
}