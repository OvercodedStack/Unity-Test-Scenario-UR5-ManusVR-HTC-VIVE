///////////////////////////////////////////////////////////////////////////////
//
//  Original System: Mouse_drag.cs
//  Subsystem:       Human-Robot Interaction
//  Workfile:        Manus_Open_VR V2
//  Revision:        1.0 - 6/22/2018
//  Author:          Esteban Segarra
//
//  Description
//  ===========
//  Wrapper to move gameobjects in-game
//
///////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse_drag : MonoBehaviour {
    public float distance;
    public float additive_ratio = 1.0f;


    private void OnMouseDown()
    {
        distance = Vector3.Distance(this.transform.position,Camera.main.transform.position);
    }


    public Vector3 debug_vector;
    private void OnMouseDrag()
    {      
        Vector3 mouse_pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 obj_pos = Camera.main.ScreenToWorldPoint(mouse_pos);

        debug_vector = obj_pos;

        transform.position = obj_pos; 
    }

    void LateUpdate()
    {
        if (Input.GetButton("Q"))
        {
            distance += additive_ratio;
        }
        if (Input.GetButton("E"))
        {
            distance += -additive_ratio;
        }
    }
}
