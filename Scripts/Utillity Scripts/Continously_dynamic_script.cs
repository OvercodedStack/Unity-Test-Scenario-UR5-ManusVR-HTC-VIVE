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
//  Script meant to enable the collision to continously dynamic for objects in a gameobject.
//
///////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Make_me_coninously_dynamic : MonoBehaviour {
	// Use this for initialization
	void Start () {
        Rigidbody[] rig;
        rig = this.GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rig.Length; i++)
        {
            rig[i].collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
    }
}
