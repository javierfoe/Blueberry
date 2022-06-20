using UnityEngine;
using Mirror;

namespace javierfoe.Blueberry
{
    [RequireComponent(typeof(BlueberryHelper))]
    public class BlueberryHUD : MonoBehaviour
    {
        public int offsetX;
        public int offsetY;
        
        private BlueberryHelper _manager;

        void Awake()
        {
            _manager = GetComponent<BlueberryHelper>();
        }

        void Start()
        {
            if (!_manager.IsInitialized)
                Debug.LogError("BlueberryNetworkManagerHelper is not initialized. HUD is not shown.", _manager.gameObject);
        }
        
        void OnGUI()
        {
            if (!_manager.IsInitialized)
                return;

            GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, 215, 9999));

            BluetoothMultiplayerMode currentMode = Blueberry.GetCurrentMode();
            if (currentMode == BluetoothMultiplayerMode.None)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
            }

            // client ready
            if (NetworkClient.isConnected && !NetworkClient.ready)
            {
                if (GUILayout.Button("Client Ready"))
                {
                    NetworkClient.Ready();
                    if (NetworkClient.localPlayer == null)
                    {
                        NetworkClient.AddPlayer();
                    }
                }
            }

            GUILayout.EndArea();
        }

        void StartButtons()
        {
            if (!NetworkClient.active)
            {
                // Host
                if (GUILayout.Button("Host (Server + Client)", GUILayout.Height(100)))
                {
                    _manager.StartHost();
                }

                // Client
                if (GUILayout.Button("Client", GUILayout.Height(100)))
                {
                    _manager.StartClient();
                }

                // Server Only
                if (GUILayout.Button("Server Only", GUILayout.Height(100)))
                {
                    _manager.StartServer();
                }
            }
            else
            {
                // Connecting
                GUILayout.Label($"Connecting ..");
                if (GUILayout.Button("Cancel Connection Attempt"))
                {
                    _manager.StopHost();
                }
            }
        }

        void StatusLabels()
        {
            if (NetworkServer.active)
            {
                //HOST
                if (NetworkClient.active)
                    GUILayout.Label($"<b>Host</b>: running via {Transport.activeTransport}");
                //SERVER
                else
                    GUILayout.Label($"<b>Server</b>: running via {Transport.activeTransport}");

                StopButton(true);
            }
            //CLIENT
            else if (Blueberry.GetCurrentMode() == BluetoothMultiplayerMode.Client)
            {
                bool bluetoothClient = _manager.IsBluetoothClientConnected;
                string label = $"<b>Client</b>: connect{(bluetoothClient ? "ed" : "ing")} to {_manager.ServerDevice} via {Transport.activeTransport}";
                GUILayout.Label(label);
                StopButton(bluetoothClient);
            }
        }

        void StopButton(bool stop)
        {
            string label = stop ? "Stop" : "Cancel";
            if (GUILayout.Button(label, GUILayout.Height(100)))
            {
                _manager.StopHost();
            }
        }
    }
}
