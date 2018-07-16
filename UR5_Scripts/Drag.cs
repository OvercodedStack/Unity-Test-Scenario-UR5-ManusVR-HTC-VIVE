using UnityEngine;
using System.Collections;

public class Drag : MonoBehaviour
{
    public float speed = 0.001F;
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            // Get movement of the finger since last frame
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

            // Move object across XY plane
            transform.Translate(touchDeltaPosition.y * speed, 0, touchDeltaPosition.x * speed);
        }
    }
}