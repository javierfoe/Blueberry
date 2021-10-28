using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchscreenMovement : MonoBehaviour
{
    [SerializeField] private float negativeThreshold = 0.4f;
    [SerializeField] private float positiveThreshold = 0.6f;
    public static float Horizontal { get; private set; }
    public static float Vertical { get; private set; }
    public static bool Left { get; private set; }
    public static bool Right { get; private set; }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

            if (mousePos.x > positiveThreshold)
            {
                Left = false;
                Right = true;
                Horizontal = 1;
            }
            else if (mousePos.x < negativeThreshold)
            {
                Left = true;
                Right = false;
                Horizontal = -1;
            }
            else
            {
                Left = false;
                Right = false;
                Horizontal = 0;
            }
            if (mousePos.y > positiveThreshold)
            {
                Vertical = 1;
            }
            else if (mousePos.y < negativeThreshold)
            {
                Vertical = -1;
            }
            else
            {
                Vertical = 0;
            }
        }
        else
        {
            ResetInput();
        }
    }

    private void ResetInput()
    {
        Left = false;
        Right = false;
        Horizontal = 0;
        Vertical = 0;
    }
}
