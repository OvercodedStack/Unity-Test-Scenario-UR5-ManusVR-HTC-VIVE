///////////////////////////////////////////////////////////////////////////////
//
//  Original System: Manus_interpreter.h.cpp
//  Subsystem:       Human-Robot Interaction
//  Workfile:        Manus_interpreter.cs
//  Revision:        1.1 - 6/11/2018
//  Author:          Esteban Segarra
//
//  Description
//  ===========
//  Manus_VR Hand data phraser - intended to wrap around a gameobject and provide nessesary values to 
//  a mesh in order to simulate compression and orientation.
//
///////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ManusVR;
using manus_interface;



public class Manus_hand_model_inputter : MonoBehaviour
{
    [SerializeField]
    //private string hand_side;                   //Which hand side to use 
    //public Vector3 test_my_finger;
    //public Vector3 my_other_finger;
    public bool is_right = false;
    public bool enable_manus_rotation = false;
    public double[] raw_list;
    public Vector3 hand_position;               //Adjust the postion of the hand object
    public Quaternion hand_orientation;         //Adjust the orientation of the hand object 
    private Quaternion[] finger_orientations;   //Adjust the orientation of the finger objects 
    private double[] finger_compression;        //Finger compression
    private bool hand_side_right;
    GameObject hand_mesh;                       //Setup GameObjects
    GameObject my_wrist;
    GameObject[] fingers_occulus;
    Manus_interpreter manus_interpret;          //The Manus API interpreter
    device_type_t type_of_device;               //Which side is this hand on - on Manus terms


    void Start()
    {
        fingers_occulus = new GameObject[15]; //Gameobject array for each of the bones. 
        string wrist_string, offset;
        string hand_side;
        //string fing_0, fing_1, fing_2, fing_3, fing_4, fing_5, fing_6, fing_7, fing_8, fing_9, fing_10, fing_11, fing_12, fing_13, fing_14; //Finger strings illeterable; 
        string[] fings = new string[15];
        ///string indx_1, indx_2, indx_3, mid_1, mid_2, mid_3;
        ///string rng_1, rng_2, rng_3, pnk_1, pnk_2, pnk_3;
        ///string thm_1, thm_2, thm_3;


        //Each bone in the Occulus hand has a unique identifier that links it back to the mesh. By using this method, one can illeterate through it and assign the unique IDs 

        if (is_right == true)                                  //Check if my hand is left or right and setup accordingly 
        {
            hand_side = "RightHand";
            hand_side_right = true;
            type_of_device = device_type_t.GLOVE_RIGHT;
            wrist_string = "RightHand/hands:r_hand_world";
            offset = wrist_string + "/hands:b_r_hand";
            fings[0] = offset + "/hands:b_r_index1";
            fings[1] = fings[0] + "/hands:b_r_index2";
            fings[2] = fings[1] + "/hands:b_r_index3";
            fings[3] = offset + "/hands:b_r_middle1";
            fings[4] = fings[3] + "/hands:b_r_middle2";
            fings[5] = fings[4] + "/hands:b_r_middle3";
            fings[6] = offset + "/hands:b_r_ring1";
            fings[7] = fings[6] + "/hands:b_r_ring2";
            fings[8] = fings[7] + "/hands:b_r_ring3";
            fings[9] = offset + "/hands:b_r_pinky0/hands:b_r_pinky1";
            fings[10] = fings[9] + "/hands:b_r_pinky2";
            fings[11] = fings[10] + "/hands:b_r_pinky3";
            fings[12] = offset + "/hands:b_r_thumb1";
            fings[13] = fings[12] + "/hands:b_r_thumb2";
            fings[14] = fings[13] + "/hands:b_r_thumb3";
        }
        else
        {
            hand_side = "LeftHand";
            hand_side_right = false;                                    //Left hand becomes the 0th 
            type_of_device = device_type_t.GLOVE_LEFT;
            wrist_string = "LeftHand/hands:l_hand_world";
            offset = wrist_string + "/hands:b_l_hand";
            fings[0] = offset + "/hands:b_l_index1";                    //index bone 1 
            fings[1] = fings[0] + "/hands:b_l_index2";                  //index bone 2 
            fings[2] = fings[1] + "/hands:b_l_index3";                  //index bone 3 
            fings[3] = offset + "/hands:b_l_middle1";                   //middle bone 1 
            fings[4] = fings[3] + "/hands:b_l_middle2";                 //middle bone 2 
            fings[5] = fings[4] + "/hands:b_l_middle3";                 //middle bone 3 
            fings[6] = offset + "/hands:b_l_ring1";                     //ring bone 1
            fings[7] = fings[6] + "/hands:b_l_ring2";                   //ring bone 1
            fings[8] = fings[7] + "/hands:b_l_ring3";                   //ring bone 1
            fings[9] = offset + "/hands:b_l_pinky0/hands:b_l_pinky1";   //pinky bone 1
            fings[10] = fings[9] + "/hands:b_l_pinky2";                 //pinky bone 2
            fings[11] = fings[10] + "/hands:b_l_pinky3";                //pinky bone 3
            fings[12] = offset + "/hands:b_l_thumb1";                   //thumb bone 1
            fings[13] = fings[12] + "/hands:b_l_thumb2";                //thumb bone 2
            fings[14] = fings[13] + "/hands:b_l_thumb3";                //thumb bone 3
        }
        hand_mesh = GameObject.Find(hand_side);                         //Find that object
        my_wrist = GameObject.Find(wrist_string);                       //and that one too. 
        for (int i = 0; i < 15; i++)
        {
            fingers_occulus[i] = GameObject.Find(fings[i]);
        }
        manus_interpret = this.GetComponent<Manus_interpreter>();       //Find the API 
    }

