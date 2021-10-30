using UnityEngine;

namespace javierfoe.Blueberry
{
    public class Joystick : MonoBehaviour
    {
        public static float Horizontal { get; private set; }
        public static float Vertical { get; private set; }

        private bool touchStart;
        private Vector3 pointA, pointB;

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                pointA = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            }
            if (Input.GetMouseButton(0))
            {
                touchStart = true;
                pointB = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            }
            else
            {
                touchStart = false;
            }

            if (touchStart)
            {
                Vector3 direction = (pointB-pointA).normalized;

                Horizontal = direction.x;
                Vertical = direction.y;
            }
            else
            {
                Horizontal = 0;
                Vertical = 0;
            }

            Debug.Log($"Horizontal: {Horizontal} Vertical:{Vertical}");
        }
    }
}