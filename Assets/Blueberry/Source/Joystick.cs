using UnityEngine;
using UnityEngine.EventSystems;

namespace javierfoe.Blueberry
{
    public class Joystick : MonoBehaviour
    {
        public static float Horizontal { get; private set; }
        public static float Vertical { get; private set; }

        private Vector3 pointA, pointB;

        // Update is called once per frame
        private void Update()
        {
            int touchCount = Input.touchCount;
            if(touchCount > 0)
            {
                Touch touch;
                int fingerId = -1;
                for(int i = 0; i < touchCount; i++)
                {
                    touch = Input.GetTouch(i);
                    if (fingerId > -1 && fingerId != touch.fingerId ||EventSystem.current.IsPointerOverGameObject(touch.fingerId)) continue;
                    fingerId = touch.fingerId;
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            pointA = touch.position;
                            break;
                        case TouchPhase.Moved:
                            pointB = touch.position;
                            Vector3 direction = pointB - pointA;
                            if (direction.sqrMagnitude > 1)
                            {
                                direction = direction.normalized;
                            }

                            Horizontal = direction.x;
                            Vertical = direction.y;
                            break;
                        case TouchPhase.Ended:
                            Horizontal = 0;
                            Vertical = 0;
                            break;
                    }
                }
            }
        }
    }
}