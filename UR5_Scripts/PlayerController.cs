
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour {
    private TextMesh DisplayText;
    private InputField MessageOutput;

    void Start() {
        DisplayText = this.GetComponent<TextMesh>();

        //if (isServer) {
        GameObject IF = GameObject.Find("Canvas/Scaler/MessageOutput");
        MessageOutput = IF.GetComponent<InputField>();

        DisplayText.text = "";
        MessageOutput.text = "";
        //}

        //GameObject Canvas = GameObject.Find("Canvas");
    }

    void Update() {
        if (!isLocalPlayer)
        {
            //DisplayText.text = "not localplayer";
            return;
        }

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);

        //DisplayText.text = ServerText.text;
        SendMsg(MessageOutput.text);
    }


    public override void OnStartLocalPlayer() {
        //GetComponent<MeshRenderer>().material.color = Color.blue;
        return;
    }

    [ClientRpc]
    void RpcMsg(string msg)
    {
        //Debug.Log("Sent msg:" + msg);
        if (DisplayText.text != msg)
        {
            DisplayText.text = msg;
        }
    }

    public void SendMsg(string msg)
    {
        //if (!isServer)
        //    return;
        
        RpcMsg(msg);
    }
}