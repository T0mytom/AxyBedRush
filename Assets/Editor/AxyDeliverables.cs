using System.IO;
using UnityEditor;
using UnityEngine;

public static class AxyDeliverables
{
    private const string UnityPackagePath = "AxyTheBarbarian.unitypackage";

    private static readonly string[] ExportedFolders =
    {
        "Assets/Art",
        "Assets/Editor",
        "Assets/Prefabs",
        "Assets/Scenes",
        "Assets/Scripts"
    };

    private static readonly string[] ExportedFiles =
    {
        "Assets/Settings/UniversalRP.asset",
        "Assets/Settings/Renderer2D.asset",
        "Assets/Settings/DefaultVolumeProfile.asset",
        "Assets/Settings/UniversalRenderPipelineGlobalSettings.asset"
    };

    [MenuItem("Axy/Export UnityPackage")]
    public static void Export()
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string outputPath = Path.Combine(projectRoot, UnityPackagePath);

        var assets = new System.Collections.Generic.List<string>(ExportedFolders);
        foreach (string file in ExportedFiles)
        {
            if (File.Exists(file))
            {
                assets.Add(file);
            }
        }

#pragma warning disable CS0618
        AssetDatabase.ExportPackage(assets.ToArray(), outputPath, ExportPackageOptions.Recurse);
#pragma warning restore CS0618

        Debug.Log($"[AxyDeliverables] UnityPackage exportado en: {outputPath}");
    }

    [MenuItem("Axy/Build, Validate And Export")]
    public static void RunAll()
    {
        AxyProjectBuilder.BuildAll();
        AxyProjectValidator.Validate();
        Export();
    }
}
