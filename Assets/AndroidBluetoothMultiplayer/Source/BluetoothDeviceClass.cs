#if UNITY_ANDROID

namespace LostPolygon.AndroidBluetoothMultiplayer {
    /// <summary>
    /// A helper class that simplifies working with Bluetooth device classes.
    /// </summary>
    public static class BluetoothDeviceClass {
        /// <summary>
        /// Retrieves major Bluetooth device class a full Bluetooth device class.
        /// </summary>
        /// <param name="deviceClass">The full Bluetooth device class.</param>
        /// <returns>The <see cref="MajorClass"/> part of full Bluetooth device class.</returns>
        public static MajorClass GetMajorClass(this Class deviceClass) {
            return (MajorClass) ((int) deviceClass & 0x1F00);
        }

        /// <summary>
        /// Whether a device with Bluetooth class <paramref name="deviceClass"/>
        /// is a handheld device and believed to have data connectivity.
        /// </summary>
        /// <param name="deviceClass">The Bluetooth device class to test against.</param>
        /// <returns>
        /// true, if a device with Bluetooth class <paramref name="deviceClass"/>
        /// is a handheld device and believed to have data connectivity.
        /// </returns>
        public static bool IsProbablyHandheldDataCapableDevice(this Class deviceClass) {
            switch (deviceClass) {
                case Class.ComputerUncategorized:
                case Class.ComputerDesktop:
                case Class.ComputerLaptop:
                case Class.ComputerHandheldPcPda:
                case Class.ComputerPalmSizePcPda:
                case Class.PhoneUncategorized:
                case Class.PhoneCellular:
                case Class.PhoneCordless:
                case Class.PhoneSmart:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Whether a device with Bluetooth class <paramref name="deviceClass"/>
        /// is believed to have data connectivity.
        /// </summary>
        /// <param name="deviceClass">The Bluetooth device class to test against.</param>
        /// <returns>
        /// true if a device with Bluetooth class <paramref name="deviceClass"/>
        /// is believed to have data connectivity, false otherwise.
        /// </returns>
        public static bool IsProbablyDataCapable(this Class deviceClass) {
            switch (deviceClass) {
                case Class.ComputerUncategorized:
                case Class.ComputerDesktop:
                case Class.ComputerServer:
                case Class.ComputerLaptop:
                case Class.ComputerHandheldPcPda:
                case Class.ComputerPalmSizePcPda:
                case Class.ComputerWearable:
                case Class.PhoneUncategorized:
                case Class.PhoneCellular:
                case Class.PhoneCordless:
                case Class.PhoneSmart:
                case Class.PhoneModemOrGateway:
                case Class.PhoneIsdn:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Major Bluetooth device class.
        /// </summary>
        public enum MajorClass {
            Misc = 0x0000,
            Computer = 0x0100,
            Phone = 0x0200,
            Networking = 0x0300,
            AudioVideo = 0x0400,
            Peripheral = 0x0500,
            Imaging = 0x0600,
            Wearable = 0x0700,
            Toy = 0x0800,
            Health = 0x0900,
            Uncategorized = 0x1F00,
        }


        /// <summary>
        /// Full Bluetooth device class.
        /// </summary>
        public enum Class {
            // Devices in the COMPUTER major class
            ComputerUncategorized = 0x0100,
            ComputerDesktop = 0x0104,
            ComputerServer = 0x0108,
            ComputerLaptop = 0x010C,
            ComputerHandheldPcPda = 0x0110,
            ComputerPalmSizePcPda = 0x0114,
            ComputerWearable = 0x0118,

            // Devices in the PHONE major class
            PhoneUncategorized = 0x0200,
            PhoneCellular = 0x0204,
            PhoneCordless = 0x0208,
            PhoneSmart = 0x020C,
            PhoneModemOrGateway = 0x0210,
            PhoneIsdn = 0x0214,

            // Minor classes for the AUDIO_VIDEO major class
            AudioVideoUncategorized = 0x0400,
            AudioVideoWearableHeadset = 0x0404,
            AudioVideoHandsfree = 0x0408,
            // AUDIO_VIDEO_RESERVED = 0x040C,
            AudioVideoMicrophone = 0x0410,
            AudioVideoLoudspeaker = 0x0414,
            AudioVideoHeadphones = 0x0418,
            AudioVideoPortableAudio = 0x041C,
            AudioVideoCarAudio = 0x0420,
            AudioVideoSetTopBox = 0x0424,
            AudioVideoHifiAudio = 0x0428,
            AudioVideoVcr = 0x042C,
            AudioVideoVideoCamera = 0x0430,
            AudioVideoCamcorder = 0x0434,
            AudioVideoVideoMonitor = 0x0438,
            AudioVideoVideoDisplayAndLoudspeaker = 0x043C,
            AudioVideoVideoConferencing = 0x0440,
            // AUDIO_VIDEO_RESERVED  = 0x0444,
            AudioVideoVideoGamingToy = 0x0448,

            // Devices in the WEARABLE major class
            WearableUncategorized = 0x0700,
            WearableWristWatch = 0x0704,
            WearablePager = 0x0708,
            WearableJacket = 0x070C,
            WearableHelmet = 0x0710,
            WearableGlasses = 0x0714,

            // Devices in the TOY major class
            ToyUncategorized = 0x0800,
            ToyRobot = 0x0804,
            ToyVehicle = 0x0808,
            ToyDollActionFigure = 0x080C,
            ToyController = 0x0810,
            ToyGame = 0x0814,

            // Devices in the HEALTH major class
            HealthUncategorized = 0x0900,
            HealthBloodPressure = 0x0904,
            HealthThermometer = 0x0908,
            HealthWeighing = 0x090C,
            HealthGlucose = 0x0910,
            HealthPulseOximeter = 0x0914,
            HealthPulseRate = 0x0918,
            HealthDataDisplay = 0x091C,
        }
    }
}

#endif