using System;
using UnityEngine;
using Mirror;

namespace LostPolygon.AndroidBluetoothMultiplayer
{
    /// <summary>
    /// A helper class that works in conjunction with <see cref="NetworkManager"/>.
    /// It automatically manages enabling Bluetooth, showing the device picker,
    /// and otherwise correctly handling the Bluetooth session.
    /// </summary>
    /// <example>
    /// The NetworkManager.Start* family of methods is mirrored, just use this class
    /// instead of using NetworkManager directly to start your client/server/host.
    /// </example>
    [RequireComponent(typeof(NetworkManager))]
    [AddComponentMenu("Network/Android Bluetooth Multiplayer/AndroidBluetoothNetworkManagerHelper")]
    public class AndroidBluetoothNetworkManagerHelper : MonoBehaviour
    {

        [SerializeField]
        [HideInInspector]
        protected NetworkManager _networkManager;

        private IgnoranceTransport.Ignorance _transportLayer;

        [SerializeField]
        protected BluetoothNetworkManagerSettings _bluetoothNetworkManagerSettings = new BluetoothNetworkManagerSettings();

        private bool _isInitialized;
        private BluetoothMultiplayerMode _desiredMode = BluetoothMultiplayerMode.None;
        private Action _clientAction;
        private Action _hostAction;

        /// <summary>
        /// Gets a value indicating whether the plugin has initialized successfully.
        /// </summary>
        public bool IsInitialized
        {
            get { return _isInitialized; }
        }

        protected virtual void OnEnable()
        {
            _networkManager = GetComponent<NetworkManager>();
            _transportLayer = GetComponent<IgnoranceTransport.Ignorance>();

            // Setting the UUID. Must be unique for every application
            _isInitialized = AndroidBluetoothMultiplayer.Initialize(_bluetoothNetworkManagerSettings.Uuid);

            // Registering the event listeners
            AndroidBluetoothMultiplayer.ListeningStarted += OnBluetoothListeningStarted;
            AndroidBluetoothMultiplayer.ListeningStopped += OnBluetoothListeningStopped;
            AndroidBluetoothMultiplayer.AdapterEnabled += OnBluetoothAdapterEnabled;
            AndroidBluetoothMultiplayer.AdapterEnableFailed += OnBluetoothAdapterEnableFailed;
            AndroidBluetoothMultiplayer.AdapterDisabled += OnBluetoothAdapterDisabled;
            AndroidBluetoothMultiplayer.DiscoverabilityEnabled += OnBluetoothDiscoverabilityEnabled;
            AndroidBluetoothMultiplayer.DiscoverabilityEnableFailed += OnBluetoothDiscoverabilityEnableFailed;
            AndroidBluetoothMultiplayer.ConnectedToServer += OnBluetoothConnectedToServer;
            AndroidBluetoothMultiplayer.ConnectionToServerFailed += OnBluetoothConnectionToServerFailed;
            AndroidBluetoothMultiplayer.DisconnectedFromServer += OnBluetoothDisconnectedFromServer;
            AndroidBluetoothMultiplayer.ClientConnected += OnBluetoothClientConnected;
            AndroidBluetoothMultiplayer.ClientDisconnected += OnBluetoothClientDisconnected;
            AndroidBluetoothMultiplayer.DevicePicked += OnBluetoothDevicePicked;
        }

