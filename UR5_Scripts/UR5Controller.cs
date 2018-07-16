// Original Author: Long Qian
// Email: lqian8@jhu.edu
// Modified by Shelly Bagchi

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
//using UnityStandardAssets.CrossPlatformInput;

// Needed //////////////////////////////////////////////////
//using HoloLensXboxController;
///////////////////////////////////////////////////////////

public class UR5Controller : MonoBehaviour {

    public GameObject RobotBase;
    private float[] jointValues = new float[6];
    private GameObject[] jointList = new GameObject[6];
    //private float[] upperLimit = { 180f, 180f, 180f, 180f, 180f, 180f };
    //private float[] lowerLimit = { -180f, -180f, -180f, -180f, -180f, -180f };
    // Using actual joint limits, -5 degrees to avoid stops
    // 0 == base
    public float[] upperLimit_r = { 355f, 0f, 155f, 345f, 345f, 345f };
    public float[] lowerLimit_r = { -355f, -175f, -155f, -345f, -345f, -345f };
    // With offsets
    private float[] upperLimit_s = { 400f, 90f, 155f, 435f, 345f, 345f };
    private float[] lowerLimit_s = { -310f, -85f, -155f, -255f, -345f, -345f };

    public GameObject CanvasObj;
    private Slider[] sliderList = new Slider[6];

    public InputField TextControl;
    public Toggle TextToggle;

    public float[] getJointValues()
    {
        return jointValues;
    }

    private void setJointValues(float[] input)
    {
        jointValues = input;
    }

    public GameObject[] getJointList()
    {
        return jointList;
    }

    public Slider[] getSliderList()
    {
        return sliderList;
    }

    public void setSliderList(float[] values)
    {
        for (int i = 0; i < 6; i++)
        {
            sliderList[i].value = values[i];
        }
    }

    // Needed //////////////////////////////////////////////////
    //private ControllerInput controllerInput;
    ///////////////////////////////////////////////////////////

    // Use this for initialization
    void Start()
    {
        initializeJoints(jointList);
        initializeSliders(sliderList);

        TextControl.text = "(0,0,0,0,0,0)";

        // Needed //////////////////////////////////////////////////
        //controllerInput = new ControllerInput(0, 0.19f);
        // First parameter is the number, starting at zero, of the controller you want to follow.
        // Second parameter is the default “dead” value; meaning all stick readings less than this value will be set to 0.0.
        ///////////////////////////////////////////////////////////
        
    }

    // Update is called once per frame
    void Update() {
        //TextControl.text = string.Format("({0:0.0}, {1:0.0}, {2:0.0}, {3:0.0}, {4:0.0}, {5:0.0})",
        //    jointValues[5], jointValues[4], jointValues[3],
        //    jointValues[2], jointValues[1], jointValues[0]);

        // Needed //////////////////////////////////////////////////
        //controllerInput.Update();
        ///////////////////////////////////////////////////////////
    }

    // Right before camera renders
    void LateUpdate() {

        for (int i = 0; i < 6; i++)
        {
            Vector3 currentRotation = jointList[i].transform.localEulerAngles;
            //Debug.Log(currentRotation);
            currentRotation.z = jointValues[i];
            jointList[i].transform.localEulerAngles = currentRotation;
        }
    }

    void OnGUI() {
        for (int i = 0; i < 6; i++) {
            jointValues[i] = sliderList[i].value;
        }

        if (TextToggle.isOn) {
            float[] offsetValues = offsetSliderValues(sliderList);
            /*var temp = "";
            offsetJointValues(offsetValues).ToList().ForEach(i => temp+=", "+i.ToString());
            Debug.Log("re-offset slider values for checking (order not reversed): " +  temp );*/

            TextControl.text = string.Format("({0:0.0}, {1:0.0}, {2:0.0}, {3:0.0}, {4:0.0}, {5:0.0})",
                offsetValues[5], offsetValues[4], offsetValues[3],
                offsetValues[2], offsetValues[1], offsetValues[0]);

        }
        else {
            TextControl.text = string.Format("({0:0.0}, {1:0.0}, {2:0.0}, {3:0.0}, {4:0.0}, {5:0.0})",
                jointValues[5], jointValues[4], jointValues[3],
                jointValues[2], jointValues[1], jointValues[0]);
        }
    }


