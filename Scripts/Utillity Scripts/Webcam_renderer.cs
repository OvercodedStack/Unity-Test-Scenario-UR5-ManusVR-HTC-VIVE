///////////////////////////////////////////////////////////////////////////////
//
//  Original System: UR5_to_TPC.cs
//  Subsystem:       Human-Robot Interaction
//  Workfile:        Unity workspace?
//  Revision:        1.0 - 7/27/2018
//  Author:          Esteban Segarra
//
//  Description
//  ===========
//  Webcam canvas poster. This is used to grab data from live webcams and transmit them onto panels. 
//  Tutorial at: https://www.youtube.com/watch?v=P5zxAyrImV0
///////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Webcam_renderer : MonoBehaviour {
    [Range(0,10)]
    public int vals = 1;

	// Use this for initialization
	void Start () {
        WebCamDevice[] devices = WebCamTexture.devices;
        WebCamTexture webcamTexture = new WebCamTexture();
        webcamTexture.deviceName = devices[vals].name;
        this.GetComponent<MeshRenderer>().material.mainTexture = webcamTexture;
        webcamTexture.Play();
	}

}
