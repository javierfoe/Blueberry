#if UNITY_ANDROID

using UnityEngine;
using LostPolygon.AndroidBluetoothMultiplayer.Internal;

namespace LostPolygon.AndroidBluetoothMultiplayer {
    /// <summary>
    /// A core class that wraps Java methods of Android plugin.
    /// </summary>
    public sealed partial class AndroidBluetoothMultiplayer {
        #region Methods

        /// <summary>
        /// Initializes the plugin and sets the Bluetooth service UUID.
        /// </summary>
        /// <param name="uuid">Bluetooth service UUID. Must be different for each game.</param>
        /// <returns>true on success, false if UUID format is incorrect.</returns>
        public static bool Initialize(string uuid) {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("initUuid", uuid);
        }

        /// <summary>
        /// Starts the server that listens for incoming Bluetooth connections. Must be called before <see cref="Network.InitializeServer(int,int,bool)"/>.
        /// </summary>
        /// <param name="port">Server port number. Must be the same as passed to <see cref="Network.InitializeServer(int,int,bool)"/>.</param>
        /// <returns>true on success, false on error.</returns>
        /// <exception cref="BluetoothNotEnabledException">Thrown if called when Bluetooth was not enabled.</exception>
        public static bool StartServer(ushort port) {
            if (!_isPluginAvailable)
                return false;

            AssertIsBluetoothEnabled();
            return _plugin.Call<bool>("startServer", "127.0.0.1", (int) port);
        }

        /// <summary>
        /// Connects to a Bluetooth device. Must be called before <see cref="UnityEngine.Network.Connect(string,int)"/>.
        /// </summary>
        /// <param name="hostDeviceAddress">Address of host Bluetooth device to connect to.</param>
        /// <param name="port">Server port number. Must be the same as passed to  <see cref="UnityEngine.Network.Connect(string,int)"/>.</param>
        /// <returns>true on success, false on error/</returns>
        /// <exception cref="BluetoothNotEnabledException"> Thrown if called when Bluetooth was not enabled.</exception>
        public static bool Connect(string hostDeviceAddress, ushort port) {
            if (!_isPluginAvailable)
                return false;

            AssertIsBluetoothEnabled();
            return _plugin.Call<bool>("startClient", "127.0.0.1", (int) port, hostDeviceAddress);
        }

        /// <summary>
        /// Stops all Bluetooth connections.
        /// Client will disconnect from the server.
        /// Server will break connection with all the clients and then halt.
        /// </summary>
        /// <returns>true on success, false on error.</returns>
        public static bool Stop() {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("stop");
        }

        /// <summary>
        /// Starts listening for new incoming connections when
        /// in server mode.
        /// </summary>
        /// <returns>true on success, false on error.</returns>
        public static bool StartListening() {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("startListening");
        }

        /// <summary>
        /// Stops listening for new incoming connections when
        /// in server mode.
        /// </summary>
        /// <returns>true on success, false on error.</returns>
        public static bool StopListening() {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("stopListening");
        }

        /// <summary>
        /// Returns the current plugin <see cref="BluetoothMultiplayerMode"/>.
        /// </summary>
        /// <returns>The current plugin <see cref="BluetoothMultiplayerMode"/>.</returns>
        public static BluetoothMultiplayerMode GetCurrentMode() {
            if (!_isPluginAvailable)
                return BluetoothMultiplayerMode.None;

            return (BluetoothMultiplayerMode) _plugin.Call<byte>("getCurrentMode");
        }

        /// <summary>
        /// Opens a dialog asking user to make device discoverable
        /// on Bluetooth for <paramref name="discoverabilityDuration"/> seconds.
        /// This will also request the user to turn on Bluetooth if it was not enabled.
        /// </summary>
        /// <param name="discoverabilityDuration">
        /// The desired duration of discoverability (in seconds). Default value 120 seconds.
        /// On Android 4.0 and higher, value of 0 allows making device discoverable
        /// "forever" (until discoverability is disabled manually or Bluetooth is disabled).
        /// </param>
        /// <returns>true on success, false on error.</returns>
        public static bool RequestEnableDiscoverability(int discoverabilityDuration = 120) {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("requestEnableDiscoverability", discoverabilityDuration);
        }

        /// <summary>
        /// Opens a dialog asking the user to enable Bluetooth.
        /// It is recommended to use this method instead
        /// of <see cref="EnableBluetooth"/> for more native experience.
        /// </summary>
        /// <returns>true on success, false on error.</returns>
        public static bool RequestEnableBluetooth() {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("requestEnableBluetooth");
        }

        /// <summary>
        /// <para>Enables the Bluetooth adapter, if possible.</para>
        /// <para>Do not use this method unless you have provided a
        /// custom GUI acknowledging user about the action.
        /// Otherwise use <see cref="RequestEnableBluetooth"/>.</para>
        /// </summary>
        /// <returns>true on success, false on error.</returns>
        public static bool EnableBluetooth() {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("enableBluetooth");
        }

