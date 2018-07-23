///////////////////////////////////////////////////////////////////////////////
//
//  Original System: hand_pickup.cs
//  Subsystem:       Human-Robot Interaction
//  Workfile:        standalone
//  Revision:        1.0 - 7/12/2018
//  Author:          Esteban Segarra
//
//  Description
//  ===========
//  Script for moving objects using another gameobject.
//
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using manus_interface;

public class hand_pickup : MonoBehaviour
{
    //Locate interpreter
    Manus_interpreter manus_inpt;
    GameObject target_object;
    public bool is_right;
    public bool is_grab;
    public Vector3 distance_to_target;
    public GameObject Manus_API;
    public string name;
    public bool is_thumbs_up;
    private interaction interact;
    private double[] finger_array; // Temp array to collect finger array values

    // Use this for initialization
    void Start()
    {
        //GameObject game_thing = GameObject.Find("Manus_VR_Driver");     //Seek global API

        manus_inpt = Manus_API.GetComponent<Manus_interpreter>();
        interact = this.GetComponent<interaction>();
    }

    // Update is called once per frame
    //void Update () {


    //void OnCollisionEnter(Collision collision)
    //{

    //    target_object = GameObject.Find(collision.collider.name); //Locate the other object this object is in collision with. 
    //    name = collision.collider.name;
    //     distance_to_target = target_object.transform.position - this.transform.position ;//Distance offset
    //    Debug.Log("Clicked");
    // }

    private void Update()
    {
        is_grab = manus_inpt.get_hand(is_right).get_grabbing(); //Rightmost hand
        finger_array = manus_inpt.get_hand(is_right).get_raw_hand().ToArray();
        int counter = 0;
        
        for (int i = 0; i < 8; i++)
        {        
            if (finger_array[i] > .85)
            {
                counter++;
            }
        }

        if (counter >= 7 && finger_array[8] < .3 && finger_array[9] < .3)
        {
            is_thumbs_up = true;
            interact.Rotate();

        }
        else
        {
            is_thumbs_up = false; 
        }


        if (is_grab)
        {
            //target_object.transform.position = distance_to_target + this.transform.position;
            interact.Pickup();
        }
        else
        {
            interact.Drop(this.GetComponent<Rigidbody>());
        }

       

    }
}