    // Update is called once per frame
    void Update()
    {
        Manus_hand_obj hand = manus_interpret.get_hand(type_of_device); //Select glove
        List<Finger> orientations = hand.get_hand();
        List<double> raw_fings = hand.get_raw_hand();
        //List<pose> poses_orien = orientations
        raw_list = raw_fings.ToArray();


        //Get the rotation of the quaternion and set equal
        if (enable_manus_rotation)
        {
            Vector3 temp_q = hand.get_wrist().eulerAngles;
            Quaternion q = hand.get_wrist();
            my_wrist.transform.eulerAngles = new Vector3(temp_q.x, temp_q.z, temp_q.y);
        }

        hand_orientation = hand.get_wrist();
        ////Setting the orientation of the fingers. 
        ////check_me_lol = orientations[2].get_finger_data()[2].rotation;
        ////Quaternion temp_q = orientations[0].get_finger_data()[2].rotation;
        ////Quaternion out_q = new Quaternion((float) (temp_q.x * 180 / 3.14168), (float)(temp_q.y * 180 / 3.14168), (float)(temp_q.z * 180 / 3.14168), (float)(-temp_q.w * 180 / 3.14168));
        //// Quaternion out_q = new Quaternion(0, 0, (float)(temp_q.w * 180 / 3.14168), 0);
        ////check_me = out_q;
        ////fingers_occulus[0].transform.localRotation = out_q;



        //Index Finger
        Vector3 out_vector = new Vector3(0, 0, -100F * (float)raw_fings[7]); 
        fingers_occulus[1].transform.localEulerAngles = out_vector;
        out_vector = new Vector3(0, 0, -72F * (float)raw_fings[6]);
        fingers_occulus[2].transform.localEulerAngles = out_vector;

        out_vector = new Vector3(-74.875F, 106.624F, ((float)raw_fings[7] * (float)raw_fings[6] * -59.667F )+ 54.667F );
        fingers_occulus[0].transform.localEulerAngles = out_vector;

        //Middle Finger
        out_vector = new Vector3(0, 0,  -115F * (float)raw_fings[5]);
        fingers_occulus[4].transform.localEulerAngles = out_vector;
        out_vector = new Vector3(0, 0, -105F * (float)raw_fings[4]);
        fingers_occulus[5].transform.localEulerAngles = out_vector;

        out_vector = new Vector3(-80.399F, 18.783F, ((float)raw_fings[7] * (float)raw_fings[6] * -73.9F) +151.9F);
        fingers_occulus[3].transform.localEulerAngles = out_vector;

        //Ring Finger
        out_vector = new Vector3(0, 0,  -100F* (float)raw_fings[3]);
        fingers_occulus[7].transform.localEulerAngles = out_vector;
        out_vector = new Vector3(0, 0, -110F * (float)raw_fings[2]);
        fingers_occulus[8].transform.localEulerAngles = out_vector;

        out_vector = new Vector3(-69.153F, -21.521F, ((float)raw_fings[3] * (float)raw_fings[2] * -68.448F) - 166.516F);
        fingers_occulus[6].transform.localEulerAngles = out_vector;

        //Pinky Finger
        out_vector = new Vector3(0, 0, -90F * (float)raw_fings[1]);
        fingers_occulus[10].transform.localEulerAngles = out_vector;
        out_vector = new Vector3(0, 0, -100F * (float)raw_fings[0]);
        fingers_occulus[11].transform.localEulerAngles = out_vector;

        out_vector = new Vector3(-1.022F, -2.061F, (((float)raw_fings[1]+0.4F) * (float)raw_fings[0] * -83.518F) + -169.447F);
        fingers_occulus[9].transform.localEulerAngles = out_vector;

        //Thumb Finger
        out_vector = new Vector3(0, 0, -60F * (float)raw_fings[9]);
        fingers_occulus[13].transform.localEulerAngles = out_vector;
        out_vector = new Vector3(0, 0, -90F * (float)raw_fings[8]);
        fingers_occulus[14].transform.localEulerAngles = out_vector;

        out_vector = new Vector3(-11.73F, 183.41F, ((float)raw_fings[9] * (float)raw_fings[8] * -39.229F) + 27.229F);
        fingers_occulus[12].transform.localEulerAngles = out_vector;
    }
}
