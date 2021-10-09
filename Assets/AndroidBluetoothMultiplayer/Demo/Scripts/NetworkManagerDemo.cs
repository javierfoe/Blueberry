using UnityEngine;
using Mirror;
using LostPolygon.AndroidBluetoothMultiplayer.Examples.UNet;
using System.Collections.Generic;

public class NetworkManagerDemo : NetworkManager
{
    public GameObject TapMarkerPrefab; // Reference to the tap effect

    public override void OnStartServer()
    {
        base.OnStartServer();

        // Register the handler for the CreateTapMarkerMessage that is sent from client to server
        NetworkServer.RegisterHandler<CreateTapMarkerMessage>(OnServerCreateTapMarkerHandler);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        NetworkClient.RegisterHandler<CreateTapMarkerMessage>(OnClientCreateTapMarkerHandler);
    }

    // Called when client receives a CreateTapMarkerMessage
    private void OnClientCreateTapMarkerHandler(CreateTapMarkerMessage createTapMarkerMessage)
    {
        Instantiate(TapMarkerPrefab, createTapMarkerMessage.Position, Quaternion.identity);
    }

    // Called when server receives a CreateTapMarkerMessage
    private void OnServerCreateTapMarkerHandler(NetworkConnection connection, CreateTapMarkerMessage createTapMarkerMessage)
    {

        // Retransmit this message to all other clients except the one who initially sent it,
        // since that client already creates a local tap marker on his own

        NetworkConnection conn;
        foreach (KeyValuePair<int, NetworkConnectionToClient> entry in NetworkServer.connections)
        {
            conn = entry.Value;
            conn.Send(createTapMarkerMessage);
        }

        NetworkConnection local = NetworkServer.localConnection;
        if (local != null && connection != local)
        {
            local.Send(createTapMarkerMessage);
        }
    }
}
