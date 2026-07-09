using UnityEngine;
using UnityEditor;

// Automatically convert any texture file with "_bumpmap"
// in its file name into a normal map.

class MyTexturePostprocessor : AssetPostprocessor
{
//    void OnPreprocessTexture()
//    {
////        if (assetPath.Contains("_bumpmap"))
////        {
////            TextureImporter textureImporter  = (TextureImporter)assetImporter;
////            textureImporter.convertToNormalmap = true;
////        }
//        
//        var textureImporter = assetImporter as TextureImporter;
//
//        textureImporter.spriteImportMode = SpriteImportMode.Single;
//        textureImporter.textureType = TextureImporterType.Sprite;
//        textureImporter.alphaIsTransparency = true;
//        textureImporter.mipmapEnabled = false;
//    }
}