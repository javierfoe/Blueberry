#if UNITY_ANDROID

namespace LostPolygon.AndroidBluetoothMultiplayer {
    /// <summary>
    /// Represents the state in which the plugin operates.
    /// </summary>
    public enum BluetoothMultiplayerMode : byte {
        /// <summary>
        /// No Bluetooth multiplayer is going on.
        /// </summary>
        None = 0,

        /// <summary>
        /// The device acts as a server, listening for incoming connections.
        /// </summary>
        Server = 1,

        /// <summary>
        /// The device acts as a client, connecting to a specific device.
        /// </summary>
        Client = 2
    }
}

#endif