using UnityEditor;
using UnityEngine;

namespace LostPolygon.AndroidBluetoothMultiplayer.Editor {
    internal class MenuItems : EditorWindow {
        private const string kMenuRoot = "Tools/Lost Polygon/Android Bluetooth Multiplayer/";

        [MenuItem(kMenuRoot + "Generate New AndroidManifest.xml")]
        public static void GenerateManifest() {
            if (ShowWrongBuildPlatformDialog())
                return;

            ManifestGenerator.GenerateManifest();
        }

        [MenuItem(kMenuRoot + "Patch Existing AndroidManifest.xml")]
        public static void PatchManifest() {
            if (ShowWrongBuildPlatformDialog())
                return;

            if (!ManifestGenerator.PatchManifest()) {
                Debug.Log("AndroidManifest.xml is already patched.");
            }
        }

        [MenuItem(kMenuRoot + "Patch Existing AndroidManifest.xml", true)]
        public static bool PatchManifestValidate() {
            return ManifestGenerator.PatchManifestValidate();
        }

        [MenuItem(kMenuRoot + "UUID Generator", false, 10000)]
        private static void ShowUuidGeneratorWindow() {
            if (ShowWrongBuildPlatformDialog())
                return;

            GetWindow<UuidGenerator>(true);
        }

        private static bool ShowWrongBuildPlatformDialog() {
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                return false;

            EditorUtility.DisplayDialog(
                "Wrong build platform",
                "Build platform is not set to Android. " +
                "Please choose Android as build Platform in File - Build Settings...",
                "OK"
            );
            return true;
        }
    }
}