        /// <summary>
        /// Disables the Bluetooth adapter, if possible.
        /// </summary>
        /// <returns>true on success, false on error.</returns>
        public static bool DisableBluetooth() {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("disableBluetooth");
        }

        /// <summary>
        /// <para>Returns whether the Bluetooth is available.</para>
        /// <para>Bluetooth can be unavailable if no Bluetooth adapter
        /// is present, or if some error occurred.</para>
        /// </summary>
        /// <returns>true if Bluetooth connectivity is available, false otherwise.</returns>
        public static bool GetIsBluetoothAvailable() {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("isBluetoothAvailable");
        }

        /// <summary>
        /// Returns whether if Bluetooth is currently enabled and ready for use.
        /// </summary>
        /// <returns>true if Bluetooth connectivity is available and enabled, false otherwise.</returns>
        public static bool GetIsBluetoothEnabled() {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("isBluetoothEnabled");
        }

        /// <summary>
        /// <para>Shows the Bluetooth device picker dialog.</para>para>
        /// <para>Note: this method may fail some on exotic Android modifications like Amazon Fire OS.</para>
        /// </summary>
        /// <param name="showAllDeviceTypes">Whether to show all types or devices (including headsets, keyboards etc.) or only data-capable.</param>
        /// <returns>true on success, false on error.</returns>
        /// <exception cref="BluetoothNotEnabledException">Thrown if Bluetooth was not enabled.</exception>
        public static bool ShowDeviceList(bool showAllDeviceTypes = false) {
            if (!_isPluginAvailable)
                return false;

            AssertIsBluetoothEnabled();
            return _plugin.Call<bool>("showDeviceList", showAllDeviceTypes);
        }

        /// <summary>
        /// Returns <see cref="BluetoothDevice"/> of a device with address <paramref name="deviceAddress"/>.
        /// </summary>
        /// <returns><see cref="BluetoothDevice"/> if Bluetooth connectivity is available and enabled, null otherwise or on error.</returns>
        public static BluetoothDevice GetDeviceFromAddress(string deviceAddress) {
            if (!_isPluginAvailable)
                return null;

            AndroidJavaObject bluetoothDeviceJavaObject = _plugin.Call<AndroidJavaObject>("getBluetoothDeviceFromAddress", deviceAddress);
            if (bluetoothDeviceJavaObject.IsNull())
                return null;

            BluetoothDevice bluetoothDevice = new BluetoothDevice(bluetoothDeviceJavaObject);
            return bluetoothDevice;
        }

        /// <summary>
        /// Returns <see cref="BluetoothDevice"/> of the current device the application runs on.
        /// </summary>
        /// <returns><see cref="BluetoothDevice"/> if Bluetooth connectivity is available and enabled, null otherwise or on error.</returns>
        public static BluetoothDevice GetCurrentDevice() {
            if (!_isPluginAvailable)
                return null;

            AndroidJavaObject bluetoothDeviceJavaObject = _plugin.Call<AndroidJavaObject>("getCurrentDevice");
            if (bluetoothDeviceJavaObject.IsNull())
                return null;

            BluetoothDevice currentDevice = new BluetoothDevice(bluetoothDeviceJavaObject);
            return currentDevice;
        }

        /// <summary>
        /// <para>Starts the process of discovering nearby discoverable Bluetooth devices.
        /// The process is asynchronous and is usually held for 10-30 seconds in time.</para>
        /// <para>Note that performing device discovery is a heavy procedure for the
        /// Bluetooth adapter and will consume a lot of its resources and drain battery power.</para>
        /// </summary>
        /// <returns>true if Bluetooth connectivity is available and enabled, false otherwise.</returns>
        public static bool StartDiscovery() {
            if (!_isPluginAvailable)
                return false;

            AssertIsBluetoothEnabled();
            return _plugin.Call<bool>("startDiscovery");
        }

        /// <summary>
        /// <para>Stops the process of discovering nearby discoverable Bluetooth devices.</para>
        /// <para>Because discovery is a heavyweight procedure for the Bluetooth adapter,
        /// this method is called automatically when connecting to the server.</para>
        /// </summary>
        /// <returns>
        /// true if Bluetooth connectivity is available and enabled
        /// and the discovery was going on, false otherwise.
        /// </returns>
        public static bool StopDiscovery() {
            if (!_isPluginAvailable)
                return false;

            AssertIsBluetoothEnabled();
            return _plugin.Call<bool>("stopDiscovery");
        }

        /// <summary>
        /// Returns whether the local Bluetooth adapter is currently in process of device discovery.
        /// </summary>
        /// <returns>
        /// true if Bluetooth connectivity is available and enabled and
        /// device discovery is currently going on, false otherwise.
        /// </returns>
        public static bool GetIsDiscovering() {
            if (!_isPluginAvailable)
                return false;

            AssertIsBluetoothEnabled();
            return _plugin.Call<bool>("isDiscovering");
        }