    // Get sliders & offset to joint values for robot
    public float[] offsetSliderValues(Slider[] sliderList_) {

        float[] axes = new float[6];

        float tempVal = 0.0f;
        for (int i = 0; i < 6; i++)
        {
            // Offsets - should be opposite of set
            switch (i)
            {
                case 0:
                    tempVal = -1f * (sliderList_[i].value + 45f);
                    break;
                case 1:
                    tempVal = sliderList_[i].value - 90f;
                    break;
                case 3:
                    tempVal = sliderList_[i].value - 90f;
                    break;
                case 4:
                    tempVal = -1f * sliderList_[i].value;
                    break;
                default:
                    tempVal = sliderList_[i].value;
                    break;
            }

            // Check if out of bounds and loop around
            if (tempVal > upperLimit_r[i])
                tempVal -= 360f;

            else if (tempVal < lowerLimit_r[i])
                tempVal += 360f;

            // Save modified slider value
            axes[i] = tempVal;
        }

        return axes;
    }

    // Get joint values & offset to slider space
    public float[] offsetJointValues(float[] axes) {
        
        float[] sliderVals = new float[6];

        float tempVal = 0.0f;
        for (int i = 0; i < 6; i++)
        {
            // Should be opposite of offsets
            switch (i)
            {
                case 0:
                    tempVal = (-1f * axes[i]) - 45f;
                    break;
                case 1:
                    tempVal = axes[i] + 90f;
                    break;
                case 3:
                    tempVal = axes[i] + 90f;
                    break;
                case 4:
                    tempVal = -1f * axes[i];
                    break;
                default:
                    tempVal = axes[i];
                    break;
            }

            // Check if out of bounds and loop around
            if (tempVal > upperLimit_s[i])
                tempVal -= 360f; 

            else if (tempVal < lowerLimit_s[i])
                tempVal += 360f;

            // Save modified joint value
            sliderVals[i] = tempVal;
        }

        return sliderVals;
    }


    // Create the list of GameObjects that represent each joint of the robot
    public void initializeJoints(GameObject[] jointList_) {
        var RobotChildren = RobotBase.GetComponentsInChildren<Transform>();
        for (int i = 0; i < RobotChildren.Length; i++)
        {
            if (RobotChildren[i].name == "control0")
            {
                jointList_[0] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "control1")
            {
                jointList_[1] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "control2")
            {
                jointList_[2] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "control3")
            {
                jointList_[3] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "control4")
            {
                jointList_[4] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "control5")
            {
                jointList_[5] = RobotChildren[i].gameObject;
            }
        }
    }

    // Create the list of GameObjects that represent each slider in the canvas
    public void initializeSliders(Slider[] sliderList_) {
        var CanvasChildren = CanvasObj.GetComponentsInChildren<Slider>();

        for (int i = 0; i < CanvasChildren.Length; i++) {
            if (CanvasChildren[i].name == "Slider0")
            {
                sliderList_[0] = CanvasChildren[i];

                sliderList_[0].minValue = lowerLimit_s[0];
                sliderList_[0].maxValue = upperLimit_s[0];
                sliderList_[0].value = 0;
            }
            else if (CanvasChildren[i].name == "Slider1")
            {
                sliderList_[1] = CanvasChildren[i];

                sliderList_[1].minValue = lowerLimit_s[1];
                sliderList_[1].maxValue = upperLimit_s[1];
                sliderList_[1].value = 0;
            }
            else if (CanvasChildren[i].name == "Slider2")
            {
                sliderList_[2] = CanvasChildren[i];

                sliderList_[2].minValue = lowerLimit_s[2];
                sliderList_[2].maxValue = upperLimit_s[2];
                sliderList_[2].value = 0;
            }
            else if (CanvasChildren[i].name == "Slider3")
            {
                sliderList_[3] = CanvasChildren[i];

                sliderList_[3].minValue = lowerLimit_s[3];
                sliderList_[3].maxValue = upperLimit_s[3];
                sliderList_[3].value = 0;
            }
            else if (CanvasChildren[i].name == "Slider4")
            {
                sliderList_[4] = CanvasChildren[i];

                sliderList_[4].minValue = lowerLimit_s[4];
                sliderList_[4].maxValue = upperLimit_s[4];
                sliderList_[4].value = 0;
            }
            else if (CanvasChildren[i].name == "Slider5")
            {
                sliderList_[5] = CanvasChildren[i];

                sliderList_[5].minValue = lowerLimit_s[5];
                sliderList_[5].maxValue = upperLimit_s[5];
                sliderList_[5].value = 0;
            }
        }
    }
}
