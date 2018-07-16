using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Runtime;

public class drive : MonoBehaviour {
    float speed = 10.0F;
    float rotSpeed = 100.0F;

	// Use this for initialization
	//void Start () {
	//}
	
	// Update is called once per frame
	void Update () {
        float trans = CrossPlatformInputManager.GetAxis("Vertical") * speed;
        float rot = CrossPlatformInputManager.GetAxis("Horizontal") * rotSpeed;

        trans *= Time.deltaTime;
        rot *= Time.deltaTime;
        transform.Translate(0, 0, trans);
        transform.Rotate(0, rot, 0);
	}
}