        /// <summary>
        /// Returns whether the local Bluetooth adapter can be discovered by other devices.
        /// </summary>
        /// <returns>
        /// true if Bluetooth connectivity is available and enabled and
        /// device is currently discoverable by other devices, false otherwise.
        /// </returns>
        public static bool GetIsDiscoverable() {
            if (!_isPluginAvailable)
                return false;

            AssertIsBluetoothEnabled();
            return _plugin.Call<bool>("isDiscoverable");
        }

        /// <summary>
        /// Returns <see cref="BluetoothDevice[]"/> of bonded (paired) devices.
        /// This method is available even without starting the discovery process.
        /// </summary>
        /// <returns>The <see cref="BluetoothDevice[]"/> of bonded (paired) devices.</returns>
        public static BluetoothDevice[] GetBondedDevices() {
            if (!_isPluginAvailable)
                return null;

            AndroidJavaObject deviceJavaSet = _plugin.Call<AndroidJavaObject>("getBondedDevices");
            BluetoothDevice[] deviceArray = ConvertJavaBluetoothDeviceSet(deviceJavaSet);

            return deviceArray;
        }

        /// <summary>
        /// Returns <see cref="BluetoothDevice[]"/> of devices discovered
        /// during the last or current discovery session.
        /// This list is not cleared after the discovery ends.
        /// </summary>
        /// <returns>The <see cref="BluetoothDevice[]"/> of the discovered Bluetooth devices.</returns>
        public static BluetoothDevice[] GetNewDiscoveredDevices() {
            if (!_isPluginAvailable)
                return null;

            AndroidJavaObject deviceJavaSet = _plugin.Call<AndroidJavaObject>("getNewDiscoveredDevices");
            BluetoothDevice[] deviceArray = ConvertJavaBluetoothDeviceSet(deviceJavaSet);

            return deviceArray;
        }

        /// <summary>
        /// Returns <see cref="BluetoothDevice[]"/> of bonded (paired) devices and
        /// devices discovered during the ongoing discovery session.
        /// This list is not cleared after the discovery ends.
        /// </summary>
        /// <returns>The <see cref="BluetoothDevice[]"/> of the discovered Bluetooth devices.</returns>
        public static BluetoothDevice[] GetDiscoveredDevices() {
            if (!_isPluginAvailable)
                return null;

            AndroidJavaObject deviceJavaSet = _plugin.Call<AndroidJavaObject>("getDiscoveredDevices");
            BluetoothDevice[] deviceArray = ConvertJavaBluetoothDeviceSet(deviceJavaSet);

            return deviceArray;
        }

        /// <summary>
        /// Enables or disables raw packets mode. Could only be called when no Bluetooth networking is going on.
        /// This option can be used if you want to exchange raw data with a generic Bluetooth device
        /// (like an Arduino with a Bluetooth Shield). Use this only if you know what you are doing.
        /// </summary>
        /// <param name="isEnabled">
        /// The new state of raw packets mode.
        /// </param>
        /// <returns>true if no Bluetooth networking is going on, false otherwise.</returns>
        public static bool SetRawPackets(bool isEnabled) {
            if (!_isPluginAvailable)
                return false;

            return _plugin.Call<bool>("setRawPackets", isEnabled);
        }

        /// <summary>
        /// Enables or disables verbose logging. Useful for testing and debugging..
        /// </summary>
        /// <param name="isEnabled">The new state of verbose logging.</param>
        public static void SetVerboseLog(bool isEnabled) {
            if (!_isPluginAvailable)
                return;

            _plugin.CallStatic("setVerboseLog", isEnabled);
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Converts a Java Set of android.Bluetooth.BluetoothDevice into its C# representation.
        /// </summary>
        /// <param name="bluetoothDeviceJavaSet">The Java Set of android.Bluetooth.BluetoothDevice.</param>
        /// <returns>The converted <see cref="BluetoothDevice[]"/>.</returns>
        private static BluetoothDevice[] ConvertJavaBluetoothDeviceSet(AndroidJavaObject bluetoothDeviceJavaSet) {
            try {
                if (bluetoothDeviceJavaSet.IsNull())
                    return null;

                AndroidJavaObject[] deviceJavaArray = bluetoothDeviceJavaSet.Call<AndroidJavaObject[]>("toArray");
                BluetoothDevice[] deviceArray = new BluetoothDevice[deviceJavaArray.Length];
                for (int i = 0; i < deviceJavaArray.Length; i++) {
                    deviceArray[i] = new BluetoothDevice(deviceJavaArray[i]);
                }

                return deviceArray;
            } catch {
                Debug.LogError("Exception while converting BluetoothDevice Set");
                throw;
            }
        }

        /// <summary>
        /// Throws an <see cref="BluetoothNotEnabledException"/> if
        /// called when Bluetooth was not enabled.
        /// </summary>
        private static void AssertIsBluetoothEnabled() {
            if (!GetIsBluetoothEnabled())
                throw new BluetoothNotEnabledException();
        }

        #endregion
    }
}

#endif