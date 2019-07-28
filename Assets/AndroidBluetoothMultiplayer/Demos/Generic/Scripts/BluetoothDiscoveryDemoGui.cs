#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
#  define PRE_UNITY_5
#endif

using UnityEngine;

namespace LostPolygon.AndroidBluetoothMultiplayer.Examples{
    public class BluetoothDiscoveryDemoGui : BluetoothDemoGuiBase {
#if !UNITY_ANDROID
        private void Awake() {
            Debug.LogError("Build platform is not set to Android. Please choose Android as build Platform in File - Build Settings...");
        }

        private void OnGUI() {
            GUI.Label(new Rect(10, 10, Screen.width - 10, 100), "Build platform is not set to Android. Please choose Android as build Platform in File - Build Settings...");
        }
#else
        private bool _initResult;
        private string _log;
        private Vector2 _logPosition = Vector2.zero;

        private void HandleLog(string logString, string stackTrace, LogType logType) {
            if (logType == LogType.Error || logType == LogType.Exception) {
                _log += string.Format("Error: {0}, stacktrace: \n {1}", logString, stackTrace);
            } else {
                _log += logString + "\r\n";
            }
        }

        private void Awake() {
#if PRE_UNITY_5
            Application.RegisterLogCallback(HandleLog);
#else
            Application.logMessageReceived += HandleLog;
#endif

            HandleLog("This demo shows some available methods and the discovery of nearby Bluetooth devices.\r\n", "", LogType.Log);
            // Setting the UUID. Must be unique for every application
            _initResult = AndroidBluetoothMultiplayer.Initialize("8ce255c0-200a-11e0-ac64-0800200c9a66");
            // Enabling verbose logging. See logcat!
            AndroidBluetoothMultiplayer.SetVerboseLog(true);

            // Registering the event listeners
            AndroidBluetoothMultiplayer.AdapterEnabled += OnBluetoothAdapterEnabled;
            AndroidBluetoothMultiplayer.AdapterEnableFailed += OnBluetoothAdapterEnableFailed;
            AndroidBluetoothMultiplayer.AdapterDisabled += OnBluetoothAdapterDisabled;
            AndroidBluetoothMultiplayer.DiscoverabilityEnabled += OnBluetoothDiscoverabilityEnabled;
            AndroidBluetoothMultiplayer.DiscoverabilityEnableFailed += OnBluetoothDiscoverabilityEnableFailed;
            AndroidBluetoothMultiplayer.DiscoveryStarted += OnBluetoothDiscoveryStarted;
            AndroidBluetoothMultiplayer.DiscoveryFinished += OnBluetoothDiscoveryFinished;
            AndroidBluetoothMultiplayer.DeviceDiscovered += OnBluetoothDeviceDiscovered;
        }

        // Don't forget to unregister the event listeners!
        protected override void OnDestroy() {
            base.OnDestroy();

            AndroidBluetoothMultiplayer.AdapterEnabled -= OnBluetoothAdapterEnabled;
            AndroidBluetoothMultiplayer.AdapterEnableFailed -= OnBluetoothAdapterEnableFailed;
            AndroidBluetoothMultiplayer.AdapterDisabled -= OnBluetoothAdapterDisabled;
            AndroidBluetoothMultiplayer.DiscoverabilityEnabled -= OnBluetoothDiscoverabilityEnabled;
            AndroidBluetoothMultiplayer.DiscoverabilityEnableFailed -= OnBluetoothDiscoverabilityEnableFailed;
            AndroidBluetoothMultiplayer.DiscoveryStarted -= OnBluetoothDiscoveryStarted;
            AndroidBluetoothMultiplayer.DiscoveryFinished -= OnBluetoothDiscoveryFinished;
            AndroidBluetoothMultiplayer.DeviceDiscovered -= OnBluetoothDeviceDiscovered;

#if PRE_UNITY_5
            Application.RegisterLogCallback(null);
#else
            Application.logMessageReceived -= HandleLog;
#endif
        }

