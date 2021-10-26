using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AndroidDemo : MonoBehaviour
{
    [SerializeField]
    private Button buttonPrefab;

    [SerializeField]
    [Scene]
    private string[] androidScenes = null;
    [SerializeField]
    [Scene]
    private string[] pcScenes = null;

    private void Awake()
    {
        string[] scenes;
        if(Application.platform == RuntimePlatform.Android)
        {
            scenes = androidScenes;
        }
        else
        {
            scenes = pcScenes;
        }
        int length = scenes.Length;
        string[] split;
        Button button;
        for (int i = 0; i < length; i++)
        {
            string scene = scenes[i];
            split = scene.Split('/', '.');
            string sceneName = split[split.Length-2];
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
