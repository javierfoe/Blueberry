using System;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace javierfoe.Blueberry.Hub
{
    public class Hub : MonoBehaviour
    {
        [Serializable]
        public class ButtonScene
        {
            public Button button;
            [Scene]
            public string scene;
        }
        
        [SerializeField]
        private ButtonScene[] buttonScenes = null;

        private void Awake()
        {
            Button button;
            for (int i = 0; i < buttonScenes.Length; i++)
            {
                string scene = buttonScenes[i].scene;
                button = buttonScenes[i].button;
                button.onClick.AddListener(() => LoadScene(scene));
            }
        }

        private void LoadScene(string scene)
        {
            SceneManager.LoadScene(scene);
        }
    }
}