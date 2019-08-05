using UnityEngine;

namespace LostPolygon.AndroidBluetoothMultiplayer.Examples {
    public class BluetoothDemoMenu : MonoBehaviour {

        protected virtual void OnEnable() {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        }

        protected virtual void OnDisable() {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
        }

        private void SceneManagerOnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode) {
            SceneLoadedHandler(scene.buildIndex);
        }

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