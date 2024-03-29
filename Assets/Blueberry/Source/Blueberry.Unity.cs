using UnityEngine;
using Blueberry.Internal;

namespace Blueberry
{
    /// <summary>
    /// A core class that wraps Java methods of Android plugin.
    /// </summary>
    public sealed partial class Blueberry : MonoBehaviour
    {
        /// <summary>
        /// Java Name of main plugin facade class.
        /// </summary>
        private const string kPluginClassName = "com.lostpolygon.unity.bluetoothmediator.mediator.BluetoothMediator";

        /// <summary>
        /// The name of the GameObject, used for receiving messages from Java side.
        /// </summary>
        private static readonly string kGameObjectName = "_AndroidBluetoothMultiplayer";

        /// <summary>
        /// A reference to the Java BluetoothMediator object .
        /// </summary>
        private static AndroidJavaObject _plugin;

        /// <summary>
        /// Whether the plugin is available and was loaded successfully.
        /// </summary>
        private static bool _isPluginAvailable;

        /// <summary>
        /// A reference to singleton instance.
        /// </summary>
        private static Blueberry _instance;

        /// <summary>
        /// Initializes <see cref="Blueberry"/> class.
        /// Retrieves a pointer to the Java plugin object.
        /// </summary>
        static Blueberry()
        {
            _plugin = null;
            _isPluginAvailable = false;

#if !UNITY_EDITOR && UNITY_ANDROID
            // Retrieve BluetoothMediator singleton instance
            try {
                using (AndroidJavaClass mediatorClass = new AndroidJavaClass(kPluginClassName)) {
                    if (!mediatorClass.IsNull()) {
                        _plugin = mediatorClass.CallStatic<AndroidJavaObject>("getSingleton");
                        _isPluginAvailable = !_plugin.IsNull();
                    }
                }
            } catch {
                Debug.LogError("Blueberry initialization failed.");
                throw;
            }
#endif
        }

        /// <summary>
        /// Initalizes the singleton instance at the application start.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void TryUpdateInstance()
        {
            try
            {
                UpdateInstance();
            }
            catch
            {
                // Happens when this static constructor is called from a GameObject being created.
                // Just ignoring, as this is intended and can't be avoided.
            }
        }

        /// <summary>
        /// Tries to find an existing instance in the scene,
        /// and creates one if there were none.
        /// </summary>
        private static void UpdateInstance()
        {
            if (_instance != null)
                return;

            // Trying to find an existing instance in the scene
            _instance = (Blueberry)FindObjectOfType(typeof(Blueberry));

            // Creating a new instance in case there are no instances present in the scene
            if (_instance != null)
                return;

            GameObject gameObject = new GameObject(kGameObjectName);
            _instance = gameObject.AddComponent<Blueberry>();

            // Make it hidden and indestructible
            gameObject.hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy;
        }

        private void Awake()
        {
            // Kill other instances
            if (FindObjectsOfType(typeof(Blueberry)).Length > 1)
            {
                Debug.LogError("Multiple " + kGameObjectName + " instances found, destroying...");
                DestroyImmediate(gameObject);
                return;
            }

            _instance = this;

            // Set the GameObject name to the class name for UnitySendMessage
            gameObject.name = kGameObjectName;

            // We want this object to persist across scenes
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(this);
        }
    }
}