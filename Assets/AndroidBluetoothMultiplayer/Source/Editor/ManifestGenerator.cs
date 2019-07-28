using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace LostPolygon.AndroidBluetoothMultiplayer.Editor {
    internal static class ManifestGenerator {
        private const string kCoarseLocationPermission = "android.permission.ACCESS_COARSE_LOCATION";
        private const string kFineLocationPermission = "android.permission.ACCESS_FINE_LOCATION";

        private static readonly string[] kRequiredPermissions =
        {
            "android.permission.BLUETOOTH_ADMIN",
            "android.permission.BLUETOOTH"
        };

        private static readonly string[] kActivityNameReplaceFrom =
        {
            "com.unity3d.player.UnityPlayerProxyActivity",
            "com.unity3d.player.UnityPlayerActivity",
            "com.unity3d.player.UnityPlayerNativeActivity"
        };

        private static readonly string[] kActivityNameReplaceTo =
        {
            "com.lostpolygon.unity.bluetoothmediator.player.BluetoothUnityPlayerProxyActivity",
            "com.lostpolygon.unity.bluetoothmediator.player.BluetoothUnityPlayerActivity",
            "com.lostpolygon.unity.bluetoothmediator.player.BluetoothUnityPlayerNativeActivity"
        };

        public static void GenerateManifest() {
            string newManifestPath = GetProjectManifestPath();

            if (File.Exists(newManifestPath) &&
                !EditorUtility.DisplayDialog(
                    "Overwrite existing AndroidManifest.xml",
                    "An existing AndroidManifest.xml file is present. Are you sure you want to overwrite it?",
                    "Overwrite",
                    "Cancel"))
                return;

            try {
                string defaultManifestText = GetDefaultManifestText();
                XmlDocument manifestXmlDocument = new XmlDocument();
                manifestXmlDocument.PreserveWhitespace = true;
                manifestXmlDocument.LoadXml(defaultManifestText);
                PatchManifest(manifestXmlDocument);

                string newManifestDirectoryPath = Path.GetDirectoryName(newManifestPath);
                if (newManifestDirectoryPath != null)
                    Directory.CreateDirectory(newManifestDirectoryPath);

                SaveAsUtf8(manifestXmlDocument, newManifestPath, true);
                AssetDatabase.ImportAsset(newManifestPath, ImportAssetOptions.ForceUpdate);

                Debug.Log("AndroidManifest.xml generated successfully!");
            } catch (Exception e) {
                throw new Exception("Can't generate AndroidManifest.xml!\n" + e);
            }
        }

        public static bool PatchManifest() {
            if (!IsManifestFileExists())
                return false;

            string manifestPath = GetProjectManifestPath();
            try {
                XmlDocument manifestXmlDocument = new XmlDocument();
                manifestXmlDocument.PreserveWhitespace = true;
                manifestXmlDocument.Load(manifestPath);
                bool isModified = PatchManifest(manifestXmlDocument);
                if (isModified) {
                    SaveAsUtf8(manifestXmlDocument, manifestPath, false);
                    AssetDatabase.ImportAsset(manifestPath, ImportAssetOptions.ForceUpdate);
                    Debug.Log("AndroidManifest.xml patched.");
                }
                return isModified;
            } catch (Exception e) {
                throw new Exception("Can't patch AndroidManifest.xml!\n", e);
            }
        }

        public static bool IsManifestFileExists() {
            string manifestPath = GetProjectManifestPath();
            return File.Exists(manifestPath);
        }

        public static bool PatchManifestValidate() {
            return IsManifestFileExists();
        }

        private static bool PatchManifest(XmlDocument manifestXmlDocument) {
            bool isModified = false;

            isModified |= PatchManifestActivityClassNames(manifestXmlDocument);
            isModified |= PatchManifestPermissions(manifestXmlDocument);

            return isModified;
        }

        private static bool PatchManifestPermissions(XmlDocument manifestXmlDocument) {
            bool isModified = false;
            IEnumerable<XmlElement> permissionNodes = GetChildElementsWithName(manifestXmlDocument.DocumentElement, "uses-permission");
            List<string> existingPermissions =
                permissionNodes
                    .Where(node => node.HasAttribute("android:name"))
                    .Select(node => node.Attributes["android:name"].Value)
                    .ToList();

            List<string> missingPermissions = kRequiredPermissions.Except(existingPermissions).ToList();

            // At least coarse location permission is required
            if (!existingPermissions.Contains(kCoarseLocationPermission) && !existingPermissions.Contains(kFineLocationPermission)) {
                missingPermissions.Add(kCoarseLocationPermission);
            }

            if (missingPermissions.Count > 0) {
                isModified = true;
            }

            const string androidXmlNamespace = "http://schemas.android.com/apk/res/android";
            foreach (string missingPermission in missingPermissions) {
                XmlElement permissionNode = manifestXmlDocument.CreateElement("uses-permission");
                XmlAttribute permissionNodeNameAttribute = manifestXmlDocument.CreateAttribute("android:name", androidXmlNamespace);
                permissionNodeNameAttribute.Value = missingPermission;

                permissionNode.Attributes.Append(permissionNodeNameAttribute);
                manifestXmlDocument.DocumentElement.AppendChild(permissionNode);
                manifestXmlDocument.DocumentElement.AppendChild(manifestXmlDocument.CreateWhitespace(Environment.NewLine));
            }

            return isModified;
        }

        private static bool PatchManifestActivityClassNames(XmlDocument manifestXmlDocument) {
            bool isModified = false;

            XmlElement applicationNode = GetFirstChildElementWithName(manifestXmlDocument.DocumentElement, "application");
            IEnumerable<XmlElement> activityNodes = GetChildElementsWithName(applicationNode, "activity");
            foreach (XmlElement activityNode in activityNodes) {
                if (!activityNode.HasAttribute("android:name"))
                    continue;

                XmlAttribute nameAttribute = activityNode.Attributes["android:name"];
                for (int i = 0; i < kActivityNameReplaceFrom.Length; i++) {
                    if (nameAttribute.Value != kActivityNameReplaceFrom[i])
                        continue;

                    nameAttribute.Value = kActivityNameReplaceTo[i];
                    isModified = true;
                    break;
                }
            }

            return isModified;
        }

        private static string GetProjectManifestPath() {
            string manifestFilePath = PathCombine("Assets", "Plugins", "Android", "AndroidManifest.xml");
            return manifestFilePath;
        }

        private static string GetDefaultManifestText() {
            try {
                string unityContentsPath = EditorApplication.applicationContentsPath;

                string[] manifestPathComponentsStyle1 = { "PlaybackEngines", "androidplayer", "Apk", "AndroidManifest.xml" };
                string[] manifestPathComponentsStyle2 = { "PlaybackEngines", "androidplayer", "AndroidManifest.xml" };

                string manifestPath = PathCombine(unityContentsPath, PathCombine(manifestPathComponentsStyle1));

                if (!File.Exists(manifestPath)) {
                    manifestPath = PathCombine(unityContentsPath, PathCombine(manifestPathComponentsStyle2));

                    if (!File.Exists(manifestPath)) {
                        string unityRootPath = PathCombine(EditorApplication.applicationPath, "..");

                        manifestPath = PathCombine(unityRootPath, PathCombine(manifestPathComponentsStyle1));
                        if (!File.Exists(manifestPath))
                            throw new FileNotFoundException("Default AndroidManifest.xml not found");
                    }
                }

                string manifest = File.ReadAllText(manifestPath);

                return manifest;
            } catch (Exception e) {
                throw new Exception("Error getting default AndroidManifest.xml!", e);
            }
        }

        private static XmlElement GetFirstChildElementWithName(XmlElement element, string parentNodeName) {
            XmlElement parentNode =
                element
                .ChildNodes
                .OfType<XmlElement>()
                .FirstOrDefault(childElement => childElement.LocalName == parentNodeName);

            return parentNode;
        }

        private static IEnumerable<XmlElement> GetChildElementsWithName(XmlElement element, string nodeName) {
            IEnumerable<XmlElement> nodes =
                element
                .ChildNodes
                .OfType<XmlElement>()
                .Where(childElement => childElement.LocalName == nodeName);

            return nodes;
        }

        private static void SaveAsUtf8(XmlDocument xmlDocument, string filePath, bool reindent = false) {
            UTF8Encoding utf8EncodingNoBom = new UTF8Encoding(false);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = utf8EncodingNoBom; // Do not emit the BOM

            if (!reindent) {
                using (XmlWriter xmlWriter = XmlWriter.Create(filePath, settings)) {
                    xmlDocument.Save(xmlWriter);
                }
            } else {
                XElement element = XElement.Parse(xmlDocument.InnerXml);
                settings.Indent = true;
                using (XmlWriter xmlWriter = XmlWriter.Create(filePath, settings)) {
                    element.Save(xmlWriter);
                }
            }
        }

        private static string PathCombine(params string[] paths) {
            if (paths == null)
                throw new ArgumentNullException("paths");

            if (paths.Length == 2)
                return Path.Combine(paths[0], paths[1]);

            string result = paths[0];
            for (int i = 1; i < paths.Length; i++) {
                result = Path.Combine(result, paths[i]);
            }

            return result;
        }
    }
}