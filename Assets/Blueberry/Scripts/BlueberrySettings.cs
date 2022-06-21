using System;
using UnityEngine;

namespace javierfoe.Blueberry
{

    [Serializable]
    public class BlueberrySettings
    {
        [Tooltip("Bluetooth service application identifier, must be unique for every application. " +
                 "If you have multiple scenes with different NetworkManager's in your project, " +
                 "make sure the UUID is identical everywhere, otherwise Bluetooth connections will fail.")]
        [SerializeField]
        protected string uuid = "";

        [Tooltip("Bluetooth discoverability interval. Server is made discoverable over Bluetooth, so clients would " +
                 "have the ability to locate the server. On Android 4.0 and higher, value of 0 allows making device discoverable " +
                 "\"forever\" (until discoverability is disabled manually or Bluetooth is disabled).")]
        [SerializeField]
        protected int defaultBluetoothDiscoverabilityInterval = 120;

        [Tooltip("Indicates whether to stop the Bluetooth server when listening " +
                 "for incoming Bluetooth connections has stopped.")]
        [SerializeField]
        protected bool stopBluetoothServerOnListeningStopped = true;

        [Tooltip("Indicates whether Android Bluetooth Multiplayer events should be logged.")] [SerializeField]
        protected bool logBluetoothEvents;

        /// <summary>
        /// Gets or sets the Bluetooth service UUID.
        /// </summary>
        /// <value>
        /// The UUID. Must be unique for every application
        /// </value>
        /// <exception cref="ArgumentException">UUID can't be empty.</exception>
        public string Uuid
        {
            get => uuid;
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentException("UUID can't be empty", nameof(value));

                uuid = value;
            }
        }

        /// <summary>
        /// Gets or sets the default Bluetooth discoverability interval.
        /// </summary>
        /// <exception cref="ArgumentException">Discoverability interval can't be less than 0.</exception>
        public int DefaultBluetoothDiscoverabilityInterval
        {
            get => defaultBluetoothDiscoverabilityInterval;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Discoverability interval can't be < 0", nameof(value));

                defaultBluetoothDiscoverabilityInterval = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to stop the Bluetooth server when listening
        /// for incoming Bluetooth connections has stopped.
        /// </summary>
        public bool StopBluetoothServerOnListeningStopped
        {
            get => stopBluetoothServerOnListeningStopped;
            set => stopBluetoothServerOnListeningStopped = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Android Bluetooth Multiplayer events should be logged.
        /// </summary>
        public bool LogBluetoothEvents {
            get => logBluetoothEvents;
            set => logBluetoothEvents = value;
        }
    }
}