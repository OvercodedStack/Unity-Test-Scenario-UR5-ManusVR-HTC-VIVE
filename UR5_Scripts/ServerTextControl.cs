
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ServerTextControl : NetworkBehaviour {
    private TextMesh DisplayText;
    public InputField ServerText;

	// Use this for initialization
	void Start () {
        DisplayText = this.GetComponent<TextMesh>();
    }
	
	// Update is called once per frame
	void Update () {
        DisplayText.text = ServerText.text;
	}
}
