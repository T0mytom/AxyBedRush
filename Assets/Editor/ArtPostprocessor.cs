using UnityEditor;
using UnityEngine;

public class ArtPostprocessor : AssetPostprocessor
{
    private const float PixelsPerUnit = 32f;

    private void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith("Assets/Art/") || !assetPath.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        TextureImporter importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = PixelsPerUnit;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.filterMode = FilterMode.Point;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
    }
}
