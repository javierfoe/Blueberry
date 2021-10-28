using Mirror;
using UnityEngine;

namespace javierfoe.AndroidBluetoothMultiplayer.Examples.RigidbodyPhysics
{
    public class AddForce : NetworkBehaviour
    {
        public float force = 500f;
        private Rigidbody rigidbody3d;

        void Start()
        {
            rigidbody3d = GetComponent<Rigidbody>();
            rigidbody3d.isKinematic = !isServer;
        }

        void Update()
        {
            if (isServer && Input.GetMouseButtonDown(0))
            {
                rigidbody3d.AddForce(Vector3.up * force);
            }
        }
    }
}
