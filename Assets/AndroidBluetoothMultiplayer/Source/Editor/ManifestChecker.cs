#if UNITY_ANDROID

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace LostPolygon.AndroidBluetoothMultiplayer.Editor {
    [InitializeOnLoad]
    internal class ManifestChecker {
        static ManifestChecker() {
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged += GenerateManifestIfAbsent;
            GenerateManifestIfAbsent(PlayModeStateChange.EnteredEditMode);
#else
            EditorApplication.playmodeStateChanged += GenerateManifestIfAbsent;
            GenerateManifestIfAbsent();
#endif
        }

        [PostProcessScene]
        private static void GenerateManifestIfAbsentPostprocessScene() {
#if UNITY_2017_2_OR_NEWER
            GenerateManifestIfAbsent(PlayModeStateChange.EnteredEditMode);
#else
            GenerateManifestIfAbsent();
#endif
        }

#if UNITY_2017_2_OR_NEWER
        private static void GenerateManifestIfAbsent(PlayModeStateChange playModeStateChange) {
#else
        private static void GenerateManifestIfAbsent() {
#endif
            if (ManifestGenerator.IsManifestFileExists()) {
                ManifestGenerator.PatchManifest();
                return;
            }

            ManifestGenerator.GenerateManifest();
            Debug.Log("AndroidManifest.xml was missing, a new one was generated");
        }
    }
}

#endif