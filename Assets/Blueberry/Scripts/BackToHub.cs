using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace javierfoe.Blueberry.Hub
{
    public class BackToHub : MonoBehaviour
    {
        private static BackToHub instance;
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
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                NetworkManager.singleton.offlineScene = null;
                NetworkManager.singleton.StopHost();
                Destroy(NetworkManager.singleton.gameObject);
                NetworkManager.ResetStatics();
                SceneManager.LoadScene(scene);
            }
        }
    }
}