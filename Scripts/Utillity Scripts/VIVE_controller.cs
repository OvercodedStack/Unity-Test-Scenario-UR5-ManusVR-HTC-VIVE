///////////////////////////////////////////////////////////////////////////////
//
//  Original System: Vive_controller.cs
//  Subsystem:       Human-Robot Interaction
//  Workfile:        Manus_interpreter.cs
//  Revision:        1.0 - 7/5/2018
//  Author:          Esteban Segarra
//
//  Description
//  ===========
//  Vive controller wrapper
//
///////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VIVE_controller : MonoBehaviour {
    //Variables and constants. 
    private SteamVR_TrackedObject track_obj;
    private SteamVR_Controller.Device device_obj;
    private interaction interact;
    public float gripper_ratio = 0;
    public float gripper_speed = 0.01f;
    private bool release_gripper = false;
    public GameObject gripper;
    private gripper_kinematic kine;
    

	// Use this for initialization
	void Start () {
        track_obj = GetComponent<SteamVR_TrackedObject>();
        interact = this.GetComponent<interaction>();
        kine = gripper.GetComponent<gripper_kinematic>();
         
    }

    // Update is called once per frame
    void Update () {
        device_obj = SteamVR_Controller.Input((int)track_obj.index);

        //Conditionals for when the controller's trigger is pressed. 
        if (device_obj.GetPress
            (SteamVR_Controller.ButtonMask.Trigger))
        {
            device_obj.TriggerHapticPulse(100);
            interact.Pickup();
        } 
        //Trigger conditional 
        if (device_obj.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            interact.Drop(device_obj);
        }


        //If the grip on the controller is gripped, the gripper on the UR will close 
        if (device_obj.GetPress(SteamVR_Controller.ButtonMask.Grip)){
            if (gripper_ratio < 1.0f)
                gripper_ratio += gripper_speed;
            if (gripper_ratio >= 1.0f)
                gripper_ratio = 1.0f;
               // device_obj.TriggerHapticPulse(10000);
            device_obj.TriggerHapticPulse(100);
        }

        //IF the menu button is pressed, the gripper will attempt to open.
        if (device_obj.GetPress(SteamVR_Controller.ButtonMask.ApplicationMenu))
            release_gripper = !release_gripper;


        //Release gripper conditional. 
        if (release_gripper)
            {
            //If greater, attempt to "close"
            if (gripper_ratio > 0)
                gripper_ratio -= gripper_speed; 
            //If shut, reset. 
            if (gripper_ratio <= 0)
            {
                gripper_ratio = 0;
                device_obj.TriggerHapticPulse(10000);
                release_gripper = false; 
            }
        }


        //Touchpad controls for rotating. 
        if (device_obj.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            interact.Drop(device_obj);
            float tiltAroundX = device_obj.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).x;
            float tiltAroundY = device_obj.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).y;
            interact.Rotate(tiltAroundX, tiltAroundY);
            device_obj.TriggerHapticPulse(100);
           // Debug.Log("Bounce");
        }
        if (device_obj.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
            interact.Drop(device_obj);
        Vector2 trig_Val = device_obj.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
    }

    //Return gripper status
    public float get_ratio()
    {
        return gripper_ratio;
    }
}
