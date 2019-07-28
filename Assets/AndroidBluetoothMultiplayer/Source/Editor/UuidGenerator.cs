using System;
using UnityEditor;
using UnityEngine;

namespace LostPolygon.AndroidBluetoothMultiplayer.Editor {
    internal class UuidGenerator : EditorWindow {
#if UNITY_ANDROID
        private string _uuid = "";

        private void OnEnable() {
            titleContent = new GUIContent("UUID Generator");
        }

        private void OnGUI() {
            minSize = new Vector2(260f, 80f);
            maxSize = minSize;

            EditorGUILayout.LabelField("Randomly generated UUID: ");
            EditorGUILayout.SelectableLabel(_uuid, EditorStyles.textField, GUILayout.Height(16f));
            if (GUILayout.Button("Generate New UUID") || _uuid == "") {
                _uuid = Guid.NewGuid().ToString();
                GUI.FocusControl("");
                Repaint();
            }
        }
#endif
    }
}
