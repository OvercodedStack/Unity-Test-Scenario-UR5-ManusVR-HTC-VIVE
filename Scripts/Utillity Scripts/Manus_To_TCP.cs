///////////////////////////////////////////////////////////////////////////////
//
//  Original System: Manus_To_TCP.cs
//  Subsystem:       Human-Robot Interaction
//  Workfile:        Unity workspace?
//  Revision:        1.0 - 6/29/2018
//  Author:          Esteban Segarra
//
//  Description
//  ===========
//  Data phraser from Manus to TPC server. 
//
//  ==============================NOT IN USE===================================
//
//
///////////////////////////////////////////////////////////////////////////////




//==============================NOT IN USE===================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using manus_interface;
 
using TPC_Server;

public class Manus_To_TCP : MonoBehaviour {
    TCP_Server server;
    Manus_interpreter interpreter;
    ur5_kinematics angle_controller;

	// Use this for initialization
	void Start () {
        server = this.GetComponent<TCP_Server>();
        GameObject locator = GameObject.Find("Manus_VR_Driver");
        interpreter = locator.GetComponent<Manus_interpreter>();

	}

	// Update is called once per frame
	void Update () {
        //convert_array
        //string converted_string_manus = consodiliate_strings();
        ////server.IPC_comms_message = converted_string_manus;
        //server.SendMessage();

	}

 //   ==============================NOT IN USE===================================
    //string convert_array()
    //{

    //}

    string consodiliate_strings()
    {
        Manus_hand_obj device0 = interpreter.get_hand(0);
        Manus_hand_obj device1 = interpreter.get_hand(1);
        try
        {
            string stringed_array = null;
            double[] raw_values_manus = device0.get_raw_hand().ToArray() ;
            //raw_values_manus += device1.get_raw_hand().ToArray();

            return stringed_array;
        }
        catch
        {
            return "Problem with return type";
        }
        return "null";
    }
    //   ==============================NOT IN USE===================================


}
