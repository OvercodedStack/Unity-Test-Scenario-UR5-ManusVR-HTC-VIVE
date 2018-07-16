using UnityEngine;
using UnityEngine.UI;

public class RotateJoints : MonoBehaviour
{
    private RuntimePlatform platform = Application.platform;
    public float speed_touch = 0.01f;
    public float speed_mouse = 0.001f;

    Ray cursorRay;// = Camera.main.ScreenPointToRay(Vector3.zero);
    RaycastHit hit;

    Vector2 touchDeltaPosition = Vector2.zero;
    Vector2 mouseDeltaPosition = Vector2.zero;
    Vector2 currentMousePosition = Vector2.zero;
    Vector2 lastMousePosition = Vector2.zero;


    public UR5Controller controller;
    private GameObject[] jointList = new GameObject[6];
    private Slider[] sliderList = new Slider[6];


    // Use this for initialization
    void Start()
    {
        controller.initializeJoints(jointList);
        controller.initializeSliders(sliderList);

        cursorRay = Camera.main.ScreenPointToRay(Vector3.zero);
    }

        void Update()
    {
        /*if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
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
        }*/
        


        if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                cursorRay = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            }
        }
        else if (platform == RuntimePlatform.WindowsEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                currentMousePosition = Input.mousePosition;
                lastMousePosition = currentMousePosition;
                mouseDeltaPosition = currentMousePosition - lastMousePosition;  // aka 0
                cursorRay = Camera.main.ScreenPointToRay(currentMousePosition);
            }
            else if (Input.GetMouseButton(0)) // do this also for touch
            {
                currentMousePosition = Input.mousePosition;
                mouseDeltaPosition = currentMousePosition - lastMousePosition;
                lastMousePosition = currentMousePosition;
            }
        }

        if (Physics.Raycast(cursorRay, out hit, 1000.0f))
        {
            //Debug.Log("Hit detected on object " + hit.collider.gameObject.name + " at point " + hit.point);
            Transform parent = hit.collider.gameObject.transform.parent;
            Transform parent2 = hit.collider.gameObject.transform.parent.transform.parent;

            for (int i = 0; i < 6; i++)
            {
                if (jointList[i].name == parent.name || jointList[i].name == parent2.name)
                {
                    //Debug.Log("Found object parents:  " + parent.name + parent2.name);
                    Vector3 currentRotation = jointList[i].transform.localEulerAngles;
                    Debug.Log(currentRotation);

                    if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
                    {
                        //float d = Mathf.Sqrt(touchDeltaPosition.x*touchDeltaPosition.x + touchDeltaPosition.y*touchDeltaPosition.y);
                        //Debug.Log(d);
                        currentRotation.z += touchDeltaPosition.magnitude * Mathf.Sign(touchDeltaPosition.x) * speed_touch;
                    }
                    else if (platform == RuntimePlatform.WindowsEditor)
                    {
                        //currentRotation.z += Input.GetAxis("Mouse X") * speed_mouse * 40.0f;  //got x40 factor from SE...??
                        currentRotation.z += mouseDeltaPosition.magnitude * Mathf.Sign(mouseDeltaPosition.x) * speed_mouse;
                    }
                    //Debug.Log(currentRotation);

                    sliderList[i].value = currentRotation.z;
                    //jointList[i].transform.localEulerAngles = currentRotation;
                }
            }
        }


    }



}