///////////////////////////////////////////////////////////////////////////////
//
//  Original System: Camera_controls.cs
//  Subsystem:       Human-Robot Interaction
//  Workfile:        Manus_Open_VR V2
//  Revision:        1.0 - 6/22/2018
//  Author:          Esteban Segarra
//
//  Description
//  ===========
//  Camera control wrapper for object in control. 
//
///////////////////////////////////////////////////////////////////////////////
//Some Code inherited from https://forum.unity.com/threads/moving-main-camera-with-mouse.119525/
//Scrolling and orbit inherited from https://www.youtube.com/watch?v=bVo0YLLO43s 
using UnityEngine;

public class Camera_controls : MonoBehaviour
{
    //Limits for camera 
    protected Transform _XForm_Camera;
    protected Transform _XForm_Parent;
    protected Vector3 _LocalRotation;
    protected float _CameraDistance = 10f;
    private Vector3 shift;

    //Speed Limits
    public float horizontal_speed = 1;
    public float vertical_speed = 1;
    public float foward_speed = 1;
    public float turn_speed = 1; 
    //public bool CameraDisabled = true;
 
    public float ScrollSensitvity = 2f;
    public float OrbitDampening = 10f;
    public float ScrollDampening = 6f;

    void Start()
    {
        this._XForm_Camera = this.transform;
        this._XForm_Parent = this.transform.parent;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //if ()
        //    CameraDisabled = !CameraDisabled;

        
        if (Input.GetMouseButton(1))
        //
        {

            _LocalRotation.x += Input.GetAxis("Mouse X") * turn_speed;
            _LocalRotation.y += Input.GetAxis("Mouse Y") * turn_speed;

            //Clamp the y Rotation to horizon and not flipping over at the top
            if (_LocalRotation.y < 0f)
                _LocalRotation.y = 0f;
            else if (_LocalRotation.y > 90f)
                _LocalRotation.y = 90f;

            //float hor = horizontal_speed * Input.GetAxis("Mouse Y");
            //float ver = vertical_speed * Input.GetAxis("Mouse X");


            //Clamp the y Rotation to horizon and not flipping over at the top
            //if (this.transform.localRotation.eulerAngles.y < 0f)
            //    hor = 0f;
            //else if (this.transform.localRotation.eulerAngles.y > 90f)
            //    hor = 0;

            //transform.Rotate(hor, ver, 0);


            //if (Input.GetButton("LShift"))
            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                float ScrollAmount = Input.GetAxis("Mouse ScrollWheel") * ScrollSensitvity;
                ScrollAmount *= (this._CameraDistance * 0.3f);
                this._CameraDistance += ScrollAmount * -1f;
                this._CameraDistance = Mathf.Clamp(this._CameraDistance, 0.5f, 100f);
            }

            //Actual Camera Rig Transformations
            Quaternion QT = Quaternion.Euler(_LocalRotation.y, _LocalRotation.x, 0);
            this._XForm_Parent.rotation = Quaternion.Lerp(this._XForm_Parent.rotation, QT, Time.deltaTime * OrbitDampening);

            if (this._XForm_Camera.localPosition.z != this._CameraDistance * -1f)
            {
                this._XForm_Camera.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this._XForm_Camera.localPosition.z, this._CameraDistance * -1f, Time.deltaTime * ScrollDampening));
            }
        }




        //Conditionals adjusted for camera viewset
        if (Input.GetButton("Foward"))
        {
            shift = new Vector3(0, foward_speed, 0);
            _XForm_Parent.transform.Translate(shift);
        }

        if (Input.GetButton("Back"))
        {
            shift = new Vector3(0, -foward_speed, 0);
            _XForm_Parent.transform.Translate(shift);
        }
        if (Input.GetButton("Left"))
        {
            shift = new Vector3(-horizontal_speed, 0, 0);
            _XForm_Parent.transform.Translate(shift);
        }

        if (Input.GetButton("Right"))
        {
            shift = new Vector3(horizontal_speed, 0, 0);
            _XForm_Parent.transform.Translate(shift);
        }

    }
}