        protected virtual void OnDisable()
        {

#if UNITY_ANDROID
            // Stopping all Bluetooth connectivity on Unity networking disconnect event
            AndroidBluetoothMultiplayer.Stop();
#endif
            // Unregistering the event listeners
            AndroidBluetoothMultiplayer.ListeningStarted -= OnBluetoothListeningStarted;
            AndroidBluetoothMultiplayer.ListeningStopped -= OnBluetoothListeningStopped;
            AndroidBluetoothMultiplayer.AdapterEnabled -= OnBluetoothAdapterEnabled;
            AndroidBluetoothMultiplayer.AdapterEnableFailed -= OnBluetoothAdapterEnableFailed;
            AndroidBluetoothMultiplayer.AdapterDisabled -= OnBluetoothAdapterDisabled;
            AndroidBluetoothMultiplayer.DiscoverabilityEnabled -= OnBluetoothDiscoverabilityEnabled;
            AndroidBluetoothMultiplayer.DiscoverabilityEnableFailed -= OnBluetoothDiscoverabilityEnableFailed;
            AndroidBluetoothMultiplayer.ConnectedToServer -= OnBluetoothConnectedToServer;
            AndroidBluetoothMultiplayer.ConnectionToServerFailed -= OnBluetoothConnectionToServerFailed;
            AndroidBluetoothMultiplayer.DisconnectedFromServer -= OnBluetoothDisconnectedFromServer;
            AndroidBluetoothMultiplayer.ClientConnected -= OnBluetoothClientConnected;
            AndroidBluetoothMultiplayer.ClientDisconnected -= OnBluetoothClientDisconnected;
            AndroidBluetoothMultiplayer.DevicePicked -= OnBluetoothDevicePicked;
        }
        #region NetworkManager methods

        /// <seealso cref="NetworkManager.StartClient()"/>
        public void StartClient()
        {
            StartBluetoothClient(_networkManager.StartClient);
        }

        /// <seealso cref="NetworkManager.StartServer()"/>
        public void StartServer()
        {
            StartBluetoothHost(_networkManager.StartServer);
        }

        /// <seealso cref="NetworkManager.StartHost()"/>
        public void StartHost()
        {
            StartBluetoothHost(_networkManager.StartHost);
        }

        public void StopHost()
        {
            _networkManager.StopHost();
            AndroidBluetoothMultiplayer.StopDiscovery();
            AndroidBluetoothMultiplayer.Stop();
        }

        #endregion

        #region Bluetooth events

        protected virtual void OnBluetoothListeningStarted()
        {
            if (_bluetoothNetworkManagerSettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - ListeningStarted");
            }

            // Starting networking server if Bluetooth listening started successfully
            if (_hostAction != null)
            {
                _hostAction();
                _hostAction = null;
            }
        }

        protected virtual void OnBluetoothListeningStopped()
        {
            if (_bluetoothNetworkManagerSettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - ListeningStopped");
            }

            if (_bluetoothNetworkManagerSettings.StopBluetoothServerOnListeningStopped)
            {
                AndroidBluetoothMultiplayer.Stop();
            }
        }

        protected virtual void OnBluetoothDevicePicked(BluetoothDevice device)
        {
            if (_bluetoothNetworkManagerSettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - DevicePicked: " + device);
            }

            // Trying to connect to the device picked by user
            AndroidBluetoothMultiplayer.Connect(device.Address, (ushort)_transportLayer.port);
        }

        protected virtual void OnBluetoothClientDisconnected(BluetoothDevice device)
        {
            if (_bluetoothNetworkManagerSettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - ClientDisconnected: " + device);
            }
        }

        protected virtual void OnBluetoothClientConnected(BluetoothDevice device)
        {
            if (_bluetoothNetworkManagerSettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - ClientConnected: " + device);
            }
        }

        protected virtual void OnBluetoothDisconnectedFromServer(BluetoothDevice device)
        {
            if (_bluetoothNetworkManagerSettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - DisconnectedFromServer: " + device);
            }

            // Stop networking on Bluetooth failure
            StopAll();
            ClearState();
        }

        protected virtual void OnBluetoothConnectionToServerFailed(BluetoothDevice device)
        {
            if (_bluetoothNetworkManagerSettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - ConnectionToServerFailed: " + device);
            }
        }

        protected virtual void OnBluetoothConnectedToServer(BluetoothDevice device)
        {
            if (_bluetoothNetworkManagerSettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - ConnectedToServer: " + device);
            }

            // Trying to negotiate a Unity networking connection,
            // when Bluetooth client connected successfully
            if (_clientAction != null)
            {
                _clientAction();
                _clientAction = null;
            }
        }

