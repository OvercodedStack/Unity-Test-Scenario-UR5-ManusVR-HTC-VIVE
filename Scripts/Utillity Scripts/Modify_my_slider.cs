///////////////////////////////////////////////////////////////////////////////
//
//  Original System: modify_my_slider.cs
//  Subsystem:       Human-Robot Interaction
//  Workfile:        Standalone
//  Revision:        1.0 - 6/13/2018
//  Author:          Esteban Segarra
//
//  Description
//  ===========
//  Wrapper script to change the value of a slider on the gameobject. 
//
///////////////////////////////////////////////////////////////////////////////
 
using UnityEngine;
using UnityEngine.UI;
using manus_interface;
using ManusVR;

public class Modify_my_slider : MonoBehaviour {
    private Slider[] sliders_on_gameObj;
    private Manus_interpreter manus_inpt;
    private float z1, z2, z3, z4, z5; // Motor movement overrides
    public Toggle toggle_manus;

    //Initialization
    void Start () {
        sliders_on_gameObj = this.GetComponentsInChildren<Slider>();    //Get my components
        //manus_inpt = this.GetComponent<Manus_interpreter>();            //Get my manus API
        GameObject game_thing = GameObject.Find("Manus_VR_Driver");     //Seek global API
        manus_inpt = game_thing.GetComponent<Manus_interpreter>();
       
    }

    void Update()
    {
        if (toggle_manus.isOn)
        {
            Manus_hand_obj hand = manus_inpt.get_hand(device_type_t.GLOVE_RIGHT);   //Select glove
            double[] hand_compression_ratios = hand.get_raw_hand().ToArray();       //Array conversion
            z1 = (float)hand_compression_ratios[6]; //Index
            z2 = (float)hand_compression_ratios[4]; //Middle
            z3 = (float)hand_compression_ratios[2]; //Ring
            z4 = (float)hand_compression_ratios[0]; //Pinky
            z5 = (float)hand_compression_ratios[8]; //Thumb

            //Slider Overrides
            sliders_on_gameObj[0].value = z5 * 111F - 85.0F;    //Rotate the base
            sliders_on_gameObj[1].value = z1 * 350.0F - 175.0F; //Move the second arm
            sliders_on_gameObj[2].value = z2 * 310.0F - 155.0F; //Move the first arm
            sliders_on_gameObj[3].value = z3 * 690.0F - 345.0F; //Rotate the last arm
            sliders_on_gameObj[4].value = 0;                    //???
                                                                ///sliders_on_gameObj[5] exists but useless for now. 
        }
    }
}
