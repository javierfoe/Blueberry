using UnityEngine;
using Mirror;

namespace LostPolygon.AndroidBluetoothMultiplayer.Examples.UNet
{
    /// <summary>
    /// A very simple object. Moves to the position of the touch with interpolation.
    /// </summary>
    public class TestActor : NetworkBehaviour
    {
        public float Speed = 100f;

        [SyncVar]
        public float PositionRandomOffset = 0f;

        [SyncVar]
        private Vector3 _destination;
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rigidbody2D;

        [SyncVar(hook = "OnColorChangedHandler")]
        private Color _color;

        [SyncVar(hook = "OnTransformLocalScaleChangedHandler")]
        public Vector3 TransformLocalScale;

        private readonly Color[] kColors = {
            //Color.white,
            Color.blue,
            Color.cyan,
            Color.green,
            Color.magenta,
            Color.red,
            Color.yellow
        };

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _destination = transform.position;

            _color = kColors[Random.Range(0, kColors.Length)];
            _spriteRenderer.color = _color;
            TransformLocalScale = transform.localScale;
        }

        private void Update()
        {
            if (!hasAuthority)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                _destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _destination += (Vector3)(Random.insideUnitCircle * PositionRandomOffset);
                CmdDestination(_destination);
            }
        }

        private void FixedUpdate()
        {
            _destination.z = 0f;
            _rigidbody2D.position = Vector3.MoveTowards(_rigidbody2D.position, _destination, Speed * Time.deltaTime);
        }

        private void OnTransformLocalScaleChanged(Vector3 localScale)
        {
            transform.localScale = localScale;
        }

        private void OnTransformLocalScaleChangedHandler(Vector3 oldLocalScale, Vector3 newLocalScale)
        {
            OnTransformLocalScaleChanged(newLocalScale);
        }

        private void OnColorChanged(Color color)
        {
            _spriteRenderer.color = color;
        }

        private void OnColorChangedHandler(Color oldColor, Color newColor)
        {
            OnColorChanged(newColor);
        }

        public override void OnStartClient()
        {
            OnTransformLocalScaleChanged(TransformLocalScale);
            OnColorChanged(_color);
        }

        [Command]
        private void CmdDestination(Vector3 destination)
        {
            _destination = destination;
        }
    }
}