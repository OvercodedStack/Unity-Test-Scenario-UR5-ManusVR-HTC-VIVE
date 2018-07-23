/////////////////////////////////////////////////////////////////////////////
//
//  Original System: TouchControl.cs
//  Subsystem:       Human-Robot Interaction
//  Workfile:        Manus_interpreter.cs
//  Revision:        1.1 - 6/21/2018
//  Author:          Shelly Bagachi/ Edits made: Esteban Segarra
//
//  Description
//  ===========
//  Touchscreen and mouse controls for UR5 robot control. 
//
///////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//From http://answers.unity3d.com/questions/610440/on-touch-event-on-game-object-on-android-2d.html

public class TouchControl : MonoBehaviour {
    //private GameObject thisObj;
    private RuntimePlatform platform = Application.platform;
    public float speed_touch = 0.01f;
    public float speed_mouse = 10f;

    Ray cursorRay;
    RaycastHit hit;

    //public bool did_get_hit;
    public bool did_feel_hit;
    bool was_prev_clicked = false;
    bool first_click = true;
    public bool newHit = false;
    public Vector2 touchPosition           = Vector2.zero;
    public Vector2 touchDeltaPosition      = Vector2.zero;
    public Vector2 currentMousePosition    = Vector2.zero;
    public Vector2 lastMousePosition       = Vector2.zero;
    public Vector2 mouseDeltaPosition      = Vector2.zero;

    // Use this for initialization
    void Start () {
        cursorRay = Camera.main.ScreenPointToRay(Vector3.zero);
    }
	
	// Update is called once per frame
	void Update () {
        if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                touchPosition = Input.GetTouch(0).position;
                cursorRay = Camera.main.ScreenPointToRay(touchPosition);
                touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            }
        }
        else if (platform == RuntimePlatform.WindowsEditor)
        {
            newHit = false;
            if (!Input.GetMouseButton(0) && was_prev_clicked)
            {
                currentMousePosition = Input.mousePosition;
                was_prev_clicked = false;
                first_click = true;
                mouseDeltaPosition = currentMousePosition - lastMousePosition;
                Vector3 pos_cam = new Vector3(currentMousePosition.x, currentMousePosition.y, 0);
                cursorRay = Camera.main.ScreenPointToRay(pos_cam);
                newHit = true;
            }

            if (Input.GetMouseButton(0) && first_click)
            {
                lastMousePosition = Input.mousePosition;
                was_prev_clicked = true;
                first_click = false;
                currentMousePosition = new Vector3();
            }

            else if (Input.GetMouseButton(0))
            {
                was_prev_clicked = true;
            }
            

        }

        Debug.DrawLine(currentMousePosition, lastMousePosition);
        if (Physics.Raycast(cursorRay, out hit, 1000.0f))
        {
            //Debug.DrawLine(cursorRay.origin, hit.point);
            if (hit.collider.gameObject.name == this.name)
            {
                {
                    if (newHit)
                        Debug.Log("Hit detected on object " + hit.collider.gameObject.name + " at point " + hit.point);
                    var pos = gameObject.transform.position;

                    Debug.Log(gameObject.transform.position.ToString());

                    if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
                    {
                        //float d = Mathf.Sqrt(touchDeltaPosition.x*touchDeltaPosition.x + touchDeltaPosition.y*touchDeltaPosition.y);
                        //Debug.Log(d);
                        //pos.z += touchDeltaPosition.magnitude * Mathf.Sign(touchDeltaPosition.x) * speed_touch;
                        pos.x += touchDeltaPosition.x * speed_touch;
                        pos.y += touchDeltaPosition.y * speed_touch;

                    }
                    else if (platform == RuntimePlatform.WindowsEditor)
                    {
                        //currentRotation.z += Input.GetAxis("Mouse X") * speed_mouse * 40.0f;  //got x40 factor from SE...??
                        //pos.z += mouseDeltaPosition.magnitude * Mathf.Sign(mouseDeltaPosition.x) * speed_mouse;
                        pos.x += mouseDeltaPosition.x * speed_mouse;
                        pos.y += mouseDeltaPosition.y * speed_mouse;
                    }
                    Debug.Log(pos.ToString());
                    gameObject.transform.position = pos;
                }
            }
        }
    }


    //Check where my camera is 
    void checkTouch(Vector2 pos)
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(pos);
        Vector2 touchPos = new Vector2(wp.x, wp.y);
        Collider2D hit = Physics2D.OverlapPoint(touchPos);
        //did_get_hit = hit;
        if (hit)
        {
            if (hit.transform.gameObject.name == name)
            {
                Debug.Log(name);
                hit.transform.gameObject.SendMessage("Clicked", 0, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
