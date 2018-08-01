///////////////////////////////////////////////////////////////////////////////
//
//  Original System: UR5_to_TPC.cs
//  Subsystem:       Human-Robot Interaction
//  Workfile:        Unity workspace?
//  Revision:        1.0 - 7/7/2018
//  Author:          Esteban Segarra
//
//  Description
//  ===========
//  Interaction class for objects 
//  Adopted from https://www.youtube.com/watch?v=q86rb1Eq-bA
///////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interaction : MonoBehaviour
{
    private FixedJoint attach_joint = null;
    private Rigidbody current_rigidBody = null;
    private List<Rigidbody> rigidbody_list = new List<Rigidbody>();

    // Use this for initialization
    void Awake()
    {
        attach_joint = GetComponent<FixedJoint>();
    }

    //If Colliding, check for interaction.
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Interact"))
        {
            return;
        }
        rigidbody_list.Add(other.gameObject.GetComponent<Rigidbody>());
    }

    //Check if no longer colliding
    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Interact"))
        {
            return;
        }
        rigidbody_list.Remove(other.gameObject.GetComponent<Rigidbody>());

    }

    public void Pickup()
    {
        current_rigidBody = Get_Nearest_RigidBody();

        if (!current_rigidBody)
        {
            return;
        }
        //Copy position of controller
        current_rigidBody.transform.position = transform.position;
        //current_rigidBody.transform.rotation = transform.rotation;

        //Set to the Controller
        attach_joint.connectedBody = current_rigidBody;

    }


    //Methods to rotate around, this one is working specifically for trackpad rotation
    public void Rotate(float x, float y)
    {
        float multiplier = 2.5f;
        current_rigidBody = Get_Nearest_RigidBody();

        if (!current_rigidBody)
        {
            //Debug.Log("no");
            return;
        }
        Vector3 rotation_new = current_rigidBody.transform.localEulerAngles;
        rotation_new.x += multiplier * x;
        rotation_new.y += multiplier * y;

        current_rigidBody.transform.localEulerAngles = rotation_new;
        attach_joint.connectedBody = current_rigidBody;
        //Debug.Log("Rot");
    }

    //Rotating around quaternion
    public void Rotate(Quaternion rotation)
    {
        current_rigidBody = Get_Nearest_RigidBody();

        if (!current_rigidBody)
        {
            return;
        }
        current_rigidBody.transform.rotation = rotation;
        attach_joint.connectedBody = current_rigidBody;

    }

    //Rotate about the selected rigidbody. 
    public void Rotate()
    {
        current_rigidBody = Get_Nearest_RigidBody();

        if (!current_rigidBody)
        {
            return;
        }
        current_rigidBody.transform.rotation = transform.rotation;
        attach_joint.connectedBody = current_rigidBody;

    }

    //Overloading for dropping with a device
    public void Drop(SteamVR_Controller.Device device)
    {
        if (!current_rigidBody)
        {
            return;
        }
        current_rigidBody.velocity = device.velocity;
        current_rigidBody.angularVelocity = device.angularVelocity;

        attach_joint.connectedBody = null;
        current_rigidBody = null;


    }

    //Overloading in case there's no device just a rigidbody. 
    public void Drop(Rigidbody device)
    {
        if (!current_rigidBody)
        {
            return;
        }
        current_rigidBody.velocity = device.velocity;
        current_rigidBody.angularVelocity = device.angularVelocity;

        attach_joint.connectedBody = null;
        current_rigidBody = null;


    }

    private Rigidbody Get_Nearest_RigidBody()
    {
        Rigidbody nearest_rigid = null;

        float min_dist = float.MaxValue;
        float dist = 0.0f;

        foreach (Rigidbody contact_body in rigidbody_list)
        {
            dist = (contact_body.gameObject.transform.position - transform.position).sqrMagnitude;
            if (dist < min_dist)
            {
                min_dist = dist;
                nearest_rigid = contact_body;
            }
        }
        return nearest_rigid;
    }

}
