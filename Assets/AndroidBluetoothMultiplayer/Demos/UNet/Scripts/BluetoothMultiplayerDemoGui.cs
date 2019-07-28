using UnityEngine;
using Mirror;
using UnityEngine.UI;

namespace LostPolygon.AndroidBluetoothMultiplayer.Examples.UNet {
    public class BluetoothMultiplayerDemoGui : BluetoothDemoGuiBase {
        public GameObject TapMarkerPrefab;

        public AndroidBluetoothNetworkManagerHelper AndroidBluetoothNetworkManagerHelper;
        public BluetoothMultiplayerDemoNetworkManager BluetoothMultiplayerDemoNetworkManager;

        public DeviceBrowserController CustomDeviceBrowser;

        public GameObject UIPanelGameObject;
        public GameObject ErrorUIPanelGameObject;

        public GameObject StartServerButtonGameObject;
        public GameObject ConnectToServerButtonGameObject;
        public GameObject DisconnectButtonGameObject;

        public Toggle StressTestToggle;
        public Toggle UseCustomDeviceBrowserUIToggle;

#if !UNITY_ANDROID
        private void Awake() {
            Debug.LogError("Build platform is not set to Android. Please choose Android as build Platform in File - Build Settings...");
        }

        protected override void OnEnable() {
            base.OnEnable();

            UIPanelGameObject.SetActive(false);
            ErrorUIPanelGameObject.SetActive(true);
        }
#else

        protected override void Update() {
            base.Update();

            // Spawn an effect where user has tapped
            if (NetworkClient.active && Input.GetMouseButtonDown(0)) {
                Vector2 tapPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // Send the message with the tap position to the server, so it can send it to other clients
                NetworkClient.Send(new CreateTapMarkerMessage { Position = tapPosition });

                // Local client can just instantiate the effect for immediate response, as this is a purely visual thing
                Instantiate(TapMarkerPrefab, tapPosition, Quaternion.identity);
            }

            // Refresh UI
            UIPanelGameObject.SetActive(AndroidBluetoothNetworkManagerHelper.IsInitialized);
            ErrorUIPanelGameObject.SetActive(!AndroidBluetoothNetworkManagerHelper.IsInitialized);

            if (!AndroidBluetoothNetworkManagerHelper.IsInitialized)
                return;

            BluetoothMultiplayerMode currentMode = AndroidBluetoothMultiplayer.GetCurrentMode();
            StartServerButtonGameObject.SetActive(currentMode == BluetoothMultiplayerMode.None);
            ConnectToServerButtonGameObject.SetActive(currentMode == BluetoothMultiplayerMode.None);
            DisconnectButtonGameObject.SetActive(currentMode != BluetoothMultiplayerMode.None);
            if (DisconnectButtonGameObject.activeInHierarchy) {
                DisconnectButtonGameObject.GetComponentInChildren<Text>().text = currentMode == BluetoothMultiplayerMode.Client ? "Disconnect" : "Stop server";
            }

            bool togglesInteractable = currentMode == BluetoothMultiplayerMode.None;
            bool togglesActive = currentMode != BluetoothMultiplayerMode.Client;

            StressTestToggle.interactable = togglesInteractable;
            StressTestToggle.gameObject.SetActive(togglesActive);

            UseCustomDeviceBrowserUIToggle.interactable = togglesInteractable;
            UseCustomDeviceBrowserUIToggle.gameObject.SetActive(togglesActive);

            // Update values
            BluetoothMultiplayerDemoNetworkManager.StressTestMode = StressTestToggle.isOn;
        }

        private void Awake() {
            // Enabling verbose logging. See logcat!
            AndroidBluetoothMultiplayer.SetVerboseLog(true);
        }

        protected override void OnGoingBackToMenu() {
            // Gracefully closing all Bluetooth connectivity and loading the menu
            try {
                BluetoothMultiplayerDemoNetworkManager.StopHost();
                AndroidBluetoothMultiplayer.StopDiscovery();
                AndroidBluetoothMultiplayer.Stop();
            } catch {
                //
            }
        }

        #region UI Handlers

        public void OnBackToMenuButton() {
            GoBackToMenu();
        }

        public void OnStartServerButton() {
            AndroidBluetoothNetworkManagerHelper.StartHost();
        }

        public void OnConnectToServerButton() {
            AndroidBluetoothNetworkManagerHelper.SetCustomDeviceBrowser(UseCustomDeviceBrowserUIToggle.isOn ? CustomDeviceBrowser : null);
            AndroidBluetoothNetworkManagerHelper.StartClient();
        }

        public void OnDisconnectButton() {
            AndroidBluetoothMultiplayer.StopDiscovery();
            AndroidBluetoothMultiplayer.Stop();
            BluetoothMultiplayerDemoNetworkManager.StopHost();
        }

        #endregion
#endif
    }
}