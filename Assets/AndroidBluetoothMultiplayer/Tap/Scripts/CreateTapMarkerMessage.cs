using UnityEngine;
using Mirror;

namespace javierfoe.AndroidBluetoothMultiplayer.Examples.UNet {
    public struct CreateTapMarkerMessage : NetworkMessage {
        // Some arbitrary message type id number
        public const short kMessageType = 12345;

        // Position of the tap
        public Vector2 Position;
    }
}