        protected virtual void OnBluetoothAdapterDisabled()
        {
            if (_bluetoothNetworkManagerSettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - AdapterDisabled");
            }

            if (NetworkServer.active)
            {
                StopAll();
                ClearState();
            }
        }

        protected virtual void OnBluetoothAdapterEnableFailed()
        {
            if (_bluetoothNetworkManagerSettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - AdapterEnableFailed");
            }
        }

        protected virtual void OnBluetoothAdapterEnabled()
        {
            if (_bluetoothNetworkManagerSettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - AdapterEnabled");
            }

            // Resuming desired action after enabling the adapter
            switch (_desiredMode)
            {
                case BluetoothMultiplayerMode.Server:
                    StopAll();
                    AndroidBluetoothMultiplayer.StartServer((ushort)_transportLayer.port);
                    break;
                case BluetoothMultiplayerMode.Client:
                    StopAll();
                    // Open device picker dialog
                    AndroidBluetoothMultiplayer.ShowDeviceList();
                    break;
            }

            _desiredMode = BluetoothMultiplayerMode.None;
        }

        protected virtual void OnBluetoothDiscoverabilityEnableFailed()
        {
            if (_bluetoothNetworkManagerSettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - DiscoverabilityEnableFailed");
            }
        }

        protected virtual void OnBluetoothDiscoverabilityEnabled(int discoverabilityDuration)
        {
            if (_bluetoothNetworkManagerSettings.LogBluetoothEvents)
            {
                Debug.Log(string.Format("Event - DiscoverabilityEnabled: {0} seconds", discoverabilityDuration));
            }
        }

        #endregion

        private void StartBluetoothClient(Action onReadyAction)
        {
            _clientAction = onReadyAction;

            // If Bluetooth is enabled, immediately open the device picker
            if (AndroidBluetoothMultiplayer.GetIsBluetoothEnabled())
            {
                StopAll();
                // Open device picker dialog
                AndroidBluetoothMultiplayer.ShowDeviceList();
            }
            else
            {
                // Otherwise, we have to enable Bluetooth first and wait for callback
                _desiredMode = BluetoothMultiplayerMode.Client;
                AndroidBluetoothMultiplayer.RequestEnableBluetooth();
            }
        }

        private void StartBluetoothHost(Action onReadyAction)
        {
            _hostAction = onReadyAction;

            // If Bluetooth is enabled, immediately start the Bluetooth server
            if (AndroidBluetoothMultiplayer.GetIsBluetoothEnabled())
            {
                AndroidBluetoothMultiplayer.RequestEnableDiscoverability(_bluetoothNetworkManagerSettings.DefaultBluetoothDiscoverabilityInterval);
                StopAll(); // Just to be sure
                AndroidBluetoothMultiplayer.StartServer((ushort)_transportLayer.port);
            }
            else
            {
                // Otherwise, we have to enable Bluetooth first and wait for callback
                _desiredMode = BluetoothMultiplayerMode.Server;
                AndroidBluetoothMultiplayer.RequestEnableDiscoverability(_bluetoothNetworkManagerSettings.DefaultBluetoothDiscoverabilityInterval);
            }
        }

        private void StopAll()
        {
            _networkManager.StopHost();
            AndroidBluetoothMultiplayer.Stop();
        }

        private void ClearState()
        {
            _desiredMode = BluetoothMultiplayerMode.None;
            _clientAction = null;
            _hostAction = null;
        }

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            OnValidate();
        }

        protected virtual void OnValidate()
        {
            if (String.IsNullOrEmpty(_bluetoothNetworkManagerSettings.Uuid))
            {
                _bluetoothNetworkManagerSettings.Uuid = Guid.NewGuid().ToString();
            }
        }
#endif
    }
}