using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		BitBuffer buffer = new BitBuffer ();
		buffer.EnqueueEnum (MessageType.SERVER, MessageType.TOTAL);
		buffer.Print ();
		Debug.Log ("----");
		buffer.EnqueueEnum (ServerMessageType.PLAYER_JOINED, ServerMessageType.TOTAL);
		buffer.Print ();
		Debug.Log ("----");
		buffer.EnqueueBytes (new PlayerJoinedMessage(1).Serialize ());
		buffer.Print ();
		Debug.Log ("----");


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
