using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace LostPolygon.AndroidBluetoothMultiplayer.Examples.UNet
{
    public class BluetoothMultiplayerDemoGui : MonoBehaviour
    {
        public AndroidBluetoothNetworkManagerHelper AndroidBluetoothNetworkManagerHelper;

        public GameObject UIPanelGameObject;
        public GameObject ErrorUIPanelGameObject;

        public GameObject StartServerButtonGameObject;
        public GameObject ConnectToServerButtonGameObject;
        public GameObject DisconnectButtonGameObject;

        protected virtual void OnEnable()
        {
#if !UNITY_ANDROID
            UIPanelGameObject.SetActive(false);
            ErrorUIPanelGameObject.SetActive(true);
#endif
        }

        private void Update()
        {
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
        }

        private void Awake()
        {
#if !UNITY_ANDROID
            Debug.LogError("Build platform is not set to Android. Please choose Android as build Platform in File - Build Settings...");
#endif
        }

        #region UI Handlers

        public void OnStartServerButton()
        {
            AndroidBluetoothNetworkManagerHelper.StartHost();
        }

        public void OnConnectToServerButton()
        {
            AndroidBluetoothNetworkManagerHelper.StartClient();
        }

        public void OnDisconnectButton()
        {
            AndroidBluetoothNetworkManagerHelper.StopHost();
        }

        #endregion
    }
}