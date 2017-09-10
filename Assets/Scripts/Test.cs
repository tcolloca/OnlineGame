using System;
using UnityEngine;

public class Test : MonoBehaviour {

	public void Start () {
		Serializer serializer = new Serializer ();
		byte[] bytes = serializer.Serialize (new JoinMessage (5));
		Message obj = serializer.Deserialize (bytes);
	}

}

