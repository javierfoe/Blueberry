#if UNITY_ANDROID

using System;

namespace LostPolygon.AndroidBluetoothMultiplayer {
    /// <summary>
    /// Custom Bluetooth device browser definition.
    /// </summary>
    public interface ICustomDeviceBrowser {
        event Action OnOpened;
        event Action OnClosing;
        event Action<BluetoothDevice> OnDevicePicked;
        void Open();
        void Close();
    }
}

#endif
