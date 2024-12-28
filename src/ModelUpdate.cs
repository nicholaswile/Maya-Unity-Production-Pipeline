using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

public class ModelUpdate : AssetPostprocessor
{
    private static string notif_path = "Assets/Models/update_notification.txt";

    // Called before the model is imported.
    private void OnPreprocessModel()
    {
        if (File.Exists(notif_path))
        {
            // Asset path is a field of AssetPostprocessor which is the name of the asset being imported.
            string model_path = assetPath;

            // If that asset exists already, then we make a backup copy if it.
            if (File.Exists(model_path))
            {

                // Rename the asset. 
                string directory = Path.GetDirectoryName(model_path);
                string filename = Path.GetFileNameWithoutExtension(model_path);
                string extension = Path.GetExtension(model_path);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string backup_path = model_path.Combine(directory, $"{filename}_backup_{timestamp}{extension}");

                // Create a backup copy of the existing asset before it's overwritten.
                AssetDatabase.CopyAsset(model_path, backup_path);
                Debug.Log($"Backup created for {model_path}. Old version saved as: {backup_path}");
            }
        }
    }

    // Called when the model is imported.
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        // Display all imported assets.
        foreach (string asset_path in importedAssets)
        {
            Debug.Log($"Imported asset: {asset_path}");
        }

        // Check whether the imported assets come from the Maya scene using the notification file.
        if (File.Exists(notif_path))
        {
            // Import all assets that are models.
            foreach (string model_path in importedAssets.Where(path => path.StartsWith("Assets/Models/") && path.EndsWith(".fbx")))
            {
                AssetDatabase.ImportAsset(model_path, ImportAssetOptions.ForceUpdate);
                Debug.Log($"Updated model: {model_path}");
            }

            File.Delete(notif_path);
            Debug.Log("Models updated from Maya changes.");
        }
    }
}
