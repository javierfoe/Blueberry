using UnityEngine;
using Mirror;

namespace LostPolygon.AndroidBluetoothMultiplayer.Examples.UNet {
    public struct CreateTapMarkerMessage : NetworkMessage {
        // Some arbitrary message type id number
        public const short kMessageType = 12345;

        // Position of the tap
        public Vector2 Position;
    }
}