using System;
using UnityEngine;
using UnityEngine.UI;

using System.Text.RegularExpressions;

public class distributePose : MonoBehaviour {
    private InputField thisField;
    private string lastPose;

    public InputField MessageOutput;
    public int idx;

    private char[] poseComponents = {'x','y','z','r','p','y'};

	// Use this for initialization
	void Start () {
        thisField = this.GetComponent<InputField>();

        thisField.text = String.Format("{0}: 0", poseComponents[idx]);

        lastPose = MessageOutput.text;
    }
	
	// Update is called once per frame
	void Update () {
        string pose = MessageOutput.text;

        if (!pose.Equals(lastPose))
        {
            Debug.Log("New pose received");
            string resultString = Regex.Match(pose, @"\d.d+").Value;
            Debug.Log(resultString);
        }

        lastPose = pose;
    }
}
