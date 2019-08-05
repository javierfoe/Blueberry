#if UNITY_ANDROID

using System;

namespace LostPolygon.AndroidBluetoothMultiplayer {
    public sealed partial class AndroidBluetoothMultiplayer {
        #region Events

        /// <summary>
        /// Fired when server is started and waiting for incoming connections.
        /// </summary>
        public static event Action ListeningStarted;

        /// <summary>
        /// Fired when listening for incoming connections.
        /// was stopped by <see cref="StopListening"/>.
        /// </summary>
        public static event Action ListeningStopped;

        /// <summary>
        /// Fired when Bluetooth adapter was enabled.
        /// </summary>
        public static event Action AdapterEnabled;

        /// <summary>
        /// Fired when request to enabled Bluetooth failed for some reason.
        /// (user did not authorized to enable Bluetooth or an error occured).
        /// </summary>
        public static event Action AdapterEnableFailed;

        /// <summary>
        /// Fired when Bluetooth adapter was disabled.
        /// </summary>
        public static event Action AdapterDisabled;

        /// <summary>
        /// Fired when Bluetooth discoverability was enabled.
        /// </summary>
        public static event Action<int> DiscoverabilityEnabled;

        /// <summary>
        /// Fired when request to enabled Bluetooth discoverability failed for some reason.
        /// (user dismissed the request dialog or an error occured).
        /// </summary>
        public static event Action DiscoverabilityEnableFailed;

        /// <summary>
        /// Fired when Bluetooth client successfully connected to the Bluetooth server.
        /// Provides <see cref="BluetoothDevice"/> of the server device.
        /// </summary>
        public static event Action<BluetoothDevice> ConnectedToServer;

        /// <summary>
        /// Fired when Bluetooth client failed to connect to the Bluetooth server.
        /// Provides <see cref="BluetoothDevice"/> of the server device.
        /// </summary>
        public static event Action<BluetoothDevice> ConnectionToServerFailed;

        /// <summary>
        /// Fired when Bluetooth client disconnected from the Bluetooth server.
        /// Provides <see cref="BluetoothDevice"/> of the server device.
        /// </summary>
        public static event Action<BluetoothDevice> DisconnectedFromServer;

        /// <summary>
        /// Fired on Bluetooth server when an incoming Bluetooth client connection was accepted.
        /// Provides <see cref="BluetoothDevice"/> of the connected client device.
        /// </summary>
        public static event Action<BluetoothDevice> ClientConnected;

        /// <summary>
        /// Fired on Bluetooth server when an Bluetooth client had disconnected.
        /// Provides <see cref="BluetoothDevice"/> of the disconnected client device.
        /// </summary>
        public static event Action<BluetoothDevice> ClientDisconnected;

        /// <summary>
        /// Fired when user selects a device in the device picker dialog.
        /// Provides <see cref="BluetoothDevice"/> of the picked device.
        /// </summary>
        public static event Action<BluetoothDevice> DevicePicked;

        /// <summary>
        /// Fired when Bluetooth discovery is actually started.
        /// </summary>
        public static event Action DiscoveryStarted;

        /// <summary>
        /// Fired when Bluetooth discovery is finished.
        /// </summary>
        public static event Action DiscoveryFinished;

        /// <summary>
        /// Fired when a new device was found during Bluetooth discovery procedure.
        /// Provides <see cref="BluetoothDevice"/> of the found device.
        /// </summary>
        public static event Action<BluetoothDevice> DeviceDiscovered;

        #endregion

        #region Event handlers

        // Fired when server is started and waiting for incoming connections
        private void JavaListeningStartedHandler(string empty) {
            ListeningStarted?.Invoke();
        }

        // Fired when listening for incoming connections 
        // was stopped by StopListening()
        private void JavaListeningStoppedHandler(string empty) {
            ListeningStopped?.Invoke();
        }

        // Fired when Bluetooth was enabled
        private void JavaAdapterEnabledHandler(string empty) {
            AdapterEnabled?.Invoke();
        }

        // Fired when request to enabled Bluetooth failed for some reason
        // (user did not authorized to enable Bluetooth or an error occured)
        private void JavaAdapterEnableFailedHandler(string empty) {
            AdapterEnableFailed?.Invoke();
        }

        // Fired when request to enabled Bluetooth discoverability failed for some reason
        // (user dismissed the request dialog or an error occured)
        private void JavaDiscoverabilityEnableFailedHandler(string empty) {
            DiscoverabilityEnableFailed?.Invoke();
        }

        // Fired when Bluetooth discoverability was enabled
        private void JavaDiscoverabilityEnabledHandler(string discoverabilityDurationString) {
            int discoverabilityDuration;
            if (!int.TryParse(discoverabilityDurationString, out discoverabilityDuration)) {
                discoverabilityDuration = 0;
            }

            DiscoverabilityEnabled?.Invoke(discoverabilityDuration);
        }

        // Fired when Bluetooth was disabled
        private void JavaAdapterDisabledHandler(string empty) {
            AdapterDisabled?.Invoke();
        }

        // Fired when Bluetooth client successfully connected to the Bluetooth server
        // Provides BluetoothDevice of server device
        private void JavaConnectedToServerHandler(string deviceAddress) {
            ConnectedToServer?.Invoke(GetDeviceFromAddress(deviceAddress));
        }

        // Fired when Bluetooth client failed to connect to the Bluetooth server
        // Provides BluetoothDevice of server device
        private void JavaConnectionToServerFailedHandler(string deviceAddress) {
            ConnectionToServerFailed?.Invoke(GetDeviceFromAddress(deviceAddress));
        }

        // Fired when Bluetooth client disconnected from the Bluetooth server
        // Provides BluetoothDevice of server device disconnected from
        private void JavaDisconnectedFromServerHandler(string deviceAddress) {
            DisconnectedFromServer?.Invoke(GetDeviceFromAddress(deviceAddress));
        }

        // Fired on Bluetooth server when an incoming Bluetooth client connection
        // was accepted
        // Provides BluetoothDevice of connected client device
        private void JavaClientConnectedHandler(string deviceAddress) {
            ClientConnected?.Invoke(GetDeviceFromAddress(deviceAddress));
        }

        // Fired on Bluetooth server when a Bluetooth client had disconnected
        // Provides BluetoothDevice of disconnected client device
        private void JavaClientDisconnectedHandler(string deviceAddress) {
            ClientDisconnected?.Invoke(GetDeviceFromAddress(deviceAddress));
        }

        // Fired when user selects a device in the device picker dialog. 
        // Provides BluetoothDevice of picked device
        private void JavaDevicePickedHandler(string deviceAddress) {
            DevicePicked?.Invoke(GetDeviceFromAddress(deviceAddress));
        }

        // Fired when Bluetooth discovery is actually started
        // after call to BluetoothMultiplayerAndroid.StartListening()
        private void JavaDiscoveryStartedHandler(string empty) {
            DiscoveryStarted?.Invoke();
        }

        // Fired when Bluetooth discovery is finished
        private void JavaDiscoveryFinishedHandler(string empty) {
            DiscoveryFinished?.Invoke();
        }

        // Fired when a new device was found during
        // Bluetooth discovery procedure
        private void JavaDeviceDiscoveredHandler(string deviceAddress) {
            DeviceDiscovered?.Invoke(GetDeviceFromAddress(deviceAddress));
        }

        #endregion Event handlers
    }
}

#endif