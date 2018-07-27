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
