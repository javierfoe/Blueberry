using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace javierfoe.Blueberry.Hub
{
    public class AndroidDemo : MonoBehaviour
    {
        [SerializeField]
        private Button buttonPrefab;

        [SerializeField]
        [Scene]
        private string[] scenes = null;

        private void Awake()
        {
            int length = scenes.Length;
            string[] split;
            Button button;
            for (int i = 0; i < length; i++)
            {
                string scene = scenes[i];
                split = scene.Split('/', '.');
                string sceneName = split[split.Length - 2];
                button = Instantiate(buttonPrefab, transform.GetChild(0));
                button.GetComponentInChildren<TMP_Text>().text = sceneName;
                button.onClick.AddListener(() => LoadScene(scene));
            }
        }

        private void LoadScene(string scene)
        {
            SceneManager.LoadScene(scene);
        }
    }
}