using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToAndroidDemo : MonoBehaviour
{
    private static BackToAndroidDemo instance;
    private static string scene;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        scene = SceneManager.GetActiveScene().name;
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            NetworkManager.singleton.StopHost();
            Destroy(NetworkManager.singleton.gameObject);
            SceneManager.LoadScene(scene);
        }
    }
}
