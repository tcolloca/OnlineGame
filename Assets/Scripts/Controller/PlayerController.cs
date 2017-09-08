using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed = 0.5f;
	private Player player;
	private PlayerInputRecorder inputRecorder;

	void Start () {
		player = new Player (1);
		inputRecorder = new PlayerInputRecorder (player);
	}
	
	void Update () {
		inputRecorder.Record ();
	}
}
