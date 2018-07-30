///////////////////////////////////////////////////////////////////////////////
//
//  Original System: UR5_to_TPC.cs
//  Subsystem:       Human-Robot Interaction
//  Workfile:        Unity workspace?
//  Revision:        1.0 - 6/29/2018
//  Author:          Esteban Segarra
//
//  Description
//  ===========
//  Data phraser from UR5 to TPC server. 
//
///////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using UnityEngine.UI;
using TPC_Server;

public class UR5_to_TPC : MonoBehaviour
{
    public string output_string;
    TCP_Server server;
    ur5_kinematics angle_controller;
    gripper_kinematic grip_obj;
    public GameObject robot;

    // Use this for initialization
    void Start()
    {
        server = GetComponent<TCP_Server>();
        GameObject gripper = GameObject.Find("Base_Gripper");
        grip_obj = gripper.GetComponent<gripper_kinematic>();

        //GameObject robot = GameObject.Find("UR5");
        angle_controller = robot.GetComponent<ur5_kinematics>();
    }

    //Add the grippper data to the string
    string add_gripper()
    {
        string out_stg = null;

        out_stg += "Gripper:";
        out_stg += grip_obj.get_ratio().ToString();
        out_stg += "; ";
        return out_stg;
    }

    //Runstate
    private void Update()
    {
        output_string = "";
        output_string = convert_array(angle_controller.get_vector_UR5());
        output_string += add_gripper();
        output_string += "\0 NUL NULL"; //Termination of string required on backend of websocket (C++) 
        //server.SendMessage(output_string);
        server.set_msg(output_string);
    }

    //Convert an array into a formatted string. 
    string convert_array(float[] array_in)
    {
        string output_str = null;
        output_str += "UR5_pos>";
        for (int i = 0; i < 6; i++)
        {
            output_str += array_in[i].ToString();
            if (i < 5)
                output_str += ",";
        }
        output_str += ";";
        return output_str;
    }
}