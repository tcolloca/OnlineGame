using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkView : MonoBehaviour {

	public int id;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void UpdatePosition (Vector2 position) {
		transform.position = position;
	}
}
