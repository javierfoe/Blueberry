using Mirror;
using UnityEngine;

namespace javierfoe.Blueberry.Examples.Pong
{
    public class Player : NetworkBehaviour
    {
        public float speed = 30;
        public Rigidbody2D rigidbody2d;

        // need to use FixedUpdate for rigidbody
        void FixedUpdate()
        {
            // only let the local player control the racket.
            // don't control other player's rackets
            if (isLocalPlayer)
                if (Input.GetMouseButton(0))
                {
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 direction = (mousePosition - transform.position).normalized;
                    rigidbody2d.velocity = new Vector2(0, direction.y) * speed * Time.fixedDeltaTime;
                }
                else
                {
                    rigidbody2d.velocity = Vector2.zero;
                }
        }
    }
}
