#if UNITY_ANDROID

using System;

namespace LostPolygon.AndroidBluetoothMultiplayer {
    /// <summary>
    /// Represents an error when code was attempting to use Bluetooth adapter when it was disabled.
    /// </summary>
    [Serializable]
    public sealed class BluetoothNotEnabledException : Exception {
        public BluetoothNotEnabledException() : base("Bluetooth not enabled while calling a method that requires Bluetooth adapter to be enabled") {
        }

        public BluetoothNotEnabledException(string message) : base(message) {
        }
    }
}

#endif