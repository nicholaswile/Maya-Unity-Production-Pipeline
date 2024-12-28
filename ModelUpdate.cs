using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

public class ModelUpdate : AssetPostprocessor
{
    private static string notificationPath = "Assets/Models/update_notification.txt";

    // Called before the model is imported
    void OnPreprocessModel()
    {
        if (File.Exists(notificationPath))
        {
            // Asset path is a field of AssetPostprocessor which is the name of the asset being imported.
            string modelPath = assetPath;

            // If that asset exists already, then we make a backup copy if it.
            if (File.Exists(modelPath))
            {

                // Rename the asset. 
                string directory = Path.GetDirectoryName(modelPath);
                string fileName = Path.GetFileNameWithoutExtension(modelPath);
                string extension = Path.GetExtension(modelPath);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string oldVersionPath = Path.Combine(directory, $"{fileName}_old_{timestamp}{extension}");

                // Create a backup copy of the existing asset before it's overwritten.
                AssetDatabase.CopyAsset(modelPath, oldVersionPath);
                Debug.Log($"Backup created for {modelPath}. Old version saved as: {oldVersionPath}");
            }
        }
    }

    // Called when the model is imported
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string assetPath in importedAssets)
        {
            Debug.Log($"Imported asset: {assetPath}");
        }

        if (File.Exists(notificationPath))
        {
            foreach (string modelPath in importedAssets.Where(path => path.StartsWith("Assets/Models/") && path.EndsWith(".fbx")))
            {
                AssetDatabase.ImportAsset(modelPath, ImportAssetOptions.ForceUpdate);
                Debug.Log($"Updated model: {modelPath}");
            }

            File.Delete(notificationPath);
            Debug.Log("Models updated from Maya changes.");
        }
    }
}
