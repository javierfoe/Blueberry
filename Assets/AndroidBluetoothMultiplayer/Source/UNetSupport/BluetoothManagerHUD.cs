using UnityEngine;
using Mirror;

namespace javierfoe.AndroidBluetoothMultiplayer
{
    [RequireComponent(typeof(AndroidBluetoothNetworkManagerHelper))]
    public class BluetoothManagerHUD : MonoBehaviour
    {
        AndroidBluetoothNetworkManagerHelper manager;

        public int offsetX;
        public int offsetY;

        void Awake()
        {
            manager = GetComponent<AndroidBluetoothNetworkManagerHelper>();
        }

        void OnGUI()
        {
            if (!manager.IsInitialized) return;

            GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, 215, 9999));

            BluetoothMultiplayerMode currentMode = AndroidBluetoothMultiplayer.GetCurrentMode();
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

            StopButtons();

            GUILayout.EndArea();
        }

        void StartButtons()
        {
            if (!NetworkClient.active)
            {
                // Host
                if (GUILayout.Button("Host (Server + Client)", GUILayout.Height(100)))
                {
                    manager.StartHost();
                }

                // Client
                if (GUILayout.Button("Client", GUILayout.Height(100)))
                {
                    manager.StartClient();
                }

                // Server Only
                if (GUILayout.Button("Server Only", GUILayout.Height(100)))
                {
                    manager.StartServer();
                }
            }
            else
            {
                // Connecting
                GUILayout.Label($"Connecting ..");
                if (GUILayout.Button("Cancel Connection Attempt"))
                {
                    manager.StopHost();
                }
            }
        }

        void StatusLabels()
        {
            // host mode
            // display separately because this always confused people:
            //   Server: ...
            //   Client: ...
            if (NetworkServer.active && NetworkClient.active)
            {
                GUILayout.Label($"<b>Host</b>: running via {Transport.activeTransport}");
            }
            // server only
            else if (NetworkServer.active)
            {
                GUILayout.Label($"<b>Server</b>: running via {Transport.activeTransport}");
            }
            // client only
            else if (NetworkClient.isConnected)
            {
                GUILayout.Label($"<b>Client</b>: connected to {manager.ConnectedTo} via {Transport.activeTransport}");
            }
        }

        void StopButtons()
        {
            BluetoothMultiplayerMode currentMode = AndroidBluetoothMultiplayer.GetCurrentMode();
            if (currentMode != BluetoothMultiplayerMode.None)
            {
                if (GUILayout.Button("Stop", GUILayout.Height(100)))
                {
                    manager.StopHost();
                }
            }
        }
    }
}
