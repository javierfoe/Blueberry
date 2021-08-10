using UnityEngine;
using UnityEngine.UI;

namespace LostPolygon.AndroidBluetoothMultiplayer.Examples {
    public class DeviceBrowserDeviceButtonData : MonoBehaviour {
        public Button Button;
        public Text DeviceNameText;

        [HideInInspector]
        public bool IsButtonRegistered;
    }
}
