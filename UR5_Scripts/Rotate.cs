using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour
{
    public float speed = 1.0F;
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            // Get movement of the finger since last frame
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            //Vector3 touchDeltaPosition = (Vector3)Input.GetTouch(0).deltaPosition;

            // Move object across XY plane
            //transform.Translate(touchDeltaPosition.y * speed, 0, touchDeltaPosition.x * speed);
            // Rotate
            //transform.Rotate(touchDeltaPosition.y * speed, 0, touchDeltaPosition.x * speed);
            Vector3 currentRotation = transform.localEulerAngles;
            //Debug.Log(currentRotation);
            currentRotation.x += touchDeltaPosition.x * speed;
            transform.localEulerAngles = currentRotation;
        }
    }
}