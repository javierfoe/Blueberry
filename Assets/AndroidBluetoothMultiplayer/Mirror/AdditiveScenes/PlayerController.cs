using Mirror;
using UnityEngine;

namespace javierfoe.AndroidBluetoothMultiplayer.Examples
{
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(NetworkTransform))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : NetworkBehaviour
    {
        public CharacterController characterController;

        void OnValidate()
        {
            if (characterController == null)
                characterController = GetComponent<CharacterController>();

            characterController.enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<NetworkTransform>().clientAuthority = true;
        }

        public override void OnStartLocalPlayer()
        {
            Camera.main.orthographic = false;
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = new Vector3(0f, 3f, -8f);
            Camera.main.transform.localEulerAngles = new Vector3(10f, 0f, 0f);

            characterController.enabled = true;
        }

        void OnDisable()
        {
            if (isLocalPlayer && Camera.main != null)
            {
                Camera.main.orthographic = true;
                Camera.main.transform.SetParent(null);
                Camera.main.transform.localPosition = new Vector3(0f, 70f, 0f);
                Camera.main.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
            }
        }

        [Header("Movement Settings")]
        public float moveSpeed = 8f;
        public float turnSensitivity = 5f;
        public float maxTurnSpeed = 150f;

        [Header("Diagnostics")]
        public float vertical;
        public float turn;

        void Update()
        {
            if (!isLocalPlayer || characterController == null || !characterController.enabled)
                return;

            vertical = TouchscreenMovement.Vertical;

            bool left = TouchscreenMovement.Left;
            bool right = TouchscreenMovement.Right;

            if (left)
                turn = Mathf.MoveTowards(turn, -maxTurnSpeed, turnSensitivity);
            else if (right)
                turn = Mathf.MoveTowards(turn, maxTurnSpeed, turnSensitivity);
            else
                turn = Mathf.MoveTowards(turn, 0, turnSensitivity);
        }

        void FixedUpdate()
        {
            if (!isLocalPlayer || characterController == null || !characterController.enabled)
                return;

            transform.Rotate(0f, turn * Time.fixedDeltaTime, 0f);

            Vector3 direction = new Vector3(0, 0, vertical);
            direction = Vector3.ClampMagnitude(direction, 1f);
            direction = transform.TransformDirection(direction);
            direction *= moveSpeed;

            characterController.SimpleMove(direction);
        }
    }
}
