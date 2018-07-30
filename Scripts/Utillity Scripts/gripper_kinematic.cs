///////////////////////////////////////////////////////////////////////////////
//
//  Original System: gripper_kinematics.cs
//  Subsystem:       Human-Robot Interaction
//  Workfile:        Manus_interpreter.cs
//  Revision:        1.0 - 7/5/2018
//  Author:          Esteban Segarra
//
//  Description
//  ===========
//  Gripper Control on UR5. This used to view the asthetics on the simulated UR5
//
///////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gripper_kinematic : MonoBehaviour {
    private GameObject gripper_left, gripper_right;
    private Vector3 manipulator_grip_L, manipulator_grip_R; 
    private float max_limit = 0.3f;
    private float min_limit = 0.075f;
    //private VIVE_controller controller;
    private float grip_ratio = 0;

    // Use this for initialization
    void Start () {
        gripper_left = GameObject.Find("Left Handle");
        gripper_right = GameObject.Find("Right Handle");
        //GameObject controller_device = GameObject.Find("Controller (left)");
        //controller = controller_device.GetComponent<VIVE_controller>();
	}
	
    public void set_grip(float val)
    {
        grip_ratio = val; 
    }

	// Update is called once per frame
	void Update () {
        
        //Set a float that goes from 0.000 - 1.000
        //controller.get_ratio();
        update_grippers(grip_ratio);
	}

    public float get_ratio()
    {
        return grip_ratio;
    }

    void update_grippers(float close_ratio)
    {
        float my_grip_ratio = 0.3f - (0.225f * close_ratio);
        manipulator_grip_L = new Vector3(0, 2, my_grip_ratio);
        manipulator_grip_R = new Vector3(0, 2, -my_grip_ratio);
        gripper_left.transform.localPosition = manipulator_grip_L;
        gripper_right.transform.localPosition = manipulator_grip_R;

    }
}
