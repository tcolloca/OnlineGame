using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		BitBuffer buffer = new BitBuffer ();
		Serializer serializer = new Serializer ();
		byte[] bytes = serializer.Serialize (new MovementMessage (5, new bool[] {true, true, false, false}));
		Debug.Log (serializer.Deserialize (bytes));
	}

	// Update is called once per frame
	void Update () {
		
	}
}
