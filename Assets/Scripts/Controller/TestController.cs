using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
		buffer.EnqueueBytes (BitConverter.GetBytes (1));
		new PlayerJoinedMessage(1).Serialize (buffer);
		buffer.Print ();
		Debug.Log ("----");

		int b = buffer.DequeueEnum (MessageType.TOTAL);
		Debug.Log (b);
		buffer.Print ();
		Debug.Log ("----");
		b = buffer.DequeueEnum (ServerMessageType.TOTAL);
		Debug.Log (b);
		buffer.Print ();
		Debug.Log ("----");
		byte[] arr = buffer.DequeueBytes (4);
		Debug.Log(BitConverter.ToInt32 (arr, 0));
		//Debug.Log(new PlayerJoinedMessage (buffer));
		buffer.Print ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
