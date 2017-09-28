using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public PlayerInput playerInput { get; set; }
	
	// Update is called once per frame
	void Update () {
		Debug.Log ("Player controller update");
		bool up, down, left, right, shoot;
		up = down = left = right = shoot = false;
		if (Input.GetKey (KeyCode.W)) {
			up = true;
		}
		if (Input.GetKey (KeyCode.S)) {
			down = true;
		}
		if (Input.GetKey (KeyCode.A)) {
			left = true;
		}
		if (Input.GetKey (KeyCode.D)) {
			right = true;
		}
		if (Input.GetKey (KeyCode.RightShift)) {
			shoot = true;
		}
		playerInput = new PlayerInput (up, down, left, right, shoot);
	}
}
