using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace javierfoe.Blueberry.Hub
{
    public class BackToHub : MonoBehaviour
    {
        private static BackToHub _instance;
        private static string _scene;

        // Start is called before the first frame update
        void Start()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            _scene = SceneManager.GetActiveScene().name;
            _instance = this;
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
                SceneManager.LoadScene(_scene);
            }
        }
    }
}