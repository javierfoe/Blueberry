using UnityEngine;
using UnityEngine.UI;

namespace javierfoe.Blueberry
{
    public class BlueberryHelperChat : BlueberryHelper
    {
        [SerializeField] private InputField serverName;
        [SerializeField] private Button host, client;

        protected override void Awake()
        {
            base.Awake();
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            serverName.interactable = true;
            ((Text)serverName.placeholder).text = "localhost";
            host.onClick.AddListener(_networkManager.StartHost);
            client.onClick.AddListener(_networkManager.StartClient);
#else
            host.onClick.AddListener(StartHost);
            client.onClick.AddListener(StartClient);
#endif
        }
    }
}