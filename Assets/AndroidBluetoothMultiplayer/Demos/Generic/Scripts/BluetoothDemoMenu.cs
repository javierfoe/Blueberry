using UnityEngine;

namespace LostPolygon.AndroidBluetoothMultiplayer.Examples {
    public class BluetoothDemoMenu : MonoBehaviour {
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
        protected void OnLevelWasLoaded(int level) {
            SceneLoadedHandler(level);
        }
#else
        protected virtual void OnEnable() {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        }

        protected virtual void OnDisable() {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
        }

        private void SceneManagerOnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode) {
            SceneLoadedHandler(scene.buildIndex);
        }
#endif

        protected virtual void SceneLoadedHandler(int buildIndex) {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            CameraFade.StartAlphaFade(Color.black, true, 0.25f, 0.0f);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape))
                QuitApplication();
        }

        private void QuitApplication() {
            CameraFade.StartAlphaFade(Color.black, false, 0.25f, 0f, Application.Quit);
        }

        public void LoadLevel(string levelName) {
            CameraFade.StartAlphaFade(Color.black, false, 0.25f, 0f, () => BluetoothExamplesTools.LoadLevel(levelName));
        }
    }
}