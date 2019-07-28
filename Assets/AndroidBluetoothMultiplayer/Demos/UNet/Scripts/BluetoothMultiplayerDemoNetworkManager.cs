using UnityEngine;
using Mirror;
using Random = UnityEngine.Random;
using System.Collections.Generic;

namespace LostPolygon.AndroidBluetoothMultiplayer.Examples.UNet
{
    public class BluetoothMultiplayerDemoNetworkManager : AndroidBluetoothNetworkManager
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

        public override void OnServerReady(NetworkConnection conn)
        {
            base.OnServerReady(conn);

            // Spawn the controllable actors
            int actorCount = !StressTestMode ? 1 : kStressModeActors;
            for (int i = 0; i < actorCount; i++)
            {
                Vector3 position = Random.insideUnitCircle * 15f;
                GameObject actorGameObject = (GameObject)Instantiate(playerPrefab, position, Quaternion.identity);
                TestActor testActor = actorGameObject.GetComponent<TestActor>();

                // Make them smaller and more random in stress test mode
                if (StressTestMode)
                {
                    testActor.PositionRandomOffset = 10f;
                    actorGameObject.transform.localScale *= 0.5f;
                    testActor.TransformLocalScale = actorGameObject.transform.localScale;
                }


                // Set player authority
                NetworkServer.SpawnWithClientAuthority(actorGameObject, conn);
            }
        }

        // Called when client receives a CreateTapMarkerMessage
        private void OnClientCreateTapMarkerHandler(NetworkConnection connection, CreateTapMarkerMessage createTapMarkerMessage)
        {
            Instantiate(TapMarkerPrefab, createTapMarkerMessage.Position, Quaternion.identity);
        }

        // Called when server receives a CreateTapMarkerMessage
        private void OnServerCreateTapMarkerHandler(NetworkConnection connection, CreateTapMarkerMessage createTapMarkerMessage)
        {

            // Retransmit this message to all other clients except the one who initially sent it,
            // since that client already creates a local tap marker on his own

            NetworkConnection conn;
            foreach (KeyValuePair<int, NetworkConnection> entry in NetworkServer.connections)
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
}