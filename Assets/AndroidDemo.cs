using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AndroidDemo : MonoBehaviour
{
    [SerializeField]
    [Scene]
    private string[] scenes = null;

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 40, 400, 9999));

        int length = scenes.Length;
        string scene, sceneName;
        string[] split;
        for (int i = 0; i < length; i++)
        {
            scene = scenes[i];
            split = scene.Split('/', '.');
            sceneName = split[split.Length-2];
            if (GUILayout.Button(sceneName, GUILayout.Height(75)))
            {
                LoadScene(scene);
            }
        }

        GUILayout.EndArea();
    }

    private void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

}
