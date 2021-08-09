using UnityEngine;
using Mirror;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using LostPolygon.AndroidBluetoothMultiplayer.Examples.UNet;
using LostPolygon.AndroidBluetoothMultiplayer;

public class NetworkManagerDemo : NetworkManager
{
    public GameObject TapMarkerPrefab; // Reference to the tap effect
    public bool StressTestMode;

    private const int kStressModeActors = 30;

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

    public override void OnStopClient()
    {
        base.OnStopClient();

#if UNITY_ANDROID
        // Stopping all Bluetooth connectivity on Unity networking disconnect event
        AndroidBluetoothMultiplayer.Stop();
#endif
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        // Spawn the controllable actors
        int actorCount = !StressTestMode ? 1 : kStressModeActors;
        GameObject player = null;
        for (int i = 0; i < actorCount; i++)
        {
            Vector3 position = Random.insideUnitCircle * 15f;
            player = (GameObject)Instantiate(playerPrefab, position, Quaternion.identity);
            TestActor testActor = player.GetComponent<TestActor>();

            // Make them smaller and more random in stress test mode
            if (StressTestMode)
            {
                testActor.PositionRandomOffset = 10f;
                player.transform.localScale *= 0.5f;
                testActor.TransformLocalScale = player.transform.localScale;
            }
            NetworkServer.Spawn(player, conn);
        }
        NetworkServer.AddPlayerForConnection(conn, player);
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
            if (conn == null || conn == connection)
                continue;

            conn.Send(createTapMarkerMessage);
        }

        NetworkConnection local = NetworkServer.localConnection;
        if (local != null && connection != local)
        {
            local.Send(createTapMarkerMessage);
        }
    }
}
