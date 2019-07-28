using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LostPolygon.AndroidBluetoothMultiplayer.Examples {
    public class DeviceBrowserController : MonoBehaviour
#if UNITY_ANDROID
        , ICustomDeviceBrowser
#endif
    {
        public GameObject DeviceBrowserPanelGameObject;
        public GameObject DeviceBrowserContentPanelGameObject;
        public GameObject DeviceBrowserDeviceButtonPrefab;
        public Button DeviceBrowserRefreshButton;
        public GameObject DeviceBrowserRefreshIndicatorGameObject;

#if UNITY_ANDROID
        private readonly Dictionary<BluetoothDevice, DeviceBrowserDeviceButtonData> _deviceBrowserButtons = new Dictionary<BluetoothDevice, DeviceBrowserDeviceButtonData>();
        private bool _isDiscovering;

        public void Clear() {
            foreach (DeviceBrowserDeviceButtonData deviceBrowserButton in _deviceBrowserButtons.Values.ToArray()) {
                DestroyDeviceBrowserButton(deviceBrowserButton);
            }
            _deviceBrowserButtons.Clear();
        }

        public void OnCloseButton() {
            Close();
        }

        public void OnRefreshButton() {
            if (AndroidBluetoothMultiplayer.GetIsBluetoothEnabled()) {
                AndroidBluetoothMultiplayer.StartDiscovery();
            }
        }

        private void OnEnable() {
            AndroidBluetoothMultiplayer.DeviceDiscovered += OnDeviceDiscovered;
            AndroidBluetoothMultiplayer.DiscoveryStarted += OnDiscoveryStarted;
            AndroidBluetoothMultiplayer.DiscoveryFinished += OnDiscoveryFinished;
        }

        private void OnDisable() {
            AndroidBluetoothMultiplayer.DeviceDiscovered -= OnDeviceDiscovered;
            AndroidBluetoothMultiplayer.DiscoveryStarted -= OnDiscoveryStarted;
            AndroidBluetoothMultiplayer.DiscoveryFinished -= OnDiscoveryFinished;
        }

        private void Update() {
            if (_isDiscovering) {
                DeviceBrowserRefreshIndicatorGameObject.transform.Rotate(0f, 0f, 270f * Time.deltaTime, Space.Self);
            } else {
                DeviceBrowserRefreshIndicatorGameObject.transform.rotation = Quaternion.identity;
            }
        }

        private void OnDiscoveryFinished() {
            _isDiscovering = false;
            DeviceBrowserRefreshButton.interactable = !_isDiscovering;
        }

        private void OnDiscoveryStarted() {
            _isDiscovering = true;
            DeviceBrowserRefreshButton.interactable = !_isDiscovering;
        }

        private void OnDeviceDiscovered(BluetoothDevice bluetoothDevice) {
            Debug.Log("Bluetooth Event - DeviceDiscovered");

            UpdateDeviceBrowserButton(bluetoothDevice, false);
        }

        private void UpdateDeviceBrowserButton(BluetoothDevice device, bool exitIfNoButton) {
            if (!device.IsConnectable)
                return;

            DeviceBrowserDeviceButtonData deviceBrowserDeviceButtonData;
            GameObject deviceBrowserButtonGameObject;
            if (!_deviceBrowserButtons.TryGetValue(device, out deviceBrowserDeviceButtonData)) {
                if (exitIfNoButton)
                    return;

                deviceBrowserButtonGameObject = Instantiate(DeviceBrowserDeviceButtonPrefab);
                deviceBrowserDeviceButtonData = deviceBrowserButtonGameObject.GetComponent<DeviceBrowserDeviceButtonData>();
                _deviceBrowserButtons.Add(device, deviceBrowserDeviceButtonData);

                deviceBrowserButtonGameObject.transform.SetParent(DeviceBrowserContentPanelGameObject.transform, false);
            }

            deviceBrowserDeviceButtonData.DeviceNameText.text = device.Name;
            if (!deviceBrowserDeviceButtonData.IsButtonRegistered) {
                deviceBrowserDeviceButtonData.Button.onClick.AddListener(() => {
                    if (OnDevicePicked != null)
                        OnDevicePicked(device);
                });
                deviceBrowserDeviceButtonData.IsButtonRegistered = true;
            }
        }

        private static void DestroyDeviceBrowserButton(DeviceBrowserDeviceButtonData deviceBrowserDeviceButtonData) {
            deviceBrowserDeviceButtonData.Button.onClick.RemoveAllListeners();
            Destroy(deviceBrowserDeviceButtonData.gameObject);
        }

        #region ICustomDeviceBrowser

#pragma warning disable 67
        public event Action OnOpened;
        public event Action OnClosing;
        public event Action<BluetoothDevice> OnDevicePicked;

#pragma warning restore 67

        public void Open() {
            DeviceBrowserPanelGameObject.SetActive(true);
            AndroidBluetoothMultiplayer.StartDiscovery();
            BluetoothDevice[] connectedDevices = AndroidBluetoothMultiplayer.GetDiscoveredDevices();
            foreach (BluetoothDevice connectedDevice in connectedDevices) {
                UpdateDeviceBrowserButton(connectedDevice, false);
            }

            if (OnOpened != null)
                OnOpened();
        }

        public void Close() {
            if (AndroidBluetoothMultiplayer.GetIsBluetoothEnabled()) {
                AndroidBluetoothMultiplayer.StopDiscovery();
            }

            Clear();
            if (OnClosing != null)
                OnClosing();
            DeviceBrowserPanelGameObject.SetActive(false);
        }

        #endregion
#endif
    }
}
