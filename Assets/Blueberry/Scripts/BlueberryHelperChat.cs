using UnityEngine;
using UnityEngine.UI;

namespace Blueberry
{
    public class BlueberryHelperChat : BlueberryHelper
    {
        [SerializeField] private InputField serverName;
        
        protected override void Awake()
        {
            base.Awake();
#if !UNITY_STANDALONE_WIN && !UNITY_EDITOR
            serverName.interactable = false;
            ((Text)serverName.placeholder).text = "Blueberry";
#endif
        }
    }
}