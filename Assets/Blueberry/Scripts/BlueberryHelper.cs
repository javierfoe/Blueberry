using System;
using UnityEngine;
using Mirror;

namespace Blueberry
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
        protected NetworkManager networkManager;

        [SerializeField]
        protected BlueberrySettings blueberrySettings = new();

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
        public bool IsInitialized { get; private set; }

        private ushort Port => (ushort)_transport.port;

        protected virtual void Awake()
        {
            networkManager = GetComponent<NetworkManager>();
            _transport = GetComponent<IgnoranceTransport.Ignorance>();
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            // When BlueberryHUD is set replace it with NetworkManagerHUD
            BlueberryHUD hud = gameObject.GetComponent<BlueberryHUD>();
            if (hud != null)
            {
                Destroy(hud);
                gameObject.AddComponent<NetworkManagerHUD>();
            }
#elif !UNITY_ANDROID
            Debug.LogError("Not compatible target platform.");
#endif
        }

        protected virtual void OnEnable()
        {
#if UNITY_ANDROID
            // Setting the UUID. Must be unique for every application
            IsInitialized = Blueberry.Initialize(blueberrySettings.Uuid);
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
#endif
        }

        protected virtual void OnDisable()
        {

#if UNITY_ANDROID
            // Stopping all Bluetooth connectivity on Unity networking disconnect event
            Blueberry.Stop();
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
#endif
        }
        #region NetworkManager methods

        // Call StartClient, StartServer and StartHost directly when on Editor or Windows Standalone 
        /// <seealso cref="NetworkManager.StartClient()"/>
        public void StartClient()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            networkManager.StartClient();
#elif UNITY_ANDROID
            StartBluetoothClient(networkManager.StartClient);
#endif
        }

        /// <seealso cref="NetworkManager.StartServer()"/>
        public void StartServer()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            networkManager.StartServer();
#elif UNITY_ANDROID
            StartBluetoothHost(networkManager.StartServer);
#endif
        }

        /// <seealso cref="NetworkManager.StartHost()"/>
        public void StartHost()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            networkManager.StartHost();
#elif UNITY_ANDROID
            StartBluetoothHost(networkManager.StartHost);
#endif
        }

        public void StopHost()
        {
            networkManager.StopHost();
#if UNITY_ANDROID
            Blueberry.StopDiscovery();
            Blueberry.Stop();
#endif
        }

        #endregion

        #region Bluetooth events

        protected virtual void OnBluetoothListeningStarted()
        {
            if (blueberrySettings.LogBluetoothEvents)
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
            if (blueberrySettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - ListeningStopped");
            }

            if (blueberrySettings.StopBluetoothServerOnListeningStopped)
            {
                Blueberry.Stop();
            }
        }

        protected virtual void OnBluetoothDevicePicked(BluetoothDevice device)
        {
            this._device = device;
            if (blueberrySettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - DevicePicked: " + device);
            }

            // Trying to connect to the device picked by user
            Blueberry.Connect(device.Address, Port);
        }

        //Called on the server when a client disconnects
        protected virtual void OnBluetoothServerDisconnected(BluetoothDevice device)
        {
            if (blueberrySettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - ClientDisconnected: " + device);
            }
        }

        //Called on the server when a client is connected
        protected virtual void OnBluetoothServerConnected(BluetoothDevice device)
        {
            if (blueberrySettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - ClientConnected: " + device);
            }
        }

        //Called on the client when disconnected from the server
        protected virtual void OnBluetoothClientDisconnected(BluetoothDevice device)
        {
            IsBluetoothClientConnected = false;
            this._device = null;
            if (blueberrySettings.LogBluetoothEvents)
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
            if (blueberrySettings.LogBluetoothEvents)
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
            if (blueberrySettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - ConnectionToServerFailed: " + device);
            }
        }

        protected virtual void OnBluetoothAdapterDisabled()
        {
            if (blueberrySettings.LogBluetoothEvents)
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
            if (blueberrySettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - AdapterEnableFailed");
            }
        }

        protected virtual void OnBluetoothAdapterEnabled()
        {
            if (blueberrySettings.LogBluetoothEvents)
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
            if (blueberrySettings.LogBluetoothEvents)
            {
                Debug.Log("Bluetooth Event - DiscoverabilityEnableFailed");
            }
        }

        protected virtual void OnBluetoothDiscoverabilityEnabled(int discoverabilityDuration)
        {
            if (blueberrySettings.LogBluetoothEvents)
            {
                Debug.Log($"Event - DiscoverabilityEnabled: {discoverabilityDuration} seconds");
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
                Blueberry.RequestEnableDiscoverability(blueberrySettings.DefaultBluetoothDiscoverabilityInterval);
                StopAll(); // Just to be sure
                Blueberry.StartServer(Port);
            }
            else
            {
                // Otherwise, we have to enable Bluetooth first and wait for callback
                _desiredMode = BluetoothMultiplayerMode.Server;
                Blueberry.RequestEnableDiscoverability(blueberrySettings.DefaultBluetoothDiscoverabilityInterval);
            }
        }

        private void StopAll()
        {
            networkManager.StopHost();
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
            if (String.IsNullOrEmpty(blueberrySettings.Uuid))
            {
                blueberrySettings.Uuid = Guid.NewGuid().ToString();
            }
        }
#endif
    }
}