        private void OnGUI() {
            bool isBluetoothEnabled = AndroidBluetoothMultiplayer.GetIsBluetoothEnabled();
            bool isDiscoverable = false;
            bool isDiscovering = false;
            try {
                isDiscoverable = isBluetoothEnabled && AndroidBluetoothMultiplayer.GetIsDiscoverable();
                isDiscovering = isBluetoothEnabled && AndroidBluetoothMultiplayer.GetIsDiscovering();
            } catch (BluetoothNotEnabledException) {
                // This may happen in some rare cases when Bluetooth actually gets disabled
                // in the middle of C# code execution. In that case we may get a
                // BluetoothNotEnabledException, but it is safe to ignore it here.
            }

            float scaleFactor = BluetoothExamplesTools.UpdateScaleMobile();
            // Show the buttons if initialization succeeded
            if (_initResult) {
                // Simple text log view
                GUILayout.Space(190f);
                BluetoothExamplesTools.TouchScroll(ref _logPosition);
                _logPosition = GUILayout.BeginScrollView(
                    _logPosition,
                    GUILayout.MaxHeight(Screen.height / scaleFactor - 190f),
                    GUILayout.MinWidth(Screen.width / scaleFactor - 10f),
                    GUILayout.ExpandHeight(false));
                GUI.contentColor = Color.black;
                GUILayout.Label(_log, GUILayout.ExpandHeight(true), GUILayout.MaxWidth(Screen.width / scaleFactor));
                GUI.contentColor = Color.white;
                GUILayout.EndScrollView();

                // Generic GUI for calling the methods
                GUI.enabled = !isBluetoothEnabled;
                if (GUI.Button(new Rect(10, 10, 140, 50), "Request enable\nBluetooth")) {
                    AndroidBluetoothMultiplayer.RequestEnableBluetooth();
                }

                GUI.enabled = isBluetoothEnabled;
                if (GUI.Button(new Rect(160, 10, 140, 50), "Disable Bluetooth")) {
                    AndroidBluetoothMultiplayer.DisableBluetooth();
                }
                GUI.enabled = !isBluetoothEnabled || !isDiscoverable;
                if (GUI.Button(new Rect(310, 10, 150, 50), "Request discoverability")) {
                    AndroidBluetoothMultiplayer.RequestEnableDiscoverability(120);
                }

                GUI.enabled = isBluetoothEnabled && !isDiscovering;
                if (GUI.Button(new Rect(10, 70, 140, 50), "Start discovery")) {
                    AndroidBluetoothMultiplayer.StartDiscovery();
                }

                GUI.enabled = isBluetoothEnabled && isDiscovering;
                if (GUI.Button(new Rect(160, 70, 140, 50), "Stop discovery")) {
                    AndroidBluetoothMultiplayer.StopDiscovery();
                }

                GUI.enabled = isBluetoothEnabled;
                if (GUI.Button(new Rect(310, 70, 150, 50), "Get current\ndevice")) {
                    Debug.Log("Current device:");
                    BluetoothDevice device = AndroidBluetoothMultiplayer.GetCurrentDevice();
                    if (device != null) {
                        // Result can be null on error or if Bluetooth is not available
                        Debug.Log(string.Format("Device: " + BluetoothExamplesTools.FormatDevice(device)));
                    } else {
                        Debug.LogError("Error while retrieving current device");
                    }
                }

                // Just get the device lists and prints them
                if (GUI.Button(new Rect(10, 130, 140, 50), "Show bonded\ndevice list")) {
                    Debug.Log("Listing known bonded (paired) devices");
                    BluetoothDevice[] list = AndroidBluetoothMultiplayer.GetBondedDevices();

                    if (list != null) {
                        // Result can be null on error or if Bluetooth is not available
                        if (list.Length == 0) {
                            Debug.Log("No devices");
                        } else {
                            foreach (BluetoothDevice device in list) {
                                Debug.Log("Device: " + BluetoothExamplesTools.FormatDevice(device));
                            }
                        }
                    } else {
                        Debug.LogError("Error while retrieving GetBondedDevices()");
                    }
                }

                if (GUI.Button(new Rect(160, 130, 140, 50), "Show new discovered\ndevice list")) {
                    Debug.Log("Listing devices discovered during last discovery session...");
                    BluetoothDevice[] list = AndroidBluetoothMultiplayer.GetNewDiscoveredDevices();

                    if (list != null) {
                        // Result can be null on error or if Bluetooth is not available
                        if (list.Length == 0) {
                            Debug.Log("No devices");
                        } else {
                            foreach (BluetoothDevice device in list) {
                                Debug.Log("Device: " + BluetoothExamplesTools.FormatDevice(device));
                            }
                        }
                    } else {
                        Debug.LogError("Error while retrieving GetNewDiscoveredDevices()");
                    }
                }

                if (GUI.Button(new Rect(310, 130, 150, 50), "Show full\ndevice list")) {
                    Debug.Log("Listing all known or discovered devices...");
                    BluetoothDevice[] list = AndroidBluetoothMultiplayer.GetDiscoveredDevices();

                    if (list != null) {
                        // Result can be null on error or if Bluetooth is not available
                        if (list.Length == 0) {
                            Debug.Log("No devices");
                        } else {
                            foreach (BluetoothDevice device in list) {
                                Debug.Log("Device: " + BluetoothExamplesTools.FormatDevice(device));
                            }
                        }
                    } else {
                        Debug.LogError("Error while retrieving GetDiscoveredDevices()");
                    }
                }

                GUI.enabled = true;
                // Show a message if initialization failed for some reason
            } else {
                GUI.contentColor = Color.black;
                GUI.Label(
                    new Rect(10, 10, Screen.width / scaleFactor - 10, 50),
                    "Bluetooth not available. Are you running this on Bluetooth-capable " +
                    "Android device and AndroidManifest.xml is set up correctly?");
            }

            DrawBackButton(scaleFactor);
        }

        protected override void OnGoingBackToMenu() {
            // Gracefully closing all Bluetooth connectivity and loading the menu
            try {
                AndroidBluetoothMultiplayer.StopDiscovery();
                AndroidBluetoothMultiplayer.Stop();
            } catch {
                //
            }
        }

        private void OnBluetoothDeviceDiscovered(BluetoothDevice device) {
            // For demo purposes, just display the device info
            Debug.Log(
                string.Format(
                    "Event - DeviceDiscovered(): {0} [{1}], class: {2}, is connectable: {3}",
                    device.Name,
                    device.Address,
                    device.DeviceClass,
                    device.IsConnectable)
                );
        }

        private void OnBluetoothAdapterDisabled() {
            Debug.Log("Event - AdapterDisabled()");
        }

        private void OnBluetoothAdapterEnableFailed() {
            Debug.Log("Event - AdapterEnableFailed()");
        }

        private void OnBluetoothAdapterEnabled() {
            Debug.Log("Event - AdapterEnabled()");
        }

        private void OnBluetoothDiscoverabilityEnableFailed() {
            Debug.Log("Event - DiscoverabilityEnableFailed()");
        }

        private void OnBluetoothDiscoverabilityEnabled(int discoverabilityDuration) {
            Debug.Log(string.Format("Event - DiscoverabilityEnabled(): {0} seconds", discoverabilityDuration));
        }

        private void OnBluetoothDiscoveryFinished() {
            Debug.Log("Event - DiscoveryFinished()");
        }

        private void OnBluetoothDiscoveryStarted() {
            Debug.Log("Event - DiscoveryStarted()");
        }
#endif
    }
}