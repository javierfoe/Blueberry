using System;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Blueberry.Hub
{
    public class Hub : MonoBehaviour
    {
        [Serializable]
        private class ButtonScene
        {
            public Button button;
            [Scene]
            public string scene;
        }
        
        [SerializeField]
        private ButtonScene[] buttonScenes;

        private void Awake()
        {
            foreach (var buttonScene in buttonScenes)
            {
                var scene = buttonScene.scene;
                var button = buttonScene.button;
                button.onClick.AddListener(() => LoadScene(scene));
            }
        }

        private void LoadScene(string scene)
        {
            SceneManager.LoadScene(scene);
        }
    }
}