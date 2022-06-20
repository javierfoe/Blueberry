using System;
using kcp2k;
using UnityEngine;
using Mirror;

namespace javierfoe.Blueberry
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
    public class BlueberryHelper : MonoBehaviour
    {

        [SerializeField]
        [HideInInspector]
        protected NetworkManager _networkManager;

        [SerializeField]
        protected BlueberrySettings _bluetoothNetworkManagerSettings = new BlueberrySettings();

        private bool _isInitialized;
        private BluetoothMultiplayerMode _desiredMode = BluetoothMultiplayerMode.None;
        private Action _clientAction;
        private Action _hostAction;
        private BluetoothDevice _device;
        private IgnoranceTransport.Ignorance _transport;

        public string ServerDevice => _device != null ? _device.Name : "";
        public bool IsBluetoothClientConnected { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the plugin has initialized successfully.
        /// </summary>
        public bool IsInitialized
        {
            get { return _isInitialized; }
        }

        private ushort Port => (ushort)_transport.port;

        protected virtual void Awake()
        {
            _networkManager = GetComponent<NetworkManager>();
            _transport = GetComponent<IgnoranceTransport.Ignorance>();
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            // When BlueberryHUD is set replace it with NetworkManagerHUD
            BlueberryHUD hud = gameObject.GetComponent<BlueberryHUD>();
            if (hud != null)
            {
                Destroy(hud);
                gameObject.AddComponent<NetworkManagerHUD>();
            }
            // Destroy BlueberryNetworkManagerHelper
            Destroy(this);
#elif !UNITY_ANDROID
            Debug.LogError("Not compatible target platform.");
#endif
        }

        protected virtual void OnEnable()
        {
            // Setting the UUID. Must be unique for every application
            _isInitialized = Blueberry.Initialize(_bluetoothNetworkManagerSettings.Uuid);
            // Registering the event listeners
            Blueberry.ListeningStarted += OnBluetoothListeningStarted;
            Blueberry.ListeningStopped += OnBluetoothListeningStopped;
            Blueberry.AdapterEnabled += OnBluetoothAdapterEnabled;
            Blueberry.AdapterEnableFailed += OnBluetoothAdapterEnableFailed;
            Blueberry.AdapterDisabled += OnBluetoothAdapterDisabled;
            Blueberry.DiscoverabilityEnabled += OnBluetoothDiscoverabilityEnabled;
            Blueberry.DiscoverabilityEnableFailed += OnBluetoothDiscoverabilityEnableFailed;
            Blueberry.ConnectedToServer += OnBluetoothClientConnected;
            Blueberry.ConnectionToServerFailed += OnBluetoothConnectionToServerFailed;
            Blueberry.DisconnectedFromServer += OnBluetoothClientDisconnected;
            Blueberry.ClientConnected += OnBluetoothServerConnected;
            Blueberry.ClientDisconnected += OnBluetoothServerDisconnected;
            Blueberry.DevicePicked += OnBluetoothDevicePicked;
        }

        protected virtual void OnDisable()
        {

#if UNITY_ANDROID
            // Stopping all Bluetooth connectivity on Unity networking disconnect event
            Blueberry.Stop();
#endif
            // Unregistering the event listeners
            Blueberry.ListeningStarted -= OnBluetoothListeningStarted;
            Blueberry.ListeningStopped -= OnBluetoothListeningStopped;
            Blueberry.AdapterEnabled -= OnBluetoothAdapterEnabled;
            Blueberry.AdapterEnableFailed -= OnBluetoothAdapterEnableFailed;
            Blueberry.AdapterDisabled -= OnBluetoothAdapterDisabled;
            Blueberry.DiscoverabilityEnabled -= OnBluetoothDiscoverabilityEnabled;
            Blueberry.DiscoverabilityEnableFailed -= OnBluetoothDiscoverabilityEnableFailed;
            Blueberry.ConnectedToServer -= OnBluetoothClientConnected;
            Blueberry.ConnectionToServerFailed -= OnBluetoothConnectionToServerFailed;
            Blueberry.DisconnectedFromServer -= OnBluetoothClientDisconnected;
            Blueberry.ClientConnected -= OnBluetoothServerConnected;
            Blueberry.ClientDisconnected -= OnBluetoothServerDisconnected;
            Blueberry.DevicePicked -= OnBluetoothDevicePicked;
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
            Blueberry.StopDiscovery();
            Blueberry.Stop();
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
                Blueberry.Stop();
            }
        }

        protected virtual void OnBluetoothDevicePicked(BluetoothDevice device)
        {
            this._device = device;
            if (_bluetoothNetworkManagerSettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - DevicePicked: " + device);
            }

            // Trying to connect to the device picked by user
            Blueberry.Connect(device.Address, Port);
        }

        //Called on the server when a client disconnects
        protected virtual void OnBluetoothServerDisconnected(BluetoothDevice device)
        {
            if (_bluetoothNetworkManagerSettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - ClientDisconnected: " + device);
            }
        }

        //Called on the server when a client is connected
        protected virtual void OnBluetoothServerConnected(BluetoothDevice device)
        {
            if (_bluetoothNetworkManagerSettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - ClientConnected: " + device);
            }
        }

        //Called on the client when disconnected from the server
        protected virtual void OnBluetoothClientDisconnected(BluetoothDevice device)
        {
            IsBluetoothClientConnected = false;
            this._device = null;
            if (_bluetoothNetworkManagerSettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - DisconnectedFromServer: " + device);
            }

            // Stop networking on Bluetooth failure
            StopAll();
            ClearState();
        }

        //Called on the client when connected to the server
        protected virtual void OnBluetoothClientConnected(BluetoothDevice device)
        {
            IsBluetoothClientConnected = true;
            this._device = device;
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

        protected virtual void OnBluetoothConnectionToServerFailed(BluetoothDevice device)
        {
            IsBluetoothClientConnected = false;
            this._device = null;
            if (_bluetoothNetworkManagerSettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - ConnectionToServerFailed: " + device);
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
                    Blueberry.StartServer(Port);
                    break;
                case BluetoothMultiplayerMode.Client:
                    StopAll();
                    // Open device picker dialog
                    Blueberry.ShowDeviceList();
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
            if (Blueberry.GetIsBluetoothEnabled())
            {
                StopAll();
                // Open device picker dialog
                Blueberry.ShowDeviceList();
            }
            else
            {
                // Otherwise, we have to enable Bluetooth first and wait for callback
                _desiredMode = BluetoothMultiplayerMode.Client;
                Blueberry.RequestEnableBluetooth();
            }
        }

        private void StartBluetoothHost(Action onReadyAction)
        {
            _hostAction = onReadyAction;

            // If Bluetooth is enabled, immediately start the Bluetooth server
            if (Blueberry.GetIsBluetoothEnabled())
            {
                Blueberry.RequestEnableDiscoverability(_bluetoothNetworkManagerSettings.DefaultBluetoothDiscoverabilityInterval);
                StopAll(); // Just to be sure
                Blueberry.StartServer(Port);
            }
            else
            {
                // Otherwise, we have to enable Bluetooth first and wait for callback
                _desiredMode = BluetoothMultiplayerMode.Server;
                Blueberry.RequestEnableDiscoverability(_bluetoothNetworkManagerSettings.DefaultBluetoothDiscoverabilityInterval);
            }
        }

        private void StopAll()
        {
            _networkManager.StopHost();
            Blueberry.Stop();
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