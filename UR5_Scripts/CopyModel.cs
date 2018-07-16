using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyModel : MonoBehaviour {
    public GameObject inputModel;
    private GameObject[] jointListIn = new GameObject[6];
    private GameObject[] jointListOut = new GameObject[6];

    // Use this for initialization
    void Start ()
    {
        initializeJoints(inputModel, jointListIn);
        initializeJoints(this.gameObject, jointListOut);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // Right before camera renders
    void LateUpdate()
    {

        for (int i = 0; i < 6; i++)
        {
            Vector3 currentRotation = jointListIn[i].transform.localEulerAngles;
            //Debug.Log(currentRotation);
            //currentRotation.z = jointValues[i];
            jointListOut[i].transform.localEulerAngles = currentRotation;
        }
    }


    // Create the list of GameObjects that represent each joint of the robot
    void initializeJoints(GameObject RobotBase, GameObject[] jointList)
    {
        var RobotChildren = RobotBase.GetComponentsInChildren<Transform>();
        for (int i = 0; i < RobotChildren.Length; i++)
        {
            if (RobotChildren[i].name == "control0")
            {
                jointList[0] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "control1")
            {
                jointList[1] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "control2")
            {
                jointList[2] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "control3")
            {
                jointList[3] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "control4")
            {
                jointList[4] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "control5")
            {
                jointList[5] = RobotChildren[i].gameObject;
            }
        }
    }


}
