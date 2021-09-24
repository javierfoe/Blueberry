using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace LostPolygon.AndroidBluetoothMultiplayer.Examples.UNet
{
    public class BluetoothMultiplayerDemoGui : MonoBehaviour
    {
        public GameObject TapMarkerPrefab;

        public AndroidBluetoothNetworkManagerHelper AndroidBluetoothNetworkManagerHelper;
        public NetworkManagerDemo NetworkManagerDemo;

        public DeviceBrowserController CustomDeviceBrowser;

        public GameObject UIPanelGameObject;
        public GameObject ErrorUIPanelGameObject;

        public GameObject StartServerButtonGameObject;
        public GameObject ConnectToServerButtonGameObject;
        public GameObject DisconnectButtonGameObject;

        public Toggle StressTestToggle;
        public Toggle UseCustomDeviceBrowserUIToggle;

        protected virtual void OnEnable()
        {
#if !UNITY_ANDROID
            UIPanelGameObject.SetActive(false);
            ErrorUIPanelGameObject.SetActive(true);
#endif
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        }

        protected virtual void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
        }

        private void SceneManagerOnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
        {
            SceneLoadedHandler(scene.buildIndex);
        }

        protected virtual void SceneLoadedHandler(int buildIndex)
        {
            Screen.sleepTimeout = 500;
            CameraFade.StartAlphaFade(Color.black, true, 0.25f, 0.0f);
        }

        protected virtual void OnDestroy()
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }

        protected void DrawBackButton(float scaleFactor)
        {
            GUI.contentColor = Color.white;
            if (GUI.Button(new Rect(Screen.width / scaleFactor - (100f + 15f), Screen.height / scaleFactor - (40f + 15f), 100f, 40f), "Back"))
            {
                GoBackToMenu();
            }
        }

        protected void GoBackToMenu()
        {
#if UNITY_ANDROID
            OnGoingBackToMenu();
#endif
            CameraFade.StartAlphaFade(Color.black, false, 0.25f, 0f, () => LoadLevel("BluetoothDemoMenu"));
        }

        private void LoadLevel(string name)
        {
            SceneManager.LoadScene(name, LoadSceneMode.Single);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GoBackToMenu();
            }

            // Spawn an effect where user has tapped
            if (NetworkClient.active && Input.GetMouseButtonDown(0))
            {
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
            if (DisconnectButtonGameObject.activeInHierarchy)
            {
                DisconnectButtonGameObject.GetComponentInChildren<Text>().text = currentMode == BluetoothMultiplayerMode.Client ? "Disconnect" : "Stop server";
            }

            bool togglesInteractable = currentMode == BluetoothMultiplayerMode.None;
            bool togglesActive = currentMode != BluetoothMultiplayerMode.Client;

            StressTestToggle.interactable = togglesInteractable;
            StressTestToggle.gameObject.SetActive(togglesActive);

            UseCustomDeviceBrowserUIToggle.interactable = togglesInteractable;
            UseCustomDeviceBrowserUIToggle.gameObject.SetActive(togglesActive);

            // Update values
            NetworkManagerDemo.StressTestMode = StressTestToggle.isOn;
        }

        private void Awake()
        {

#if !UNITY_ANDROID
            Debug.LogError("Build platform is not set to Android. Please choose Android as build Platform in File - Build Settings...");
#endif
            // Enabling verbose logging. See logcat!
            AndroidBluetoothMultiplayer.SetVerboseLog(true);
        }

        private void OnGoingBackToMenu()
        {
            // Gracefully closing all Bluetooth connectivity and loading the menu
            try
            {
                NetworkManagerDemo.StopHost();
                AndroidBluetoothMultiplayer.StopDiscovery();
                AndroidBluetoothMultiplayer.Stop();
            }
            catch
            {
                //
            }
        }

        #region UI Handlers

        public void OnBackToMenuButton()
        {
            GoBackToMenu();
        }

        public void OnStartServerButton()
        {
            AndroidBluetoothNetworkManagerHelper.StartHost();
        }

        public void OnConnectToServerButton()
        {
#if UNITY_ANDROID
            AndroidBluetoothNetworkManagerHelper.SetCustomDeviceBrowser(UseCustomDeviceBrowserUIToggle.isOn ? CustomDeviceBrowser : null);
#endif
            AndroidBluetoothNetworkManagerHelper.StartClient();
        }

        public void OnDisconnectButton()
        {
            AndroidBluetoothMultiplayer.StopDiscovery();
            AndroidBluetoothMultiplayer.Stop();
            NetworkManagerDemo.StopHost();
        }

        #endregion
    }
}