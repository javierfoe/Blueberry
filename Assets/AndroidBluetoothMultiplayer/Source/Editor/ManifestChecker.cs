using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace javierfoe.AndroidBluetoothMultiplayer.Editor
{
    [InitializeOnLoad]
    internal class ManifestChecker
    {
        static ManifestChecker()
        {
            EditorApplication.playModeStateChanged += GenerateManifestIfAbsent;
            GenerateManifestIfAbsent(PlayModeStateChange.EnteredEditMode);
        }

        [PostProcessScene]
        private static void GenerateManifestIfAbsentPostprocessScene()
        {
            GenerateManifestIfAbsent(PlayModeStateChange.EnteredEditMode);
        }

        private static void GenerateManifestIfAbsent(PlayModeStateChange playModeStateChange)
        {
            if (ManifestGenerator.IsManifestFileExists())
            {
                ManifestGenerator.PatchManifest();
                return;
            }

            ManifestGenerator.GenerateManifest();
            Debug.Log("AndroidManifest.xml was missing, a new one was generated");
        }
    }
}